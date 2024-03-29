using System;
using System.Windows.Forms;

namespace SpinXmlReader
{
    public static class Utils
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
            if (obj == null)
            {
                outObj = default;
                return false;
            }

            outObj = obj;
            return true;
        }

        public static string StringFullOr(string str, string or)
        {
            return string.IsNullOrEmpty(str) ? or : str;
        }

        public static void StringFullOr(ref string str, string or)
        {
            str = string.IsNullOrEmpty(str) ? or : str;
        }

        public static T FindEnumByStringMethod<T>(string toMatchStr, Func<T, string> stringFunc) where T : Enum
        {
            foreach(T loopEnumValue in Enum.GetValues(typeof(T)))
            {
                var str = stringFunc.Invoke(loopEnumValue);
                if(str == toMatchStr)
                {
                    return loopEnumValue;
                }
            }

            return default;
        }

        public static int Mod(int n, int m)
        {
            return ((n % m) + m) % m;
        }

        public static uint UMod(uint n, uint m)
        {
            return (uint) Math.Abs( ((n % m) + m) % m );
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
