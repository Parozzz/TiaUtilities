using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
using System.IO;
using System.Reflection;
using Siemens.Engineering.SW;
using SpinAddin.Utility;
using System.Windows.Forms;

namespace SpinAddin
{
    public class AddIn : ContextMenuAddIn
    {
        public static string TEMP_PATH = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);

        private readonly TiaPortal _tiaPortal;
        private readonly string _traceFilePath;

        public AddIn(TiaPortal tiaPortal) : base("TiaAddIn-Spin")
        {
            _tiaPortal = tiaPortal;

            var assemblyName = Assembly.GetCallingAssembly().GetName();
            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", assemblyName.Name, assemblyName.Version.ToString(), "Logs");
            var logDirectory = Directory.CreateDirectory(logDirectoryPath);
            _traceFilePath = Path.Combine(logDirectory.FullName, string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt"));
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcBlock>("Esporta Blocchi Singolarmente", ExportBlockClick);
            menuRoot.Items.AddActionItem<PlcBlock>("Esporta Blocchi selez. su Cartella", ExportBlockFolderClick);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Blocchi", ImportBlockClick);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Cartella", ImportBlockFolderClick);

            //menuRoot.Items.AddActionItem<DataBlock>("Esporta DB", ExportDBClick);
            //menuRoot.Items.AddActionItem<DataBlock>("Importa DB", ImportDBClick);

            menuRoot.Items.AddActionItem<PlcTagTable>("Esporta TagTable Singolarmente", ExportTagTableClick);
            menuRoot.Items.AddActionItem<PlcTagTable>("Esporta TagTable selez. su Cartella", ExportTagTableFolderClick);
            menuRoot.Items.AddActionItem<PlcTagTable>("Importa TagTable", ImportTagTableClick);
            menuRoot.Items.AddActionItem<PlcTagTable>("Importa Cartella", ImportTagTableFolderClick);
        }

        private void ExportBlockClick(MenuSelectionProvider selectionProvider)
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

        private void ExportBlockFolderClick(MenuSelectionProvider selectionProvider)
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


        private void ImportBlockClick(MenuSelectionProvider selectionProvider)
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

        private void ImportBlockFolderClick(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var group = (PlcBlockGroup)block.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    foreach (var fileName in Directory.GetFiles(folderDialog.SelectedPath))
                    {
                        ImportBlock(group, fileName);
                    }
                }

                break;
            }
        }

        private void ImportBlock(PlcBlockGroup group, string fileName)
        {
            if (fileName.EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.Blocks.Import(fileInfo, ImportOptions.Override,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
                }
                catch(Exception ex) 
                { 
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

        private void ExportTagTableClick(MenuSelectionProvider selectionProvider)
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

        private void ExportTagTableFolderClick(MenuSelectionProvider selectionProvider)
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

        private void ImportTagTableClick(MenuSelectionProvider selectionProvider)
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

        private void ImportTagTableFolderClick(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                var group = (PlcTagTableGroup)tagTable.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    foreach (var fileName in Directory.GetFiles(folderDialog.SelectedPath))
                    {
                        ImportTagTable(group, fileName);
                    }
                }

                break;
            }
        }

        private void ImportTagTable(PlcTagTableGroup group, string fileName)
        {
            if (fileName.EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    group.TagTables.Import(fileInfo, ImportOptions.Override);
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

    }
}