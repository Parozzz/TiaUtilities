using FCFBConverter.Utility;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FCFBConverter
{
    public class AddIn : ContextMenuAddIn
    {
        public static string TEMP_PATH = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);

        private readonly TiaPortal _tiaPortal;
        private readonly Settings _settings;

        private readonly string _traceFilePath;

        public AddIn(TiaPortal tiaPortal) : base("TiaAddIn-Spin")
        {
            _tiaPortal = tiaPortal;
            _settings = Settings.Load();

            var assemblyName = Assembly.GetCallingAssembly().GetName();
            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", assemblyName.Name, assemblyName.Version.ToString(), "Logs");
            var logDirectory = Directory.CreateDirectory(logDirectoryPath);
            _traceFilePath = Path.Combine(logDirectory.FullName, string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt"));
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<PlcTagTable>("Controlla Tabella tag", CheckTagTableClick);
            menuRoot.Items.AddActionItem<PlcBlock>("Controlla Blocchi", CheckBlocksClick);
            
            menuRoot.Items.AddActionItem<FC>("Convert to FB", AddInClick);
            menuRoot.Items.AddActionItem<FB>("Convert to FC", AddInClick);
            menuRoot.Items.AddActionItem<IEngineeringObject>("Please select only FBs or FCs", menuSelectionProvider => { }, InfoTextStatus);

            var settingsMenu = menuRoot.Items.AddSubmenu("Settings");
            var settingsFBtoFC = settingsMenu.Items.AddSubmenu("Move static variables (FB to FC)");
            settingsFBtoFC.Items.AddActionItemWithRadioButton<IEngineeringObject>("InOut", _settings.StaticToInOut, _settings.StaticToInOutStatus);
            settingsFBtoFC.Items.AddActionItemWithRadioButton<IEngineeringObject>("Temp", _settings.StaticToTemp, _settings.StaticToTempStatus);
            settingsFBtoFC.Items.AddActionItemWithRadioButton<IEngineeringObject>("Remove static variables", _settings.RemoveStatic, _settings.RemoveStaticStatus);

            var settingsFCtoFB = settingsMenu.Items.AddSubmenu("Move return value (FC to FB)");
            settingsFCtoFB.Items.AddActionItemWithRadioButton<IEngineeringObject>("Output", _settings.ReturnToOutput, _settings.ReturnToOutputStatus);
            settingsFCtoFB.Items.AddActionItemWithRadioButton<IEngineeringObject>("Remove return value", _settings.RemoveReturn, _settings.RemoveReturnStatus);

            settingsMenu.Items.AddActionItemWithCheckBox<IEngineeringObject>("Remove corresponding instance DB(s)", _settings.RemoveIDB, _settings.RemoveIDBStatus);
            settingsMenu.Items.AddActionItemWithCheckBox<IEngineeringObject>("Open block in editor", _settings.OpenInEditor, _settings.OpenInEditorStatus);
        }

        private void CheckTagTableClick(MenuSelectionProvider selectionProvider)
        {
            try
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
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void CheckBlocksClick(MenuSelectionProvider selectionProvider)
        {
            try
            {
                foreach (PlcBlock block in selectionProvider.GetSelection())
                {
                    if (block is FC fc)
                    {
                        ExportUtil.Export(fc.Export, Util.DesktopFolder() + "\\FCExport.xml");
                    }
                    else if (block is FB fb)
                    {
                        ExportUtil.Export(fb.Export, Util.DesktopFolder() + "\\FBExport.xml");
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }


        private void AddInClick(MenuSelectionProvider menuSelectionProvider)
        {
            using (var fileStream = new FileStream(_traceFilePath, FileMode.Append))
            {
                Trace.Listeners.Add(new TextWriterTraceListener(fileStream) { TraceOutputOptions = TraceOptions.DateTime });
                Trace.AutoFlush = true;

                var blocks = new List<Block>();

                foreach (PlcBlock block in menuSelectionProvider.GetSelection())
                {
                    blocks.Add(new Block(block));
                }

                using (var exclusiveAccess = _tiaPortal.ExclusiveAccess("Converting " + blocks.Count + " blocks..."))
                {
                    var project = _tiaPortal.Projects.First();
                    using (var transaction = exclusiveAccess.Transaction(project, "Convert blocks"))
                    {
                        foreach (var block in blocks)
                        {
                            if (block.IsChangeable)
                            {
                                block.ChangeBlockType(_settings);
                            }
                        }

                        var showMessageBox = false;
                        var message = string.Empty;
                        foreach (var block in blocks)
                        {
                            if (block.ChangeSuccessful == false)
                            {
                                message +=
                                    "Block: " + block.BlockName + Environment.NewLine +
                                    "Reason: " + block.GetStateText() + Environment.NewLine +
                                    "Action: " + block.GetActionText() + Environment.NewLine + Environment.NewLine;
                                showMessageBox = true;
                            }
                        }

                        if (showMessageBox)
                        {
                            exclusiveAccess.Text = "Completed!" + Environment.NewLine + "See the message box for further information.";
                            using (var owner = Util.GetForegroundWindow())
                            {
                                MessageBox.Show(owner, "The following blocks could not be converted:" + Environment.NewLine + Environment.NewLine + message, "FC-FB Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        if (transaction.CanCommit)
                        {
                            transaction.CommitOnDispose();
                        }
                    }
                }
                Trace.Close();
            }

            try
            {
                if (new FileInfo(_traceFilePath).Length == 0)
                {
                    File.Delete(_traceFilePath);
                }
            }
            catch
            {
                // Silently ignore file operations
            }
        }

        private static MenuStatus InfoTextStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            var show = false;

            foreach (IEngineeringObject engineeringObject in menuSelectionProvider.GetSelection())
            {
                if (!(engineeringObject.GetType() == menuSelectionProvider.GetSelection().First().GetType() && (engineeringObject is FB || engineeringObject is FC)))
                {
                    show = true;
                    break;
                }
            }

            return show ? MenuStatus.Disabled : MenuStatus.Hidden;
        }
    }
}