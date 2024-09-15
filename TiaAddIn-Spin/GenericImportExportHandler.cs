using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static SpinAddin.Utility.ExportUtil;

namespace SpinAddIn
{

    public class ImportData<GROUP>
    {
        public readonly GROUP group;
        public readonly FileInfo fileInfo;
        public readonly ImportOptions importOptions;
        public readonly SWImportOptions swImportOptions;
        public ImportData(GROUP group, FileInfo fileInfo, ImportOptions importOptions, SWImportOptions swImportOptions)
        {
            this.group = group;
            this.fileInfo = fileInfo;
            this.importOptions = importOptions;
            this.swImportOptions = swImportOptions;
        }
    }


    public class GenericImportExportHandler<OBJ, GROUP> : SpinAddinMenuRegistrationService where OBJ : IEngineeringObject where GROUP : IEngineeringObject
    {
        private readonly string descriptiveName;
        private readonly Func<OBJ, ExportDelegate> exportDelegateFunction;
        private readonly Func<GROUP, string> groupNameFunction;
        private readonly Func<OBJ, string> objNameFunction;
        private readonly Func<OBJ, GROUP> parentFunction;
        private readonly Func<GROUP, IEnumerable<OBJ>> containedObjsFunction;
        private readonly Func<GROUP, IEnumerable<GROUP>> containedSubGroupsFunction;
        private readonly Predicate<ImportData<GROUP>> importPredicate;
        public GenericImportExportHandler(string descriptiveName,
            Func<OBJ, ExportDelegate> exportDelegateFunction,
            Func<GROUP, string> groupNameFunction,
            Func<OBJ, string> objNameFunction,
            Func<OBJ, GROUP> parentFunction,
            Func<GROUP, IEnumerable<OBJ>> containedObjsFunction,
            Func<GROUP, IEnumerable<GROUP>> containedSubGroupsFunction,
            Predicate<ImportData<GROUP>> importPredicate)
        {
            this.descriptiveName = descriptiveName;
            this.exportDelegateFunction = exportDelegateFunction;
            this.groupNameFunction = groupNameFunction;
            this.objNameFunction = objNameFunction;
            this.parentFunction = parentFunction;
            this.containedObjsFunction = containedObjsFunction;
            this.containedSubGroupsFunction = containedSubGroupsFunction;
            this.importPredicate = importPredicate;
        }

        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<GROUP>("Importa " + descriptiveName + " dalla cartella", ImportOnlyTopFolder);
            menuRoot.Items.AddActionItem<GROUP>("Importa " + descriptiveName + " dalla cartella (Comprese sottocartelle)", ImportAllSubFolders);
            menuRoot.Items.AddActionItem<GROUP>("Esporta " + descriptiveName + " nella cartella", ExportOnlySelectedFolder);
            menuRoot.Items.AddActionItem<GROUP>("Esporta " + descriptiveName + " nella cartella (Comprese Sottocartelle)", ExportAllFolders);

            menuRoot.Items.AddActionItem<OBJ>("Esporta " + descriptiveName + " singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<OBJ>("Esporta " + descriptiveName + " selez. su cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<OBJ>("Importa " + descriptiveName + "", ImportDescrete);
            menuRoot.Items.AddActionItem<OBJ>("Importa Cartella", ImportOnlyTopFolder);
            menuRoot.Items.AddActionItem<OBJ>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }

        private void ExportOnlySelectedFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (GROUP group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, false);
            }
        }

        private void ExportAllFolders(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (GROUP group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, GROUP group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + groupNameFunction(group);
            foreach (OBJ obj in containedObjsFunction(group))
            {
                var exportOK = ExportSingle(obj, directory + "\\" + objNameFunction(obj) + ".xml");
                if (!exportOK)
                {
                    return;
                }
            }

            if (withSubfolders)
            {
                foreach (GROUP subGroup in containedSubGroupsFunction(group))
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (OBJ obj in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export " + descriptiveName + " " + objNameFunction(obj),
                    ShowHelp = true,
                    FileName = objNameFunction(obj)
                };

                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    var exportSuccessful = ExportSingle(obj, fileDialog.FileName);
                    if (!exportSuccessful)
                    {
                        return;
                    }
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (OBJ obj in selectionProvider.GetSelection())
                {
                    var exportOK = ExportSingle(obj, directory + "\\" + objNameFunction(obj) + ".xml");
                    if (!exportOK)
                    {
                        return;
                    }
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (OBJ obj in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = true
                };

                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    var group = parentFunction(obj);
                    foreach (var fileName in fileDialog.FileNames)
                    {
                        if (!ImportBlock(group, fileName))
                        {
                            return;
                        }
                    }
                }

                break;
            }
        }

        private void ImportOnlyTopFolder(MenuSelectionProvider selectionProvider)
        {
            ImportFolderGeneric(selectionProvider, false);

        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            ImportFolderGeneric(selectionProvider, true);
        }

        private void ImportFolderGeneric(MenuSelectionProvider selectionProvider, bool includeSubFolders)
        {
            GROUP group = default(GROUP);
            foreach (GROUP loopGroup in selectionProvider.GetSelection())
            {
                group = loopGroup;
                break;
            }

            if (group == null)
            {
                foreach (OBJ obj in selectionProvider.GetSelection())
                {
                    group = parentFunction(obj);
                    break;
                }
            }

            if (group != null)
            {
                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportObjectsFromFolder(group, folderDialog.SelectedPath, includeSubFolders);
                }
            }

        }

        public bool ImportObjectsFromFolder(GROUP group, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    if (!this.ImportObjectsFromFolder(group, directoryName, true))
                    {
                        return false;
                    }
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                if (!ImportBlock(group, fileName))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExportSingle(OBJ obj, string fileName)
        {
            return ExportUtil.Export(exportDelegateFunction(obj), fileName);
        }

        private bool ImportBlock(GROUP group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var importData = new ImportData<GROUP>(group,
                        new FileInfo(fileName),
                        ImportOptions.Override,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
                    return importPredicate(importData);
                }
                catch (Exception ex)
                {
                    return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                }
            }

            return false;
        }

    }
}
