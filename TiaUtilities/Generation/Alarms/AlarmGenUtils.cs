using TiaUtilities.Configuration;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.SettingsStep;

namespace TiaUtilities.Generation.Alarms
{
    public static class AlarmGenUtils
    {

        public static List<SettingsStepDescriptor> CreateGlobalSettingsStepDescriptors()
        {
            var blocksStepDescriptor = new SettingsStepDescriptor("Blocks", "Settings for generated blocks")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC)
                    .Add(x => x.FCBlockName, Locale.GENERICS_NAME, options: new() { SupportPlaceholders = true })
                    .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .StartGroup(Locale.ALARM_SETTINGS_UDT)
                    .Add(x => x.UDTBlockName, Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_UDT_DESCR, options: new() { SupportPlaceholders = true })
                .End();
            var enablingsStepDescriptor = new SettingsStepDescriptor(Locale.ALARM_SETTINGS_ENABLE)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_ENABLE)
                    .Add(x => x.EnableCustomVariable, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR_DESCR)
                    .Add(x => x.EnableTimer, Locale.ALARM_SETTINGS_ENABLE_TIMER, Locale.ALARM_SETTINGS_ENABLE_TIMER_DESCR)
                .End();

            var segmentNamesStepDescriptor = new SettingsStepDescriptor(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                    .Add(x => x.OneEachSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.OneEachEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_SPARE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_SPARE, options: new() { SupportPlaceholders = true })
                .End();

            var alarmStepDescriptor = new SettingsStepDescriptor("Alarm", "Settings for alarm properties")
                .CreateBinder<AlarmMainConfiguration>()
                .Add(x => x.AlarmNumFormat, Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT, description: Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT_DESCR, options: new() { })
                .StartGroup("PLC")
                    .Add(x => x.AlarmNameTemplate, Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_NAME, options: new() { SupportPlaceholders = true })
                    .Add(x => x.AlarmCommentTemplate, Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_COMMENT, options: new() { SupportPlaceholders = true })
                    .Add(x => x.AlarmCommentTemplateSpare, Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_SPARE_COMMENT, options: new() { SupportPlaceholders = true })
                .StartGroup(Locale.GENERICS_HMI)
                    .Add(x => x.HmiNameTemplate, Locale.ALARM_SETTINGS_HMI_ITEM_NAME, Locale.ALARM_SETTINGS_HMI_ITEM_NAME_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTextTemplate, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTriggerTagTemplate, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTriggerTagUseWordArray, Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY, Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY_DESCR)
                .End();

            return [enablingsStepDescriptor, blocksStepDescriptor, segmentNamesStepDescriptor, alarmStepDescriptor];
        }
        
        public static List<SettingsStepDescriptor> CreateTabSettingsStepDescriptors()
        {
            var blocksStepDescriptor = new SettingsStepDescriptor("Blocks")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC)
                    .Add(x => x.GroupingType, Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE, description: Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE_DESCR)
                .End();

