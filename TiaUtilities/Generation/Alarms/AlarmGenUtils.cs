using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Languages;
using TiaUtilities.SettingsStep;
using static TiaUtilities.SettingsStep.ControlFactory.SettingsFactoryGeneralOptions;

namespace TiaUtilities.Generation.Alarms
{
    public static class AlarmGenUtils
    {
        private static readonly List<SettingsSequencePanelDescriptor> GLOBAL_DESCRIPTORS = [];
        private static readonly List<SettingsSequencePanelDescriptor> TAB_DESCRIPTORS = [];
        private static readonly List<SettingsSequencePanelDescriptor> TEMPLATE_DESCRIPTORS = [];

        public static List<SettingsSequencePanelDescriptor> CreateGlobalSettingsDescriptors()
        {
            if(GLOBAL_DESCRIPTORS.Count > 0)
            {
                return GLOBAL_DESCRIPTORS;
            }

            var blocksStepDescriptor = new SettingsSequencePanelDescriptor("Blocks", "Settings for generated blocks")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC)
                    .Add(x => x.FCBlockName, Locale.GENERICS_NAME, options: new() { SupportPlaceholders = true })
                    .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .StartGroup(Locale.ALARM_SETTINGS_UDT)
                    .Add(x => x.UDTBlockName, Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_UDT_DESCR, options: new() { SupportPlaceholders = true })
                .End();
            var enablingsStepDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_ENABLE)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_ENABLE)
                    .Add(x => x.EnableCustomVariable, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR_DESCR)
                    .Add(x => x.EnableTimer, Locale.ALARM_SETTINGS_ENABLE_TIMER, Locale.ALARM_SETTINGS_ENABLE_TIMER_DESCR)
                .End();

            var segmentNamesStepDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                    .Add(x => x.OneEachSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.OneEachEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_SPARE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_DESCR, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_SPARE, options: new() { SupportPlaceholders = true })
                .End();

            var alarmStepDescriptor = new SettingsSequencePanelDescriptor("Alarm", "Settings for alarm properties")
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

            var sqlQueryDescriptor = new SettingsSequencePanelDescriptor("SQl", "SQL query for database")
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup("Query")
                    .Add(x => x.DatabaseQuery, "", options: new() { StringEditor = StringCustomEditor.TSQL, MinWidth = 1600 })
                .End();

            GLOBAL_DESCRIPTORS.Clear();
            GLOBAL_DESCRIPTORS.AddRange([enablingsStepDescriptor, blocksStepDescriptor, segmentNamesStepDescriptor, alarmStepDescriptor, sqlQueryDescriptor]);
            return GLOBAL_DESCRIPTORS;
        }
        
        public static List<SettingsSequencePanelDescriptor> CreateTabSettingsStepDescriptors()
        {
            if(TAB_DESCRIPTORS.Count > 0)
            {
                return TAB_DESCRIPTORS;
            }

            var blocksStepDescriptor = new SettingsSequencePanelDescriptor("Blocks")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_FC)
                    .Add(x => x.GroupingType, Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE, description: Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE_DESCR)
                .End();

            var variablesStepDescriptor = new SettingsSequencePanelDescriptor("Variables")
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
                    .Add(x => x.DefaultTimerType, Locale.GENERICS_TYPE, options: new() { Selections = ["TON", "TOF"] })
                    .Add(x => x.DefaultTimerValue, Locale.GENERICS_VALUE, "It must be formatted the same as in TiaPortal (eg. T#0s, T#100ms)")

                .StartGroup(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_CUSTOM_VAR)
                    .Add(x => x.DefaultCustomVarAddress, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCustomVarValue, Locale.GENERICS_VALUE)
                .End();

            var alarmsStepDescriptor = new SettingsSequencePanelDescriptor("Alarms")
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

            var hmiStepDescriptor = new SettingsSequencePanelDescriptor(Locale.GENERICS_HMI)
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.GENERICS_HMI)
                    .Add(x => x.HmiStartID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID_DESCR)
                    .Add(x => x.DefaultHmiAlarmClass, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS_DESCR, options: new() { SupportPlaceholders = true })
                .End();

            var placeholdersStepDescriptor = new SettingsSequencePanelDescriptor("Placeholders")
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS)
                    .Add(x => x.CustomPlaceholdersJSON, "Placeholders", description: Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS_DESC, options: new() { StringEditor = StringCustomEditor.JSON })

                .End();

            TAB_DESCRIPTORS.Clear();
            TAB_DESCRIPTORS.AddRange([blocksStepDescriptor, variablesStepDescriptor, alarmsStepDescriptor, hmiStepDescriptor, placeholdersStepDescriptor]);
            return TAB_DESCRIPTORS;
        }
        
        public static List<SettingsSequencePanelDescriptor> CreateTemplateSettingsStepDescriptor()
        {
            if(TEMPLATE_DESCRIPTORS.Count > 0)
            {
                return TEMPLATE_DESCRIPTORS;
            }

            var descriptor = new SettingsSequencePanelDescriptor("Alarms")
                .CreateBinder<AlarmTemplateConfiguration>()
                .Add(x => x.StandaloneAlarms, Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS, Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS_DESC)
                .End();

            TEMPLATE_DESCRIPTORS.Clear();
            TEMPLATE_DESCRIPTORS.AddRange([descriptor]);
            return TEMPLATE_DESCRIPTORS;
        }

    }
}
