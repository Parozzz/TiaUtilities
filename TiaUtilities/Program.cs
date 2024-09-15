using ClosedXML.Excel;
using FastColoredTextBoxNS;
using InfoBox;
using Jint;
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaXmlReader
{
    internal static class Program
    {
        public const string VERSION = "0.3";

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
                AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
                {
                    if (args.Exception.Source == "Jint" || args.Exception.Source == "Acornima")
                    {
                        return;
                    }

                    Utils.ShowExceptionMessage(args.Exception);
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
