using System.Globalization;

namespace TiaXmlReader.Languages
{
    public static class LocaleVariables
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
                Thread.CurrentThread.CurrentCulture = CULTURE;
            }
        }
        private static string _lang;

        public static CultureInfo CULTURE { get; private set; }

        public static void INIT()
        {
            LANG = DEFAULT_LANG;
        }
    }
}
