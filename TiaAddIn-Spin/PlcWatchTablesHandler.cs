using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.WatchAndForceTables;
using SpinAddin.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace SpinAddIn
{
    public class PlcWatchtableHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcWatchAndForceTableGroup>("Esporta tabelle nella cartella", ExportOnlyMainFolder);
            menuRoot.Items.AddActionItem<PlcWatchAndForceTableGroup>("Esporta tabelle nella cartella (Comprese Sottocartelle)", ExportAllFolders);
            menuRoot.Items.AddActionItem<PlcWatchTable>("Esporta tabelle Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<PlcWatchTable>("Esporta tabelle selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<PlcWatchTable>("Importa Blocchi", ImportDescrete);
            menuRoot.Items.AddActionItem<PlcWatchTable>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<PlcWatchTable>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }

        private void ExportOnlyMainFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcWatchAndForceTableGroup group in selectionProvider.GetSelection())
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
            foreach (PlcWatchAndForceTableGroup group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, PlcWatchAndForceTableGroup group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + group.Name;
            foreach (PlcWatchTable block in group.WatchTables)
            {
                ExportUtil.Export(block.Export, directory + "\\" + block.Name + ".xml");
            }

            if (withSubfolders)
            {
                foreach (PlcWatchAndForceTableGroup subGroup in group.Groups)
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcWatchTable watchTable in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export BLOCK " + watchTable.Name,
                    ShowHelp = true,
                    FileName = watchTable.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(watchTable.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (PlcWatchTable watchTable in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(watchTable.Export, directory + "\\" + watchTable.Name + ".xml");
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcWatchTable watchTable in selectionProvider.GetSelection())
            {
                var group = (PlcWatchAndForceTableGroup)watchTable.Parent;

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
            foreach (PlcWatchTable watchTable in selectionProvider.GetSelection())
            {
                var group = (PlcWatchAndForceTableGroup)watchTable.Parent;

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
            foreach (PlcWatchTable watchTable in selectionProvider.GetSelection())
            {
                var group = (PlcWatchAndForceTableGroup)watchTable.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportBlocksFromFolder(group, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private void ImportBlocksFromFolder(PlcWatchAndForceTableGroup group, string folderName, bool searchSubDirectories)
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

        private void ImportBlock(PlcWatchAndForceTableGroup group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.WatchTables.Import(fileInfo, ImportOptions.Override);
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

    }
}
