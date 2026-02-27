using ClosedXML.Excel;
using FastColoredTextBoxNS;
using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities
{
    internal static class Program
    {
        //0.4.2
        //Fixed a bug where comment for alarms where generated from the hmi placeholders instead of UDT placeholders
        //Change device custom placeholder to {device_x}
        //Added ChatGPT generated icon
        public const string VERSION = "0.5.0.B02";

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LocaleVariables.INIT();

            var _ = typeof(InformationBox); //InformationBox
            _ = typeof(Engine); //Jint
            _ = typeof(XLWorkbook); //ClosedXML
            _ = typeof(CommonOpenFileDialog); //WindowsAPICodePack
            _ = typeof(FastColoredTextBox);

            DropDownMenuScrollWheelHandler.Enable(true);

            try
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    if(args.ExceptionObject is Exception exception)
                    {
                        if (exception.Source == "Jint" || exception.Source == "Acornima")
                        {
                            return;
                        }

                        Utils.ShowExceptionMessage(exception);
                    }
                };

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

        }
    }

}
