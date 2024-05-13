using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using InfoBox;
using Jint;
using ClosedXML.Excel;
using Microsoft.WindowsAPICodePack.Dialogs;
using FastColoredTextBoxNS;

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
            LocalizationVariables.INIT();

            var _ = typeof(InformationBox); //InformationBox
            _ = typeof(Engine); //Jint
            _ = typeof(XLWorkbook); //ClosedXML
            _ = typeof(CommonOpenFileDialog); //WindowsAPICodePack
            _ = typeof(FastColoredTextBox);

            DropDownMenuScrollWheelHandler.Enable(true);

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainImportExportForm());
            } catch(Exception ex) {
                Utils.ShowExceptionMessage(ex);
            }

        }
    }
}
