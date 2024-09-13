using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using InfoBox;
using Jint;
using ClosedXML.Excel;
using Microsoft.WindowsAPICodePack.Dialogs;
using FastColoredTextBoxNS;
using TiaXmlReader.Languages;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace TiaXmlReader
{
    internal static class Program
    {
        public const string VERSION = "0.2";

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
                Application.Run(new MainImportExportForm());
            } catch(Exception ex) {
                Utils.ShowExceptionMessage(ex);
            }

        }
    }

}
