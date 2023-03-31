using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using SpinAddin.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace SpinAddIn
{
    public class PlcBlockHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcBlockGroup>("Esporta blocchi nella cartella", ExportOnlyMainFolder);
            menuRoot.Items.AddActionItem<PlcBlockGroup>("Esporta blocchi nella cartella (Comprese Sottocartelle)", ExportAllFolders);
            menuRoot.Items.AddActionItem<PlcBlock>("Esporta Blocchi Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<PlcBlock>("Esporta Blocchi selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Blocchi", ImportDescrete);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }

        private void ExportOnlyMainFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcBlockGroup group in selectionProvider.GetSelection())
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
            foreach (PlcBlockGroup group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, PlcBlockGroup group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + group.Name;
            foreach (PlcBlock block in group.Blocks)
            {
                ExportUtil.Export(block.Export, directory + "\\" + block.Name + ".xml");
            }

            if (withSubfolders)
            {
                foreach (PlcBlockGroup subGroup in group.Groups)
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export BLOCK " + block.Name,
                    ShowHelp = true,
                    FileName = block.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(block.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (PlcBlock block in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(block.Export, directory + "\\" + block.Name + ".xml");
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var group = (PlcBlockGroup)block.Parent;

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
                        ImportBlock(group, fileName);
                    }
                }

                break;
            }
        }

        private void ImportAllFolder(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var group = (PlcBlockGroup)block.Parent;

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
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var group = (PlcBlockGroup)block.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportBlocksFromFolder(group, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private void ImportBlocksFromFolder(PlcBlockGroup group, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    this.ImportBlocksFromFolder(group, directoryName, true);
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                ImportBlock(group, fileName);
            }
        }

        private void ImportBlock(PlcBlockGroup group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.Blocks.Import(fileInfo, ImportOptions.Override,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

    }
}
