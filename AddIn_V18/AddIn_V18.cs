using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.WatchAndForceTables;
using SpinAddin.Utility;
using SpinAddIn;
using System;
using System.IO;
using System.Reflection;

namespace AddIn_V18
{
    public class AddIn_V18 : ContextMenuAddIn
    {
        public static string TEMP_PATH = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);

        private readonly TiaPortal _tiaPortal;
        private readonly string _traceFilePath;

        private readonly SpinAddinMenuRegistrationService plcBlockHandler;
        private readonly SpinAddinMenuRegistrationService plcTagTableHandler;
        private readonly SpinAddinMenuRegistrationService plcUDTHandler;
        private readonly SpinAddinMenuRegistrationService plcWatchtableHandler;
        private readonly SpinAddinMenuRegistrationService plcSoftwareHandler;
        private readonly SpinAddinMenuRegistrationService hmiScreenHandler;
        private readonly SpinAddinMenuRegistrationService hmiVariableHandler;
        //private readonly SpinAddinMenuRegistrationService hmiPopupHandler;

        public AddIn_V18(TiaPortal tiaPortal) : base("TiaAddIn-Spin")
        {
            _tiaPortal = tiaPortal;

            var assemblyName = Assembly.GetCallingAssembly().GetName();
            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", assemblyName.Name, assemblyName.Version.ToString(), "Logs");
            var logDirectory = Directory.CreateDirectory(logDirectoryPath);
            _traceFilePath = Path.Combine(logDirectory.FullName, string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt"));

            var plcBlockHandler = new GenericImportExportHandler<PlcBlock, PlcBlockGroup>("Blocchi",
                plcBlock => plcBlock.Export,
                group => group.Name,
                plcBlock => plcBlock.Name,
                plcBlock => (PlcBlockGroup)plcBlock.Parent,
                group => group.Blocks,
                group => group.Groups,
                importData =>
                {
                    try
                    {
                        importData.group.Blocks.Import(importData.fileInfo, importData.importOptions, importData.swImportOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.plcBlockHandler = plcBlockHandler;

            var plcTagTableHandler = new GenericImportExportHandler<PlcTagTable, PlcTagTableGroup>("Tags",
                plcTagTable => plcTagTable.Export,
                group => group.Name,
                plcTagTable => plcTagTable.Name,
                plcTagTable => (PlcTagTableGroup)plcTagTable.Parent,
                group => group.TagTables,
                group => group.Groups,
                importData =>
                {
                    try
                    {
                        importData.group.TagTables.Import(importData.fileInfo, importData.importOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.plcTagTableHandler = plcTagTableHandler;

            var plcUDTHandler = new GenericImportExportHandler<PlcType, PlcTypeGroup>("UDT",
                plcType => plcType.Export,
                group => group.Name,
                plcType => plcType.Name,
                plcType => (PlcTypeGroup)plcType.Parent,
                group => group.Types,
                group => group.Groups,
                importData =>
                {
                    try
                    {
                        importData.group.Types.Import(importData.fileInfo, importData.importOptions, importData.swImportOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.plcUDTHandler = plcUDTHandler;

            var plcWatchtableHandler = new GenericImportExportHandler<PlcWatchTable, PlcWatchAndForceTableGroup>("VAT",
                plcWatchTable => plcWatchTable.Export,
                group => group.Name,
                plcWatchTable => plcWatchTable.Name,
                plcWatchTable => (PlcWatchAndForceTableGroup)plcWatchTable.Parent,
                group => group.WatchTables,
                group => group.Groups,
                importData =>
                {
                    try
                    {
                        importData.group.WatchTables.Import(importData.fileInfo, importData.importOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.plcWatchtableHandler = plcWatchtableHandler;

            var hmiScreenHandler = new GenericImportExportHandler<Screen, ScreenFolder>(descriptiveName: "HMI Screen",
                exportDelegateFunction: plcWatchTable => plcWatchTable.Export,
                groupNameFunction: group => group.Name,
                objNameFunction: plcWatchTable => plcWatchTable.Name,
                parentFunction: plcWatchTable => (ScreenFolder)plcWatchTable.Parent,
                containedObjsFunction: group => group.Screens,
                containedSubGroupsFunction: group => group.Folders,
                importPredicate: importData =>
                {
                    try
                    {
                        importData.group.Screens.Import(importData.fileInfo, importData.importOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.hmiScreenHandler = hmiScreenHandler;

            var hmiVariableHandler = new GenericImportExportHandler<Siemens.Engineering.Hmi.Tag.TagTable, Siemens.Engineering.Hmi.Tag.TagFolder>(descriptiveName: "HMI Variable",
                exportDelegateFunction: plcWatchTable => plcWatchTable.Export,
                groupNameFunction: group => group.Name,
                objNameFunction: plcWatchTable => plcWatchTable.Name,
                parentFunction: plcWatchTable => (Siemens.Engineering.Hmi.Tag.TagFolder)plcWatchTable.Parent,
                containedObjsFunction: group => group.TagTables,
                containedSubGroupsFunction: group => group.Folders,
                importPredicate: importData =>
                {
                    try
                    {
                        importData.group.TagTables.Import(importData.fileInfo, importData.importOptions);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                    }
                });
            this.hmiVariableHandler = hmiVariableHandler;
            //hmiPopupHandler = new HMIPopupHandler();
        }

        //DO NOT REMOVE. OVERRIDE INTERNAL METHOD.
        protected override void BuildContextMenuItems(ContextMenuAddInRoot menuRoot)
        {
            plcBlockHandler.Register(menuRoot);
            plcTagTableHandler.Register(menuRoot);
            hmiScreenHandler.Register(menuRoot);
            hmiVariableHandler.Register(menuRoot);
            //hmiPopupHandler.Register(menuRoot);
            plcUDTHandler.Register(menuRoot);
            plcWatchtableHandler.Register(menuRoot);
            plcSoftwareHandler.Register(menuRoot);
        }

    }
}