            var variablesStepDescriptor = new SettingsStepDescriptor("Variables")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_PREFIXES)
                    .Add(x => x.AlarmAddressPrefix, Locale.ALARM_SETTINGS_PREFIXES_ALARM, options: new() { SupportPlaceholders = true })
                    .Add(x => x.Coil1AddressPrefix, Locale.ALARM_SETTINGS_PREFIXES_COIL1, options: new() { SupportPlaceholders = true })
                    .Add(x => x.Coil2AddressPrefix, Locale.ALARM_SETTINGS_PREFIXES_COIL2, options: new() { SupportPlaceholders = true })
                    .Add(x => x.TimerAddressPrefix, Locale.ALARM_SETTINGS_PREFIXES_TIMER, options: new() { SupportPlaceholders = true })

                .StartGroup(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL1)
                    .Add(x => x.DefaultCoil1Address, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCoil1Type, Locale.GENERICS_TYPE)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL2)
                    .Add(x => x.DefaultCoil2Address, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCoil2Type, Locale.GENERICS_TYPE)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_TIMER)
                    .Add(x => x.DefaultTimerAddress, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultTimerType, Locale.GENERICS_TYPE, options: new() { StringSelections = ["TON", "TOF"] })
                    .Add(x => x.DefaultTimerValue, Locale.GENERICS_VALUE, "It must be formatted the same as in TiaPortal (eg. T#0s, T#100ms)")

                .StartGroup(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_CUSTOM_VAR)
                    .Add(x => x.DefaultCustomVarAddress, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCustomVarValue, Locale.GENERICS_VALUE)
                .End();

            var alarmsStepDescriptor = new SettingsStepDescriptor("Alarms")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_ALARM_NUMS)
                    .Add(x => x.TotalAlarmNum, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL_DESCR)
                    .Add(x => x.StartingAlarmNum, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START_DESCR)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_SPARE)
                    .Add(x => x.EmptyAlarmContactAddress, Locale.ALARM_SETTINGS_TAB_SPARE_ADDRESS, options: new() { SupportPlaceholders = true })
                    .Add(x => x.EmptyAlarmAtEnd, Locale.ALARM_SETTINGS_TAB_SPARE_EMPTY_NUM_AT_END, Locale.ALARM_SETTINGS_TAB_SPARE_EMPTY_NUM_AT_END_DESCR)
                    .Add(x => x.SkipNumberAfterGroup, Locale.ALARM_SETTINGS_TAB_SPARE_GROUP_SKIP, Locale.ALARM_SETTINGS_TAB_SPARE_GROUP_SKIP_DESCR)
                    .Add(x => x.AntiSlipNumber, Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP, Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP_DESCR)
                    .Add(x => x.GenerateEmptyAlarmAntiSlip, Locale.ALARM_SETTINGS_TAB_SPARE_ANTI_SLIP_GEN_EMPTY)
                .End();

            var hmiStepDescriptor = new SettingsStepDescriptor(Locale.GENERICS_HMI)
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.GENERICS_HMI)
                    .Add(x => x.HmiStartID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID_DESCR)
                    .Add(x => x.DefaultHmiAlarmClass, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS_DESCR, options: new() { SupportPlaceholders = true })
                .End();

            var placeholdersStepDescriptor = new SettingsStepDescriptor("Placeholders")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS)
                    .Add(x => x.CustomPlaceholdersJSON, "Placeholders", description: Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS_DESC, options: new() { StringSpecifiedEditor = SettingsStep.ControlFactory.SettingsFactoryGeneralOptions.StringCustomEditor.JSON })

                .End();

            return [blocksStepDescriptor, variablesStepDescriptor, alarmsStepDescriptor, hmiStepDescriptor, placeholdersStepDescriptor];
        }
        
        public static List<SettingsStepDescriptor> CreateTemplateSettingsStepDescriptor()
        {
            var descriptor = new SettingsStepDescriptor("Alarms")
                .CreateBinder<AlarmTemplateConfiguration>()
                .Add(x => x.StandaloneAlarms, Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS, Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS_DESC)
                .End();

            return [descriptor];
        }
        
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
                .SetHasPlaceholderSupportDotMark()
                .AddUInt(nameof(AlarmMainConfiguration.FCBlockNumber), Locale.GENERICS_NUMBER)

                .Section(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                .AddString(nameof(AlarmMainConfiguration.OneEachSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_DESCR)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.OneEachEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_SPARE)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.GroupSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_DESCR)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.GroupEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_SPARE)
                .SetHasPlaceholderSupportDotMark()

                .Section(Locale.ALARM_SETTINGS_UDT)
                .AddString(nameof(AlarmMainConfiguration.UDTBlockName), Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_UDT_DESCR)
                .SetHasPlaceholderSupportDotMark()

                .Section(Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE)
                .AddString(nameof(AlarmMainConfiguration.AlarmNameTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_NAME)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_COMMENT)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplateSpare), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_SPARE_COMMENT)
                .SetHasPlaceholderSupportDotMark()

                .Section(Locale.GENERICS_HMI)
                .AddString(nameof(AlarmMainConfiguration.HmiNameTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_NAME, Locale.ALARM_SETTINGS_HMI_ITEM_NAME_DESCR)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.HmiTextTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT_DESCR)
                .SetHasPlaceholderSupportDotMark()
                .AddString(nameof(AlarmMainConfiguration.HmiTriggerTagTemplate), Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG_DESCR)
                .SetHasPlaceholderSupportDotMark()
                .AddBool(nameof(AlarmMainConfiguration.HmiTriggerTagUseWordArray), Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY, Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY_DESCR)
                .SetHasPlaceholderSupportDotMark();
        }

        public static void AddTabConfigSettings(SettingsBindings settingsBindings,
            Func<string> nameFunc, Func<bool> isVisibileFunc,
            Func<AlarmTabConfiguration?> tabConfigFunc, Func<Dictionary<string, ObservableConfiguration>> tabDictFunc)
        {
            settingsBindings
                .MacroSection(nameFunc, isVisibileFunc, tabConfigFunc, MainForm.Settings.PresetAlarmTabConfiguration, tabDictFunc)

                .Section(Locale.ALARM_SETTINGS_PREFIXES)
                .AddString(nameof(AlarmTabConfiguration.AlarmAddressPrefix), description: Locale.ALARM_SETTINGS_PREFIXES_ALARM)
                .AddString(nameof(AlarmTabConfiguration.Coil1AddressPrefix), description: Locale.ALARM_SETTINGS_PREFIXES_COIL1)
                .AddString(nameof(AlarmTabConfiguration.Coil2AddressPrefix), description: Locale.ALARM_SETTINGS_PREFIXES_COIL2)
                .AddString(nameof(AlarmTabConfiguration.TimerAddressPrefix), description: Locale.ALARM_SETTINGS_PREFIXES_TIMER)

                .Section(Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE)
                .AddEnum(nameof(AlarmTabConfiguration.GroupingType), description: Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE_DESCR)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL1)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil1Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderSupportDotMark()
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil1Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL2)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil2Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderSupportDotMark()
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil2Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_TIMER)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderSupportDotMark()
                .AddStringList(nameof(AlarmTabConfiguration.DefaultTimerType), ["TON", "TOF"], Locale.GENERICS_TYPE)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerValue), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_CUSTOM_VAR)
                .AddString(nameof(AlarmTabConfiguration.DefaultCustomVarAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .SetHasPlaceholderSupportDotMark()
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
