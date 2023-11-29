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
        public const string VERSION = "V18";
        public const string DEFAULT_LANG = "it-IT";

        public const string SECTIONS_NAMESPACE = "http://www.siemens.com/automation/Openness/SW/Interface/v5";
        public const string FLG_NET_NAMESPACE = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4";

        public static string GET_SECTIONS_NAMESPACE()
        {
            return SECTIONS_NAMESPACE;
        }

        public static string GET_FLAG_NET_NAMESPACE()
        {
            return FLG_NET_NAMESPACE;
        }

        public const string GLOBAL_ID_KEY = "ID";
        public const string COMPOSITION_NAME_KEY = "CompositionName";
        public const string ATTRIBUTE_LIST_KEY = "AttributeList";
        public const string OBJECT_LIST_KEY = "ObjectList";

        public static readonly CultureInfo DEFAULT_CULTURE = CultureInfo.GetCultureInfoByIetfLanguageTag("it-IT");
    }
}
