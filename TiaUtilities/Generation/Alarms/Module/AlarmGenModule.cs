using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.SettingsNew.Bindings;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        private readonly GridBindContainer gridBindContainer;
        private GridScriptHandler GridScriptHandler { get => this.gridBindContainer.GridScriptHandler; }

        private readonly AlarmGenControl control;
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenTemplateHandler templateHandler;

        private readonly List<AlarmGenTab> genTabList;
        public IEnumerable<AlarmTabConfiguration> TabConfigurations { get => this.genTabList.Select(tab => tab.TabConfig); }

        public SettingsBindings SettingsBindings { get; init; }

        public AlarmGenModule(ErrorReportThread errorThread)
        {
            this.gridBindContainer = new(errorThread);

            this.control = new();
            this.mainConfig = new();
            this.templateHandler = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmMainConfiguration, this.mainConfig);

            this.genTabList = [];
            this.SettingsBindings = new();
        }

        public void Init(GenModuleForm form)
        {
            this.gridBindContainer.Init(form);
            this.templateHandler.Init([]);

            this.control.tabControl.TabPreAdded += (sender, args) => TabCreation(args.TabPage);
            this.control.tabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab tab)
                {
                    genTabList.Remove(tab);
                }
            };

            this.control.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
                {
                    tab.Selected();
                }
            };

            form.Shown += (sender, args) =>
            {
                if (this.control.tabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    TabPage tabPage = new();
                    TabCreation(tabPage);
                    this.control.tabControl.TabPages.Add(tabPage);
                }
            };

            this.AddConfigurationBindings(this.SettingsBindings);
        }

        private void TabCreation(TabPage tabPage, AlarmGenTabSave? save = null)
        {
            tabPage.Text = save?.Name ?? "AlarmGen";

            AlarmGenTab tab = new(this.gridBindContainer, this, this.mainConfig, this.templateHandler, tabPage);

            tab.Init();
            if (save != null)
            {
                tab.LoadSave(save);
            }
            tabPage.Tag = tab;
            tabPage.Controls.Add(tab.TabControl);

            genTabList.Add(tab); //Do this AFTER. Otherwise the Selected event is called with Tag null.
        }

        public void Clear()
        {
            this.SettingsBindings.Clear();

            this.genTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => this.mainConfig.IsDirty() || this.genTabList.Any(x => x.IsDirty()) || this.GridScriptHandler.IsDirty() || this.templateHandler.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var tab in this.genTabList)
            {
                tab.Wash();
            }
            this.GridScriptHandler.Wash();
            this.templateHandler.IsDirty();
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSave()
            {
                ScriptSave = this.GridScriptHandler.CreateSave(),
                TemplateSaves = this.templateHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, projectSave.AlarmMainConfig);

            foreach (var tab in genTabList)
            {
                var tabSave = tab.CreateSave();
                projectSave.TabSaves.Add(tabSave);
            }

            return projectSave;
        }

        public void LoadSave(object saveObject)
        {
            if (saveObject is not AlarmGenSave loadedSave)
            {
                return;
            }

            this.Clear();

            this.GridScriptHandler.LoadSave(loadedSave.ScriptSave);
            this.templateHandler.LoadSave(loadedSave.TemplateSaves);
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, mainConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new();
                TabCreation(tabPage, tabSave);
                this.control.tabControl.TabPages.Add(tabPage);
            }

            this.AddConfigurationBindings(this.SettingsBindings);

            //Seems that the Selected event is not called in this case. Doing it manually.
            if (this.control.tabControl.SelectedTab?.Tag is AlarmGenTab tab)
            {
                tab.Selected();
            }
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new AlarmXmlGenerator(mainConfig);
            ioXmlGenerator.Init();
            foreach (var tab in genTabList)
            {
                ioXmlGenerator.GenerateAlarms(tab.TabPage.Text, tab.TabConfig, this.templateHandler, tab.DeviceDataList);
            }
            ioXmlGenerator.ExportXML(folderPath);
        }

        public Control? GetControl()
        {
            return this.control;
        }

        public string GetFormLocalizatedName()
        {
            return Locale.ALARM_GEN_FORM;
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.control.tabControl.SelectedTab;
            if (selectedTab != null && selectedTab.Tag is AlarmGenTab alarmGenTab)
            {
                return alarmGenTab.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        private void AddConfigurationBindings(SettingsBindings settingsBindings)
        {
            settingsBindings
                .MacroSection(() => "AlarmGenControl", () => this.mainConfig, MainForm.Settings.PresetAlarmMainConfiguration)

                .Section("Enables")
                .AddBool(nameof(AlarmMainConfiguration.EnableCustomVariable), Locale.ALARM_CONFIG_ENABLE_CUSTOM_VAR)
                .AddBool(nameof(AlarmMainConfiguration.EnableTimer), Locale.ALARM_CONFIG_ENABLE_TIMER)

                .Section(Locale.ALARM_CONFIG_FC)
                .AddString(nameof(AlarmMainConfiguration.FCBlockName), Locale.GENERICS_NAME)
                .AddUInt(nameof(AlarmMainConfiguration.FCBlockNumber), Locale.GENERICS_NUMBER)

                .Section(Locale.ALARM_CONFIG_SEGMENT_NAME)
                .AddString(nameof(AlarmMainConfiguration.OneEachSegmentName), Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH)
                .AddString(nameof(AlarmMainConfiguration.OneEachEmptyAlarmSegmentName), Locale.ALARM_CONFIG_SEGMENT_NAME_ONE_EACH_EMPTY)
                .AddString(nameof(AlarmMainConfiguration.GroupSegmentName), Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH)
                .AddString(nameof(AlarmMainConfiguration.GroupEmptyAlarmSegmentName), Locale.ALARM_CONFIG_SEGMENT_NAME_GROUP_EACH_EMPTY)

                .Section(Locale.ALARM_CONFIG_FORMATTING)
                .AddString(nameof(AlarmMainConfiguration.UDTBlockName), Locale.ALARM_CONFIG_FORMATTING_UDT_NAME)
                .AddString(nameof(AlarmMainConfiguration.AlarmNumFormat), Locale.ALARM_CONFIG_FORMATTING_FORMAT)
                .AddString(nameof(AlarmMainConfiguration.AlarmNameTemplate), Locale.ALARM_CONFIG_FORMATTING_NAME_TEMPLATE)
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplate), Locale.ALARM_CONFIG_FORMATTING_COMMENT_TEMPLATE)
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplateSpare), Locale.ALARM_CONFIG_FORMATTING_COMMENT_TEMPLATE_SPARE)

                .Section(Locale.GENERICS_HMI)
                .AddString(nameof(AlarmMainConfiguration.HmiNameTemplate), Locale.ALARM_CONFIG_FORMATTING_HMI_NAME)
                .AddString(nameof(AlarmMainConfiguration.HmiTextTemplate), Locale.ALARM_CONFIG_FORMATTING_HMI_TEXT)
                .AddString(nameof(AlarmMainConfiguration.HmiTriggerTagTemplate), Locale.ALARM_CONFIG_FORMATTING_HMI_TRIGGER_TAG_TEMPLATE)
                .AddBool(nameof(AlarmMainConfiguration.HmiTriggerTagUseWordArray), Locale.ALARM_CONFIG_FORMATTING_HMI_USE_WORD_ARRAY);

            settingsBindings
                .MacroSection(this.GetCurrentTabName, this.GetCurrentTabConfiguration, MainForm.Settings.PresetAlarmTabConfiguration, () => this.TabConfigurations)

                .Section("Grouping")
                .AddEnum(nameof(AlarmTabConfiguration.GroupingType), "Raggruppamento")

                .Section(Locale.ALARM_CONFIG_GENERATION)
                .AddUInt(nameof(AlarmTabConfiguration.TotalAlarmNum), Locale.ALARM_CONFIG_GENERATION_TOTAL_NUM)
                .AddUInt(nameof(AlarmTabConfiguration.StartingAlarmNum), Locale.ALARM_CONFIG_GENERATION_START_NUM)
                .AddUInt(nameof(AlarmTabConfiguration.SkipNumberAfterGroup), Locale.ALARM_CONFIG_GENERATION_SKIP)

                .Section(Locale.GENERICS_HMI)
                .AddUInt(nameof(AlarmTabConfiguration.HmiStartID), Locale.ALARM_CONFIG_GENERATION_SKIP)
                .AddString(nameof(AlarmTabConfiguration.DefaultHmiAlarmClass), Locale.ALARM_CONFIG_GENERATION_SKIP)

                .Section(Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP)
                .AddUInt(nameof(AlarmTabConfiguration.AntiSlipNumber), Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_AMOUNT)
                .AddBool(nameof(AlarmTabConfiguration.GenerateEmptyAlarmAntiSlip), Locale.ALARM_CONFIG_GENERATION_ANTI_SLIP_GEN_EMPTY)

                .Section("EmptyAlarms")
                .AddUInt(nameof(AlarmTabConfiguration.EmptyAlarmAtEnd), Locale.ALARM_CONFIG_GENERATION_EMPTY_NUM)
                .AddString(nameof(AlarmTabConfiguration.EmptyAlarmContactAddress), Locale.ALARM_CONFIG_GENERATION_EMPTY_ALARM_ADDRESS)

                .Section(Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER)
                .AddString(nameof(AlarmTabConfiguration.EmptyAlarmTimerAddress), Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_ADDRESS)
                .AddString(nameof(AlarmTabConfiguration.EmptyAlarmTimerType), Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_TYPE)
                .AddString(nameof(AlarmTabConfiguration.EmptyAlarmTimerValue), Locale.ALARM_CONFIG_GENERATION_EMPTY_TIMER_VALUE);
        }

        private string GetCurrentTabName()
        {
            var tabPage = this.control.tabControl.SelectedTab;
            return tabPage == null ? "" : tabPage.Text;
        }

        private AlarmTabConfiguration? GetCurrentTabConfiguration()
        {
            return this.control.tabControl.SelectedTab?.Tag is AlarmGenTab genTab ? genTab.TabConfig : null;
        }
    }
}
