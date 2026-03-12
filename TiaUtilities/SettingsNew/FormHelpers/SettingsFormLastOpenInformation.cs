using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormLastOpenInformation
    {
        private static readonly Dictionary<Guid, SettingsFormLastOpenInformation> MACRO_SECTION_LAST_OPEN_INFO_DICT = []; //This might leak a bit but i don't think it will matter a lot.
        public static SettingsFormLastOpenInformation GetLastOpenInformartionFromGuid(Guid guid)
        {
            return MACRO_SECTION_LAST_OPEN_INFO_DICT.GetOrAdd(guid, () => new());
        }

        public bool CommentVisibility { get; set; } = SettingsConstants.DEFAULT_VALUE_DESCRIPTION_VISIBILITY;
    }
}
