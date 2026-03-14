using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public class SettingsFormLastOpenInformation
    {
        private static readonly Dictionary<Guid, SettingsFormLastOpenInformation> MACRO_SECTION_LAST_OPEN_INFO_DICT = []; //This might leak a bit but i don't think it will matter a lot.
        public static SettingsFormLastOpenInformation GetFromGuid(Guid guid)
        {
            return MACRO_SECTION_LAST_OPEN_INFO_DICT.GetOrAdd(guid, () => new());
        }

        public static void RemoveGuid(Guid guid)
        {
            MACRO_SECTION_LAST_OPEN_INFO_DICT.Remove(guid);
        }

        public bool CommentVisibility { get; set; } = SettingsFormConstants.DEFAULT_VALUE_DESCRIPTION_VISIBILITY;
    }
}
