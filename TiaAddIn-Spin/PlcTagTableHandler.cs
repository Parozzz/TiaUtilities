using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Tags;
using SpinAddin.Utility;
using System;
using System.IO;
using System.Windows.Forms;


namespace SpinAddIn
{
    public class PlcTagTableHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcTagTableGroup>("Esporta TagTables nella cartella", ExportOnlyMainFolder);
            menuRoot.Items.AddActionItem<PlcTagTableGroup>("Esporta TagTables nella cartella (Comprese Sottocartelle)", ExportAllFolders);
            menuRoot.Items.AddActionItem<PlcTagTable>("Esporta TagTable Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<PlcTagTable>("Esporta TagTable selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<PlcTagTable>("Importa TagTable", ImportDescrete);
            menuRoot.Items.AddActionItem<PlcTagTable>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<PlcTagTable>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }
        private void ExportOnlyMainFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcTagTableGroup group in selectionProvider.GetSelection())
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
            foreach (PlcTagTableGroup group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, PlcTagTableGroup group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + group.Name;
            foreach (PlcTagTable tagTable in group.TagTables)
            {
                ExportUtil.Export(tagTable.Export, directory + "\\" + tagTable.Name + ".xml");
            }

            if (withSubfolders)
            {
                foreach (PlcTagTableGroup subGroup in group.Groups)
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export tagtable " + tagTable.Name,
                    ShowHelp = true,
                    FileName = tagTable.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(tagTable.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(tagTable.Export, directory + "\\" + tagTable.Name + ".xml");
                }
            }
        }

        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                var group = (PlcTagTableGroup)tagTable.Parent;

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
                        ImportTagTable(group, fileName);
                    }
                }
            }
        }

        private void ImportAllFolder(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                var group = (PlcTagTableGroup)tagTable.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportTagTableFromFolder(group, folderDialog.SelectedPath, false);
                }

                break;
            }
        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                var group = (PlcTagTableGroup)tagTable.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportTagTableFromFolder(group, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private bool ImportTagTableFromFolder(PlcTagTableGroup group, string folderName, bool searchSubFolders)
        {
            if(searchSubFolders)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    if(!this.ImportTagTableFromFolder(group, directoryName, true))
                    {
                        return false;
                    }
                }
            }


            foreach (var fileName in Directory.GetFiles(folderName))
            {
                if(!ImportTagTable(group, fileName))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ImportTagTable(PlcTagTableGroup group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.TagTables.Import(fileInfo, ImportOptions.Override);
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
