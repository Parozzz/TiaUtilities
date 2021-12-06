using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
using System.IO;
using System.Reflection;
using Siemens.Engineering.SW;
using SpinAddin.Utility;

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
            menuRoot.Items.AddActionItem<PlcTagTable>("Esporta Tabella tag", ExportTagTableClick);

            menuRoot.Items.AddActionItem<PlcBlock>("Esporta Blocco selezionato", ExportBlockClick);
            menuRoot.Items.AddActionItem<PlcBlock>("Importa Blocco", ImportBlockClick);

            menuRoot.Items.AddActionItem<DataBlock>("Esporta DB selezionata", ExportDBClick);
            menuRoot.Items.AddActionItem<DataBlock>("Importa DB", ImportDBClick);
        }

        private void ExportTagTableClick(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcTagTable tagTable in selectionProvider.GetSelection())
            {
                ExportUtil.Export(tagTable.Export, Util.DesktopFolder() + "\\TagTableExport.xml");
                foreach (PlcTag tag in tagTable.Tags)
                {
                    ExportUtil.Export(tag.Export, Util.DesktopFolder() + "\\TagExport.xml");
                    break;
                }

                break;
            }
        }

        private void ExportBlockClick(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                ExportUtil.Export(block.Export, Util.DesktopFolder() + "\\FCFBExport.xml");
            }
        }

        private void ImportBlockClick(MenuSelectionProvider selectionProvider)
        {
            foreach (PlcBlock block in selectionProvider.GetSelection())
            {
                var filePath = Util.DesktopFolder() + "\\FCFBExport.xml";

                var blockGroup = (PlcBlockGroup)block.Parent;
                try
                {
                    blockGroup.Blocks.Import(new FileInfo(filePath), ImportOptions.Override, SWImportOptions.IgnoreMissingReferencedObjects | SWImportOptions.IgnoreStructuralChanges | SWImportOptions.IgnoreUnitAttributes);
                }
                catch (Exception) { }
            }
        }

        private void ExportDBClick(MenuSelectionProvider selectionProvider)
        {
            foreach (DataBlock block in selectionProvider.GetSelection())
            {
                ExportUtil.Export(block.Export, Util.DesktopFolder() + "\\DBExport.xml");
            }
        }

        private void ImportDBClick(MenuSelectionProvider selectionProvider)
        {
            foreach (DataBlock block in selectionProvider.GetSelection())
            {
                var filePath = Util.DesktopFolder() + "\\DBExport.xml";

                var blockGroup = (PlcBlockGroup)block.Parent;
                try
                {
                    blockGroup.Blocks.Import(new FileInfo(filePath), ImportOptions.Override, SWImportOptions.IgnoreMissingReferencedObjects | SWImportOptions.IgnoreStructuralChanges | SWImportOptions.IgnoreUnitAttributes);
                }
                catch (Exception) { }
            }
        }
    }
}