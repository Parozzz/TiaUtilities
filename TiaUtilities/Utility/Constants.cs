namespace TiaUtilities.Utility
{
    public static class Constants
    {
        public static uint VERSION { get; set; } = 17;
        public static string VERSION_STRING { get => "V" + VERSION; }

        public static readonly string SAVE_FILE_EXTENSION = "json"; //Extension for program saving file (Will still be json). DO NOT PUT A DOT!

        public const string SECTIONS_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/Interface/v5";
        public const string FLG_NET_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4";
        public const string FLG_NET_NAMESPACE_V19 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v5";

        public const string STRUCTURED_TEXT_NAMESPACE_V17 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v3";
        public const string STRUCTURED_TEXT_NAMESPACE_V19 = "http://www.siemens.com/automation/Openness/SW/NetworkSource/StructuredText/v4";

        public static string? GET_SECTIONS_NAMESPACE()
        {
            if (VERSION >= 17)
            {
                return SECTIONS_NAMESPACE_V17;
            }

            return null;
        }

        public static string? GET_FLAG_NET_NAMESPACE()
        {
            if (VERSION >= 19)
            {
                return FLG_NET_NAMESPACE_V19;
            }
            else if (VERSION >= 17)
            {
                return FLG_NET_NAMESPACE_V17;
            }

            return null;
        }

        public const string DEFAULT_EMPTY_STRUCT_NAME = "_";

        public const string HEADER_AUTHOR = "Giacomo P.";
        public const string HEADER_FAMILY = "SPIN_SRL"; //DO NOT USE SPACE
        public const string UDA_BLOCK_PROPERTIES = "GENERATED_BY_XMLREADER";
    }
}
