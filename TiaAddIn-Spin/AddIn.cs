using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
using System.IO;
using System.Reflection;
using Siemens.Engineering.SW;
using Siemens.Engineering.Hmi.Screen;
using SpinAddin.Utility;
using System.Windows.Forms;

namespace SpinAddIn
{
    public class AddIn : ContextMenuAddIn
    {
        public static string TEMP_PATH = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);

        private readonly TiaPortal _tiaPortal;
        private readonly string _traceFilePath;

        private readonly SpinAddinMenuRegistrationService plcBlockHandler;
        private readonly SpinAddinMenuRegistrationService plcTagTableHandler;
        private readonly SpinAddinMenuRegistrationService hmiScreenHandler;
        private readonly SpinAddinMenuRegistrationService hmiPopupHandler;
        private readonly SpinAddinMenuRegistrationService plcUDTHandler;

        public AddIn(TiaPortal tiaPortal) : base("TiaAddIn-Spin")
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
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot menuRoot)
        {
            plcBlockHandler.Register(menuRoot);
            plcTagTableHandler.Register(menuRoot);
            hmiScreenHandler.Register(menuRoot);
            hmiPopupHandler.Register(menuRoot);
            plcUDTHandler.Register(menuRoot);
        }


    }
}