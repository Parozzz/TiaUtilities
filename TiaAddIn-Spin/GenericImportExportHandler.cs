using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private readonly string multipleDescriptiveName;
        private readonly Func<OBJ, ExportDelegate> exportDelegateFunction;
        private readonly Func<GROUP, string> groupNameFunction;
        private readonly Func<OBJ, string> objNameFunction;
        private readonly Func<OBJ, GROUP> parentFunction;
        private readonly Func<GROUP, IEnumerable<OBJ>> containedObjsFunction;
        private readonly Func<GROUP, IEnumerable<GROUP>> containedSubGroupsFunction;
        private readonly Predicate<ImportData<GROUP>> importPredicate;
        public GenericImportExportHandler(string multipleDescriptiveName,
            Func<OBJ, ExportDelegate> exportDelegateFunction,
            Func<GROUP, string> groupNameFunction,
            Func<OBJ, string> objNameFunction,
            Func<OBJ, GROUP> parentFunction,
            Func<GROUP, IEnumerable<OBJ>> containedObjsFunction,
            Func<GROUP, IEnumerable<GROUP>> containedSubGroupsFunction,
            Predicate<ImportData<GROUP>> importPredicate)
        {
            this.multipleDescriptiveName = multipleDescriptiveName;
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
            if (Util.IsItalian())
            {
                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} da file selezionati", ImportDescrete);
                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} dalle cartelle selezionata", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} dalla cartella selezionata (Incluse sottocartelle)", ImportAllSubFolders);
                menuRoot.Items.AddActionItem<GROUP>($"Esporta {multipleDescriptiveName} nella cartella selezionata", ExportOnlySelectedFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Esporta {multipleDescriptiveName} nella cartella selezionata (Comprese sotto-cartelle)", ExportAllFolders);

                menuRoot.Items.AddActionItem<OBJ>($"Copia tutti i nomi dei {multipleDescriptiveName} selezionati", CopyAllSelectedNamesToClipboard);
                menuRoot.Items.AddActionItem<OBJ>($"Sostituisci tutti i nomi dei {multipleDescriptiveName} selezionati", ChangeAllSelectedNames);
                menuRoot.Items.AddActionItem<OBJ>($"Esporta {multipleDescriptiveName} selez. su cartella", ExportAllToFolder);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName}", ImportDescrete);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName} da cartella", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName} da cartella (comprese sotto-cartelle)", ImportAllSubFolders);
            }
            else
            {
                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected file(s)", ImportDescrete);
                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected folder(s)", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected folder (Including sub-folders)", ImportAllSubFolders);
                menuRoot.Items.AddActionItem<GROUP>($"Export all {multipleDescriptiveName} inside selected folder", ExportOnlySelectedFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Export {multipleDescriptiveName} inside selected folder (Including sub-folders)", ExportAllFolders);

                menuRoot.Items.AddActionItem<OBJ>($"Copy the names of all selected {multipleDescriptiveName}", CopyAllSelectedNamesToClipboard);
                menuRoot.Items.AddActionItem<OBJ>($"Replace all the names of selected {multipleDescriptiveName}", ChangeAllSelectedNames);
                menuRoot.Items.AddActionItem<OBJ>($"Import {multipleDescriptiveName}", ImportDescrete);
                menuRoot.Items.AddActionItem<OBJ>($"Import all {multipleDescriptiveName} from folder", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<OBJ>($"Import all {multipleDescriptiveName} from folder (Including sub-folder)", ImportAllSubFolders);
                menuRoot.Items.AddActionItem<OBJ>($"Export all selected {multipleDescriptiveName} in folder", ExportAllToFolder);
            }

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

        private void CopyAllSelectedNamesToClipboard(MenuSelectionProvider selectionProvider)
        {
            try
            {
                var names = new List<string>();
                foreach (OBJ obj in selectionProvider.GetSelection())
                {
                    PropertyInfo propertyInfo = null;

                    var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        if (property.Name == "Name" || property.Name == "name")
                        {
                            propertyInfo = property;
                            break;
                        }
                    }

                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    var name = (string)propertyInfo.GetValue(obj);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }

                var namesText = String.Join("\n", names) + "\n";
                Clipboard.SetText(namesText, TextDataFormat.Text);
            }
            catch (Exception)
            {
            }
        }

        private void ChangeAllSelectedNames(MenuSelectionProvider selectionProvider)
        {
            string placeholder = "{actual_name}";
            try
            {
                string str = placeholder;
                var result = Util.ShowInputDialog(ref str);
                if (result == DialogResult.OK)
                {
                    foreach (OBJ obj in selectionProvider.GetSelection())
                    {
                        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            if (property.Name == "Name" || property.Name == "name")
                            {
                                var name = (string)property.GetValue(obj);
                                var newName = str.Replace("{actual_name}", name);
                                property.SetValue(obj, newName);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            var selection = selectionProvider.GetSelection();

            var selectedGroup = selection.Where(group => group is GROUP).Cast<GROUP>().SingleOrDefault();
            if (selectedGroup == null)
            {
                selectedGroup = selectionProvider.GetSelection()
                    .Where(obj => obj is OBJ)
                    .Cast<OBJ>()
                    .Select(parentFunction)
                    .First();
            }

            if (selectedGroup == null)
            {
                return;
            }

            var fileDialog = new OpenFileDialog
            {
                Filter = "xml files (*.xml)|*.xml",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true
            };

            if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                foreach (var fileName in fileDialog.FileNames)
                {
                    if (!ImportBlock(selectedGroup, fileName))
                    {
                        return;
                    }
                }
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
