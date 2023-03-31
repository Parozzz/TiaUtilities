using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using System;
using System.IO;
using System.Reflection;
using SpinAddIn;

namespace AddIn_V17
{
    public class AddIn_V17 : ContextMenuAddIn
    {
        public static string TEMP_PATH = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);

        private readonly TiaPortal _tiaPortal;
        private readonly string _traceFilePath;

        private readonly PlcBlockHandler plcBlockHandler;
        private readonly PlcTagTableHandler plcTagTableHandler;
        private readonly SpinAddinMenuRegistrationService hmiScreenHandler;
        private readonly SpinAddinMenuRegistrationService hmiPopupHandler;
        private readonly PLCUDTHandler plcUDTHandler;
        private readonly PlcWatchtableHandler plcWatchtableHandler;

        public AddIn_V17(TiaPortal tiaPortal) : base("TiaAddIn-Spin")
        {
            _tiaPortal = tiaPortal;

            var assemblyName = Assembly.GetCallingAssembly().GetName();
            var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", assemblyName.Name, assemblyName.Version.ToString(), "Logs");
            var logDirectory = Directory.CreateDirectory(logDirectoryPath);
            _traceFilePath = Path.Combine(logDirectory.FullName, string.Concat(DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt"));

            plcBlockHandler = new PlcBlockHandler();
            plcTagTableHandler = new PlcTagTableHandler();
            hmiScreenHandler = new HMIScreenHandler();
            hmiPopupHandler = new HMIPopupHandler();
            plcUDTHandler = new PLCUDTHandler();
            plcWatchtableHandler = new PlcWatchtableHandler();
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot menuRoot)
        {
            plcBlockHandler.Register(menuRoot);
            plcTagTableHandler.Register(menuRoot);
            hmiScreenHandler.Register(menuRoot);
            hmiPopupHandler.Register(menuRoot);
            plcUDTHandler.Register(menuRoot);
            plcWatchtableHandler.Register(menuRoot);
        }


    }
}