using System;
using System.Windows.Forms;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.Utility
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

        public static bool TryNotNull<OBJ>(OBJ obj, out OBJ outObj)
        {
            if(obj == null)
            {
                outObj = default(OBJ);
                return false;
            }

            outObj = obj;
            return true;
        }
    }

    public static class Validate
    {
        public static void NotNull(Object obj, string exceptionMsg)
        {
            if (obj == null)
            {
                throw new NullReferenceException(exceptionMsg);
            }
        }

        public static void NotNull(Object obj)
        {
            NotNull(obj, "An object is null when shouldn't");
        }

        public static void IsTrue(bool condition, string exceptionMsg)
        {
            if (!condition)
            {
                throw new ArgumentException(exceptionMsg);
            }
        }

        public static void IsTrue(bool condition)
        {
            IsTrue(condition, "A condition is false when should be true");
        }
    }
}