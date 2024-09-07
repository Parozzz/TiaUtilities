using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.Placeholders
{
    public static class GenPlaceholders
    {
        public static class Generation
        {
            public const string TAB_NAME = "{tab_name}";
        }

        public static class IO
        {
            public const string MEMORY_TYPE = "{memory_type}";
            public const string BIT = "{bit}";
            public const string BYTE = "{byte}";
            public const string VARIABLE = "{variable_name}";
            public const string IONAME = "{io_name}";
            public const string COMMENT = "{comment}";

            public const string CONFIG_DB_NAME = "{config_db_name}";
            public const string CONFIG_DB_NUMBER = "{config_db_number}";
        }

        public static class  Alarms
        {
            public const string DEVICE_NAME = "{device_name}";
            public const string DEVICE_ADDRESS = "{device_address}";
            public const string DEVICE_DESCRIPTION = "{device_description}";

            public const string ALARM_DESCRIPTION = "{alarm_description}";
            public const string ALARM_NUM = "{alarm_num}";
            public const string ALARM_NUM_START = "{alarm_num_start}";
            public const string ALARM_NUM_END = "{alarm_num_end}";
        }
    }
}
