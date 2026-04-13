namespace TiaUtilities.Generation.Placeholders
{
    public static class GenPlaceholders
    {
        public static class Generation
        {
            public const string TAB_NAME = "{tab_name}";
        }

        public static class IO
        {
            public static readonly List<string> PLACEHOLDER_LIST = [
                Generation.TAB_NAME,
                MEMORY_TYPE,
                BIT,
                BYTE,
                VARIABLE,
                IONAME,
                COMMENT,
                CONFIG_DB_NAME,
                CONFIG_DB_NUMBER
            ];

            public const string MEMORY_TYPE = "{memory_type}";
            public const string BIT = "{bit}";
            public const string BYTE = "{byte}";
            public const string VARIABLE = "{variable_name}";
            public const string IONAME = "{io_name}";
            public const string COMMENT = "{comment}";

            public const string CONFIG_DB_NAME = "{config_db_name}";
            public const string CONFIG_DB_NUMBER = "{config_db_number}";
        }

        public static class Alarms
        {
            public static readonly List<string> PLACEHOLDER_LIST = [
                Generation.TAB_NAME,
                DEVICE_NAME,
                DEVICE_ADDRESS,
                DEVICE_DESCRIPTION,
                DEVICE_TEMPLATE,
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "1"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "2"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "3"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "4"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "5"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "6"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "7"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "8"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "9"),
                DEVICE_PLACEHOLDERS_GENERIC.Replace("x", "10"),
                //DEVICE_PLACEHOLDERS_GENERIC_SPLITTER, Not a placeholder
                ALARM_DESCRIPTION,
                ALARM_HMI_TEXT,
                ALARM_NUM,
                ALARM_NUM_START,
                ALARM_NUM_END,
                //HMI_PARAMETER Not a placeholder
            ];

            public const string DEVICE_NAME = "{device_name}";
            public const string DEVICE_ADDRESS = "{device_address}";
            public const string DEVICE_DESCRIPTION = "{device_description}";
            public const string DEVICE_TEMPLATE = "{device_template}";
            public const string DEVICE_PLACEHOLDERS_GENERIC = "{device_x}";
            public const string DEVICE_PLACEHOLDERS_GENERIC_SPLITTER = "<>"; //Non a placeholder

            public const string ALARM_DESCRIPTION = "{alarm_description}";
            public const string ALARM_HMI_TEXT = "{alarm_hmi_text}";
            public const string ALARM_NUM = "{alarm_num}";
            public const string ALARM_NUM_START = "{alarm_num_start}";
            public const string ALARM_NUM_END = "{alarm_num_end}";

            public const string HMI_PARAMETER = "<hmi_parameter_x>"; //Non a placeholder
        }
    }
}
