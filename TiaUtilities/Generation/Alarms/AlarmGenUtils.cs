using TiaUtilities.Configuration;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.Generation.Alarms
{
    public static class AlarmGenUtils
    {
        public static void AddMainConfigBindings(SettingsBindings settingsBindings, AlarmMainConfiguration mainConfig)
        {
            settingsBindings
                .MacroSection("AlarmGenControl", true, mainConfig, MainForm.Settings.PresetAlarmMainConfiguration)

                .Section(Locale.ALARM_SETTINGS_ENABLE)
                .AddBool(nameof(AlarmMainConfiguration.EnableCustomVariable), Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR_DESCR)
                .AddBool(nameof(AlarmMainConfiguration.EnableTimer), Locale.ALARM_SETTINGS_ENABLE_TIMER, Locale.ALARM_SETTINGS_ENABLE_TIMER_DESCR)

                .Section(Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT)
                .AddString(nameof(AlarmMainConfiguration.AlarmNumFormat), description: Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT_DESCR)

                .Section(Locale.ALARM_SETTINGS_FC)
                .AddString(nameof(AlarmMainConfiguration.FCBlockName), Locale.GENERICS_NAME)
                .SetHasPlaceholderDotMark()
                .AddUInt(nameof(AlarmMainConfiguration.FCBlockNumber), Locale.GENERICS_NUMBER)

                .Section(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                .AddString(nameof(AlarmMainConfiguration.OneEachSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_DESCR)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.OneEachEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_SPARE)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.GroupSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_DESCR)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.GroupEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_SPARE)
                .SetHasPlaceholderDotMark()

                .Section(Locale.ALARM_SETTINGS_UDT)
                .AddString(nameof(AlarmMainConfiguration.UDTBlockName), Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_UDT_DESCR)
                .SetHasPlaceholderDotMark()

                .Section(Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE)
                .AddString(nameof(AlarmMainConfiguration.AlarmNameTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_NAME)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_COMMENT)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplateSpare), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_SPARE_COMMENT)
                .SetHasPlaceholderDotMark()

                .Section(Locale.GENERICS_HMI)
                .AddString(nameof(AlarmMainConfiguration.HmiNameTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_NAME, Locale.ALARM_SETTINGS_HMI_ITEM_NAME_DESCR)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.HmiTextTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT_DESCR)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmMainConfiguration.HmiTriggerTagTemplate), Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG_DESCR)
                .SetHasPlaceholderDotMark()
                .AddBool(nameof(AlarmMainConfiguration.HmiTriggerTagUseWordArray), Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY, Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY_DESCR)
                .SetHasPlaceholderDotMark();
        }

        public static void AddTabConfigSettings(SettingsBindings settingsBindings,
            Func<string> nameFunc, Func<bool> isVisibileFunc,
            Func<AlarmTabConfiguration?> tabConfigFunc, Func<Dictionary<string, ObservableConfiguration>> tabDictFunc)
        {
            settingsBindings
                .MacroSection(nameFunc, isVisibileFunc, tabConfigFunc, MainForm.Settings.PresetAlarmTabConfiguration, tabDictFunc)

                .Section(Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE)
                .AddEnum(nameof(AlarmTabConfiguration.GroupingType), description: Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE_DESCR)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL1)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil1Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderDotMark()
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil1Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL2)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil2Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderDotMark()
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil2Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_TIMER)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderDotMark()
                .AddStringList(nameof(AlarmTabConfiguration.DefaultTimerType), ["TON", "TOF"], Locale.GENERICS_TYPE)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerValue), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_CUSTOM_VAR)
                .AddString(nameof(AlarmTabConfiguration.DefaultCustomVarAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderDotMark()
                .AddString(nameof(AlarmTabConfiguration.DefaultCustomVarValue), Locale.GENERICS_VALUE)

                .Section(Locale.ALARM_SETTINGS_TAB_ALARM_NUMS)
                .AddUInt(nameof(AlarmTabConfiguration.TotalAlarmNum), Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL_DESCR)
                .AddUInt(nameof(AlarmTabConfiguration.StartingAlarmNum), Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START_DESCR)


                .Section(Locale.ALARM_SETTINGS_TAB_SPARE)
                .AddString(nameof(AlarmTabConfiguration.EmptyAlarmContactAddress), Locale.ALARM_SETTINGS_TAB_SPARE_ADDRESS)
                .AddUInt(nameof(AlarmTabConfiguration.EmptyAlarmAtEnd), Locale.ALARM_SETTINGS_TAB_SPARE_EMPTY_NUM_AT_END, Locale.ALARM_SETTINGS_TAB_SPARE_EMPTY_NUM_AT_END_DESCR)
                .AddUInt(nameof(AlarmTabConfiguration.SkipNumberAfterGroup), Locale.ALARM_SETTINGS_TAB_SPARE_GROUP_SKIP, Locale.ALARM_SETTINGS_TAB_SPARE_GROUP_SKIP_DESCR)
                .AddUInt(nameof(AlarmTabConfiguration.AntiSlipNumber), Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP, Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP_DESCR)
                .AddBool(nameof(AlarmTabConfiguration.GenerateEmptyAlarmAntiSlip), Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP_GEN_EMPTY)

                .Section(Locale.GENERICS_HMI)
                .AddUInt(nameof(AlarmTabConfiguration.HmiStartID), Locale.ALARM_SETTINGS_TAB_HMI_START_ID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID_DESCR)
                .AddString(nameof(AlarmTabConfiguration.DefaultHmiAlarmClass), Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS_DESCR)

                .Section(Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS)
                .AddJSON(nameof(AlarmTabConfiguration.CustomPlaceholdersJSON), description: Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS_DESC);
        }

        public static void AddTemplateConfigSettings(SettingsBindings settingsBindings,
            Func<string> nameFunc, Func<bool> isVisibileFunc,
            Func<AlarmTemplateConfiguration?> templateConfigFunc, Func<Dictionary<string, ObservableConfiguration>> templateDictFunc)
        {
            settingsBindings
                .MacroSection(nameFunc, isVisibileFunc, templateConfigFunc, MainForm.Settings.PresetTemplateConfiguration, templateDictFunc)

                .Section(Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS)
                .AddBool(nameof(AlarmTemplateConfiguration.StandaloneAlarms), "", Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS_DESC);
        }

    }
}
