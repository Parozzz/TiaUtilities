using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using SpinAddin.Utility;
using System;
using System.IO;
using System.Windows.Forms;
using Siemens.Engineering.SW.Types;

namespace SpinAddIn
{
    public class PLCUDTHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcTypeGroup>("Esporta UDT nella cartella", ExportOnlyMainFolder);
            menuRoot.Items.AddActionItem<PlcTypeGroup>("Esporta UDT nella cartella (Comprese Sottocartelle)", ExportAllFolders);
            menuRoot.Items.AddActionItem<PlcType>("Esporta UDT Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<PlcType>("Esporta UDT selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<PlcType>("Importa UDT", ImportDescrete);
            menuRoot.Items.AddActionItem<PlcType>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<PlcType>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }
        private void ExportOnlyMainFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcTypeGroup group in selectionProvider.GetSelection())
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
            foreach (PlcTypeGroup group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, PlcTypeGroup group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + group.Name;
            foreach (PlcType type in group.Types)
            {
                ExportUtil.Export(type.Export, directory + "\\" + type.Name + ".xml");
            }

            if (withSubfolders)
            {
                foreach (PlcTypeGroup subGroup in group.Groups)
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcType type in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export UDT " + type.Name,
                    ShowHelp = true,
                    FileName = type.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(type.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (PlcType type in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(type.Export, directory + "\\" + type.Name + ".xml");
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcType type in selectionProvider.GetSelection())
            {
                var group = (PlcTypeGroup) type.Parent;

                var fileDialog = new OpenFileDialog
                {
                    Filter = "xml files (*.xml)|*.xml",
                    Title = "Import UDT " + type.Name,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = true
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    foreach (var fileName in fileDialog.FileNames)
                    {
                        ImportBlock(group, fileName);
                    }
                }

                break;
            }
        }

        private void ImportAllFolder(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcType type in selectionProvider.GetSelection())
            {
                var group = (PlcTypeGroup) type.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportBlocksFromFolder(group, folderDialog.SelectedPath, false);
                }

                break;
            }
        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcType type in selectionProvider.GetSelection())
            {
                var group = (PlcTypeGroup) type.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportBlocksFromFolder(group, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private bool ImportBlocksFromFolder(PlcTypeGroup group, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    if(!this.ImportBlocksFromFolder(group, directoryName, true))
                    {
                        return false;
                    }
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                if(!ImportBlock(group, fileName))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ImportBlock(PlcTypeGroup group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.Types.Import(fileInfo, ImportOptions.Override,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
                    return true;
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }

            return false;
        }

    }
}
