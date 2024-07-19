using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public enum AlarmButtonConfigType
    {
        [Localization("ALARM_CONFIG_FC")] FC = 0,
        [Localization("ALARM_CONFIG_GENERATION")] ALARM_GEN = 1,
        [Localization("ALARM_CONFIG_DEFAULTS")] DEFAULT_VALUES = 2,
        [Localization("ALARM_CONFIG_PREFIX")] PREFIXES = 3,
        [Localization("ALARM_CONFIG_SEGMENT_NAME")] SEGMENT_NAMES = 4,
        [Localization("ALARM_CONFIG_TEXT_LIST")] TEXT_LIST = 5,
    }

}
