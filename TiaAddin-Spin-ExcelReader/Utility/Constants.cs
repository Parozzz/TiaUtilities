using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpinXmlReader
{
    public static class Constants
    {
        public const string ATTRIBUTE_LIST_NAME = "AttributeList";
        public const string OBJECT_LIST_NAME = "ObjectList";

        public static readonly CultureInfo DEFAULT_CULTURE = CultureInfo.GetCultureInfoByIetfLanguageTag("it-IT");
    }
}
