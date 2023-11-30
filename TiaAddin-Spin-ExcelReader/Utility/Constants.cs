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
        public static uint VERSION = 17;
        public static string GET_STRING_VERSION()
        {
            return "V" + VERSION;
        }

        public static readonly CultureInfo DEFAULT_CULTURE = CultureInfo.GetCultureInfoByIetfLanguageTag("it-IT");
        public const string DEFAULT_LANG = "it-IT";

        public const string SECTIONS_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/Interface/v5";
        public const string FLG_NET_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4";

        public static string GET_SECTIONS_NAMESPACE()
        {
            if(VERSION >= 17)
            {
                return SECTIONS_NAMESPACE_V17;
            }

            return null;
        }

        public static string GET_FLAG_NET_NAMESPACE()
        {
            if (VERSION >= 17)
            {
                return FLG_NET_NAMESPACE_V17;
            }

            return null;
        }

        public const string GLOBAL_ID_KEY = "ID";
        public const string COMPOSITION_NAME_KEY = "CompositionName";
        public const string ATTRIBUTE_LIST_KEY = "AttributeList";
        public const string OBJECT_LIST_KEY = "ObjectList";
    }
}
