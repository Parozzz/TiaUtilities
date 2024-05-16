using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TiaXmlReader.Languages
{
    public static class LocalizationVariables
    {
        public const string DEFAULT_LANG = "en-US";

        public static string LANG
        {
            get => _lang;
            set
            {
                _lang = value;
                CULTURE = CultureInfo.GetCultureInfoByIetfLanguageTag(_lang);
                Thread.CurrentThread.CurrentUICulture = CULTURE;
            }
        }
        private static string _lang;

        public static CultureInfo CULTURE { get => _culture; private set => _culture = value; }
        private static CultureInfo _culture;

        public static void INIT()
        {
            LANG = DEFAULT_LANG;
        }
    }
}
