using DocumentFormat.OpenXml.Drawing.Charts;
using InfoBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Utility
{
    public static class Utils
    {
        public static string DesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static ICollection<T> SingletonCollection<T>(T data)
        {
            return new System.Collections.ObjectModel.ReadOnlyCollection<T>(new T[] { data });
        }

        public static List<T> SingletonList<T>(T data)
        {
            return new List<T>(new T[] { data });
        }

        public static void ShowExceptionMessage(Exception ex, bool silent = false)
        {
            string message = "Message => " + ex.Message + "\r\nCause => " + ex.Source + "\r\nStackTrace:\r\n" + ex.StackTrace;
            string caption = "An exception occoured while executing!";
            Console.WriteLine("Exception:\r\n{0}", message);
            LogHandler.INSTANCE.AddException(message);
            if(silent)
            {
                InformationBox.Show(message, caption, icon: InformationBoxIcon.Warning, order: InformationBoxOrder.TopMost, sound: InformationBoxSound.None);
            }
        }

        public static Dictionary<string, object> CreatePublicFieldSnapshot(object obj)
        {
            Validate.NotNull(obj);

            var snapshotDict = new Dictionary<string, object>();

            var type = obj.GetType();

            var fields = type.GetFields();
            foreach (var field in fields.Where(field => field.IsPublic))
            {
                var fieldName = "FIELD_" + field.Name;
                snapshotDict.Add(fieldName, field.GetValue(obj));
            }

            var properties = type.GetProperties();
            foreach(var property in properties.Where(property => property.CanRead))
            {
                var propertyName = "PROPERTY_" + property.Name;
                snapshotDict.Add(propertyName, property.GetValue(obj));
            }

            return snapshotDict;
        }

        public static bool ComparePublicFieldSnapshot(object obj, Dictionary<string, object> snapshotDict) //TRUE IF ALL EQUALS
        {
            Validate.NotNull(obj);
            Validate.NotNull(snapshotDict);

            var type = obj.GetType();

            var fields = type.GetFields();
            foreach (var field in fields.Where(f => f.IsPublic))
            {
                var fieldName = "FIELD_" + field.Name;
                if (!snapshotDict.ContainsKey(fieldName))
                {
                    return false;
                }

                var snapshotValue = snapshotDict[fieldName];
                var value = field.GetValue(obj);
                if (AreValuesDifferent(snapshotValue, value))
                {
                    return false;
                }
            }

            var properties = type.GetProperties();
            foreach (var property in properties.Where(p => p.CanRead))
            {
                var propertyName = "PROPERTY_" + property.Name;
                if (!snapshotDict.ContainsKey(propertyName))
                {
                    return false;
                }

                var snapshotValue = snapshotDict[propertyName];
                var value = property.GetValue(obj);
                if (AreValuesDifferent(snapshotValue, value))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreStringDifferent(string strOne, string strTwo)
        {
            var oneEmpty = string.IsNullOrEmpty(strOne);
            var twoEmpty = string.IsNullOrEmpty(strTwo);
            return (oneEmpty && !twoEmpty) || (!oneEmpty && twoEmpty) || (strOne != null && strOne.Equals(strTwo));
        }

        public static bool AreValuesDifferent(object valueOne, object valueTwo)
        {
            if(valueOne is string strOne && valueOne is string strTwo)
            {
                return AreStringDifferent(strOne, strTwo);
            }

            return (valueOne == null && valueTwo != null) || (valueOne != null && valueTwo == null) || (valueOne != null && !valueOne.Equals(valueTwo));
        }

        public static bool ArePublicFieldDifferent<T>(T obj1, T obj2)
        {
            foreach (var field in typeof(T).GetFields().Where(field => field.IsPublic))
            {
                var oldValue = field.GetValue(obj1);
                var newValue = field.GetValue(obj2);
                if ((oldValue != null && newValue == null) || (oldValue == null && newValue != null) || (oldValue != null && !oldValue.Equals(newValue)))
                {
                    return true;
                }
            }

            return false;
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
            foreach (T loopEnumValue in Enum.GetValues(typeof(T)))
            {
                var str = stringFunc.Invoke(loopEnumValue);
                if (str == toMatchStr)
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
            return (uint)Math.Abs(((n % m) + m) % m);
        }

        public static string IndexSubstring(this string value, int startIndex, int endIndex)
        {
            return value.Substring(startIndex, (endIndex - startIndex) + 1);
        }

        public static bool FindNumberFromRight(string str, out int indexStart, out int indexEnd, bool numericOnly = true)
        {
            indexStart = indexEnd = -1;

            bool found = false;
            for (int x = (str.Length - 1); x >= 0; x--)
            {
                var c = str[x];
                if (!Char.IsDigit(c))
                {
                    if (found && (numericOnly || (c != '+' && c != '-' && c != '.'))) //After having found a number, i can also accept special chars!
                    {
                        indexStart = x + 1; //Since i loop from the end, the last one found is the start!
                        break;
                    }

                    continue;
                }

                if (!found)
                {
                    found = true;
                    indexEnd = x;
                }
            }

            if (found && indexStart == -1)
            {
                indexStart = 0;
            }

            return found;
        }

        public static bool SplitStringFromNumberFromRight(string str, out string beforeString, out string numString, out string afterString, bool numericOnly = true)
        {
            beforeString = numString = afterString = "";
            if (string.IsNullOrEmpty(str) || !FindNumberFromRight(str, out int indexStart, out int indexEnd, numericOnly))
            {
                return false;
            }

            beforeString = indexStart > 0 ? str.IndexSubstring(0, indexStart - 1) : "";
            afterString = indexEnd < str.Length ? str.IndexSubstring(indexEnd + 1, str.Length - 1) : "";

            if (indexStart == indexEnd)
            {
                numString = Char.ToString(str[indexStart]);
            }
            else if (indexStart == 0 && indexEnd == (str.Length - 1))
            {
                numString = str;
            }
            else
            {
                numString = str.IndexSubstring(indexStart, indexEnd);
            }

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
