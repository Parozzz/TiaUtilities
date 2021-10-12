using System;
using System.Windows.Forms;

namespace SpinAddin.Utility
{
    public static class Util
    {
        public static string DesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static void ShowExceptionMessage(Exception ex)
        {
            string message = "Message: " + ex.Message + " \r\n StackTrace: " + ex.StackTrace;
            string caption = "An exception occoured while executing Spin Addin!";

            MessageBox.Show(message, caption);
        }
    }
}