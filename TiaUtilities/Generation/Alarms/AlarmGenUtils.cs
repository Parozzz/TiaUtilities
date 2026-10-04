using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Languages;
using TiaUtilities.Settings;
using static TiaUtilities.Settings.ControlFactory.SettingsFactoryGeneralOptions;

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
            var gridsStepDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_GLOBAL_GRIDS, Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TOOLTIP)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TEMPLATE)
                    .Add(x => x.EnableCustomVariable, Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TEMPLATE_CUSTOM_VAR, Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TEMPLATE_CUSTOM_VAR_TOOLTIP)
                    .Add(x => x.EnableTimer, Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TEMPLATE_TIMER, Locale.ALARM_SETTINGS_GLOBAL_GRIDS_TEMPLATE_TIMER_TOOLTIP)
                .End();

            var blocksStepDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_TOOLTIP)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC)
                    .Add(x => x.FCBlockName, Locale.GENERICS_NAME, options: new() { SupportPlaceholders = true })
                    .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER)
                .StartGroup(Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES)
                    .Add(x => x.OneEachSegmentName, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_ONE_EACH, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_ONE_EACH_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.OneEachEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_ONE_EACH_SPARE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupSegmentName, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_GROUP_EACH, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_GROUP_EACH_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.GroupEmptyAlarmSegmentName, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_FC_SEGMENT_NAMES_GROUP_EACH_SPARE, options: new() { SupportPlaceholders = true })
                .StartGroup(Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_UDT)
                    .Add(x => x.UDTBlockName, Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_GLOBAL_PRG_BLOCKS_UDT_TOOLTIP, options: new() { SupportPlaceholders = true })
                .End();

            var alarmStepDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_GLOBAL_ALARM, Locale.ALARM_SETTINGS_GLOBAL_ALARM_TOOLTIP)
                .CreateBinder<AlarmMainConfiguration>()
                .Add(x => x.AlarmNumFormat, Locale.ALARM_SETTINGS_GLOBAL_ALARM_FORMAT, description: Locale.ALARM_SETTINGS_GLOBAL_ALARM_FORMAT_TOOLTIP, options: new() { })
                .StartGroup(Locale.GENERICS_PLC)
                    .Add(x => x.AlarmNameTemplate, Locale.ALARM_SETTINGS_GLOBAL_PLC_ALARM_VARIABLE_NAME, options: new() { SupportPlaceholders = true })
                    .Add(x => x.AlarmCommentTemplate, Locale.ALARM_SETTINGS_GLOBAL_PLC_ALARM_VARIABLE_COMMENT, options: new() { SupportPlaceholders = true })
                    .Add(x => x.AlarmCommentTemplateSpare, Locale.ALARM_SETTINGS_GLOBAL_PLC_ALARM_VARIABLE_SPARE_COMMENT, options: new() { SupportPlaceholders = true })
                .StartGroup(Locale.GENERICS_HMI)
                    .Add(x => x.HmiNameTemplate, Locale.ALARM_SETTINGS_GLOBAL_HMI_ITEM_NAME, Locale.ALARM_SETTINGS_GLOBAL_HMI_ITEM_NAME_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTextTemplate, Locale.ALARM_SETTINGS_GLOBAL_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_GLOBAL_HMI_ITEM_TEXT_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTriggerTagTemplate, Locale.ALARM_SETTINGS_GLOBAL_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_GLOBAL_HMI_TRIGGER_TAG_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.HmiTriggerTagUseWordArray, Locale.ALARM_SETTINGS_GLOBAL_HMI_USE_WORD_ARRAY, Locale.ALARM_SETTINGS_GLOBAL_HMI_USE_WORD_ARRAY_TOOLTIP)
                .End();

            var sqlQueryDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_GLOBAL_TSQL, Locale.ALARM_SETTINGS_GLOBAL_TSQL_TOOLTIP)
                .CreateBinder<AlarmMainConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_GLOBAL_TSQL_QUERY)
                    .Add(x => x.DatabaseQuery, "", options: new() { StringEditor = StringCustomEditor.TSQL, MinWidth = 1600 })
                .End();

            GLOBAL_DESCRIPTORS.Clear();
            GLOBAL_DESCRIPTORS.AddRange([gridsStepDescriptor, blocksStepDescriptor, alarmStepDescriptor, sqlQueryDescriptor]);
            return GLOBAL_DESCRIPTORS;
        }
        
        public static List<SettingsSequencePanelDescriptor> CreateTabSettingsStepDescriptors()
        {
            if(TAB_DESCRIPTORS.Count > 0)
            {
                return TAB_DESCRIPTORS;
            }

            var alarmsDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_TAB_ALARM)
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_ALARM_NUMS)
                    .Add(x => x.TotalAlarmNum, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_TOTAL_DESCR)
                    .Add(x => x.StartingAlarmNum, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START, Locale.ALARM_SETTINGS_TAB_ALARM_NUMS_START_DESCR)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_ALARM_SPARE)
                    .Add(x => x.EmptyAlarmContactAddress, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_ADDRESS, options: new() { SupportPlaceholders = true })
                    .Add(x => x.EmptyAlarmAtEnd, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_EMPTY_NUM_AT_END, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_EMPTY_NUM_AT_END_TOOLTIP)
                    .Add(x => x.SkipNumberAfterGroup, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_GROUP_SKIP, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_GROUP_SKIP_TOOLTIP)
                    .Add(x => x.AntiSlipNumber, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_ANTI_SLIP, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_ANTI_SLIP_DESCR)
                    .Add(x => x.GenerateEmptyAlarmAntiSlip, Locale.ALARM_SETTINGS_TAB_ALARM_SPARE_ANTI_SLIP_GEN_EMPTY)
                .End();

            var plcDescriptor = new SettingsSequencePanelDescriptor(Locale.GENERICS_PLC, Locale.ALARM_SETTINGS_GLOBAL_PLC_TOOLTIP)
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_FC)
                    .Add(x => x.GroupingType, Locale.ALARM_SETTINGS_TAB_PLC_GROUPING_TYPE, description: Locale.ALARM_SETTINGS_TAB_PLC_GROUPING_TYPE_DESCR)
                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES)
                    .Add(x => x.AlarmAddressPrefix, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_ALARM, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.Coil1AddressPrefix, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_COIL1, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.Coil2AddressPrefix, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_COIL2, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_TOOLTIP, options: new() { SupportPlaceholders = true })
                    .Add(x => x.TimerAddressPrefix, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_TIMER, Locale.ALARM_SETTINGS_TAB_PLC_PREFIXES_TOOLTIP, options: new() { SupportPlaceholders = true })

                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_DEFAULTS_COIL1)
                    .Add(x => x.DefaultCoil1Address, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCoil1Type, Locale.GENERICS_TYPE)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_DEFAULTS_COIL2)
                    .Add(x => x.DefaultCoil2Address, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCoil2Type, Locale.GENERICS_TYPE)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_DEFAULTS_TIMER)
                    .Add(x => x.DefaultTimerAddress, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultTimerType, Locale.GENERICS_TYPE, options: new() { Selections = ["TON", "TOF"] })
                    .Add(x => x.DefaultTimerValue, Locale.GENERICS_VALUE, Locale.ALARM_SETTINGS_TAB_PLC_DEFAULTS_TIMER_VALUE_TOOLTIP)

                .StartGroup(Locale.ALARM_SETTINGS_TAB_PLC_DEFAULTS_CUSTOM_VAR)
                    .Add(x => x.DefaultCustomVarAddress, Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE, options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultCustomVarValue, Locale.GENERICS_VALUE)
                .End();

            var hmiDescriptor = new SettingsSequencePanelDescriptor(Locale.GENERICS_HMI)
                .CreateBinder<AlarmTabConfiguration>()
                .StartGroup(Locale.GENERICS_HMI)
                    .Add(x => x.HmiStartID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID, Locale.ALARM_SETTINGS_TAB_HMI_START_ID_DESCR)
                    .Add(x => x.DefaultHmiAlarmClass, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS, Locale.ALARM_SETTINGS_TAB_HMI_DEFAULT_ALARM_CLASS_DESCR, options: new() { SupportPlaceholders = true })
                .End();

            var placeholdersDescriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS)
                .CreateBinder<AlarmTabConfiguration>()
                .Add(x => x.CustomPlaceholdersJSON, string.Empty, description: Locale.ALARM_SETTINGS_TAB_PLACEHOLDERS_DESC, options: new() { StringEditor = StringCustomEditor.JSON })

                .End();

            TAB_DESCRIPTORS.Clear();
            TAB_DESCRIPTORS.AddRange([alarmsDescriptor, plcDescriptor, hmiDescriptor, placeholdersDescriptor]);
            return TAB_DESCRIPTORS;
        }
        
        public static List<SettingsSequencePanelDescriptor> CreateTemplateSettingsStepDescriptor()
        {
            if(TEMPLATE_DESCRIPTORS.Count > 0)
            {
                return TEMPLATE_DESCRIPTORS;
            }

            var descriptor = new SettingsSequencePanelDescriptor(Locale.ALARM_SETTINGS_TEMPLATE_ALARMS)
                .CreateBinder<AlarmTemplateConfiguration>()
                .Add(x => x.StandaloneAlarms, Locale.ALARM_SETTINGS_TEMPLATE_ALARMS_STANDALONE, Locale.ALARM_SETTINGS_TEMPLATE_ALARMS_STANDALONE_TOOLTIP)
                .End();

            TEMPLATE_DESCRIPTORS.Clear();
            TEMPLATE_DESCRIPTORS.AddRange([descriptor]);
            return TEMPLATE_DESCRIPTORS;
        }

    }
}
