using System;
using System.Windows.Forms;
using TiaXmlReader.Utility;
using InfoBox;
using Jint;
using ClosedXML.Excel;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TiaXmlReader
{
    internal static class Program
    {
        public const string VERSION = "0.1";

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainImportExportForm());
        }
    }
}
