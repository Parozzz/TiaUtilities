using TiaUtilities.Configuration;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Alarms.Xml;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        public const int DEVICE_GRID_ROW_COUNT = 199;
        public const int TEMPLATE_GRID_ROW_COUNT = 499;

        private readonly GridBindContainer gridBindContainer;
        private GridScriptHandler GridScriptHandler { get => this.gridBindContainer.GridScriptHandler; }

        private readonly AlarmGenControl control;
        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenTemplateHandler templateHandler;

        private readonly List<AlarmGenTab> alarmTabList;
        public IEnumerable<AlarmTabConfiguration> TabConfigurations { get => this.alarmTabList.Select(tab => tab.TabConfig); }

        public SettingsBindings SettingsBindings { get; init; }

        private AlarmGenTemplateForm? shownTemplateForm = null;

        public AlarmGenModule(ErrorReportThread errorThread)
        {
            this.gridBindContainer = new(errorThread);

            this.control = new();
            this.mainConfig = new();
            this.templateHandler = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmMainConfiguration, this.mainConfig);

            this.alarmTabList = [];
            this.SettingsBindings = new();
        }

        public void Init(GenModuleForm form)
        {
            this.control.setupButton.Click += (sender, args) => new SettingsForm(this.SettingsBindings).Show(this.control);
            this.control.changeTemplateButton.Click += (sender, args) =>
            {
                var currentTabConfig = GetCurrentTabConfiguration();
                if (currentTabConfig != null)
                {
                    shownTemplateForm = new AlarmGenTemplateForm(mainConfig, currentTabConfig, this.gridBindContainer, templateHandler);
                    shownTemplateForm.Init();
                    shownTemplateForm.Show(this.control);
                    shownTemplateForm.FormClosed += (sender, args) =>
                    {
                        shownTemplateForm = null;
                        this.SettingsBindings.Reload();
                    };

                    this.SettingsBindings.Reload();
                }
            };

            this.gridBindContainer.Init(form);
            this.templateHandler.Init([]);
            this.templateHandler.TemplateRenamed += (sender, args) =>
            {
                foreach (var tab in this.alarmTabList)
                {
                    tab.ParseTemplateRenamed(args.OldName, args.NewName);
                }

                this.SettingsBindings.Update();
            };
            this.templateHandler.SelectedTemplateChanged += (sender, args) => this.SettingsBindings.Update();

            this.control.tabControl.TabPreAdded += (sender, args) => TabCreation(args.TabPage);
            this.control.tabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab tab)
                {
                    alarmTabList.Remove(tab);
                }
            };

            this.control.tabControl.TabNameUserChanged += (sender, args) =>
            {
                var newName = args.NewName;
                foreach(var loopTab in this.alarmTabList)
                {
                    if(newName == loopTab.Name)
                    {
                        var tabNames = this.alarmTabList
                                            .Where(tab => tab.TabPage != args.TabPage)
                                            .Select(tab => tab.Name);

                        var fixedNewName = Utils.CheckEqualityAndAddNumberAtEnd(newName, tabNames);
                        args.NewName = fixedNewName;
                    }
                }

                this.SettingsBindings.Update();
            };
            this.control.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
                {
                    tab.Selected();
                    this.SettingsBindings.Update();
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
            AlarmGenTab alarmTab = new(this.gridBindContainer, this, this.mainConfig, this.templateHandler, tabPage);
            alarmTab.Init();

            if(save == null)
            {
                alarmTab.Name = Utils.CheckEqualityAndAddNumberAtEnd("AlarmTab", this.alarmTabList.Select(tab => tab.Name));
            }
            if (save != null)
            {
                alarmTab.LoadSave(save);
                alarmTab.Name = Utils.CheckEqualityAndAddNumberAtEnd(alarmTab.Name, this.alarmTabList.Select(tab => tab.Name)); //In case the loaded file has a duplicated name!
            }

            tabPage.Tag = alarmTab;
            tabPage.Controls.Add(alarmTab.DataGridViewControl);

            alarmTabList.Add(alarmTab); //Do this AFTER. Otherwise the Selected event is called with Tag null.
        }

        public void Clear()
        {
            this.SettingsBindings.Clear();

            this.alarmTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => this.mainConfig.IsDirty() || this.alarmTabList.Any(x => x.IsDirty()) || this.GridScriptHandler.IsDirty() || this.templateHandler.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var tab in this.alarmTabList)
            {
                tab.Wash();
            }
            this.GridScriptHandler.Wash();
            this.templateHandler.IsDirty();
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSaveV1()
            {
                ScriptSave = this.GridScriptHandler.CreateSave(),
                TemplateSaves = this.templateHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, projectSave.AlarmMainConfig);

            foreach (var tab in alarmTabList)
            {
                var tabSave = tab.CreateSave();
                projectSave.TabSaves.Add(tabSave);
            }

            return projectSave;
        }

        public void LoadSave(object saveObject)
        {
            if (saveObject is not AlarmGenSaveV1 loadedSave)
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
            AlarmXmlGenerator ioXmlGenerator = new(mainConfig);
            foreach (var tab in alarmTabList)
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
                .MacroSection(() => "AlarmGenControl", () => true, () => this.mainConfig, MainForm.Settings.PresetAlarmMainConfiguration)

                .Section(Locale.ALARM_SETTINGS_ENABLE)
                .AddBool(nameof(AlarmMainConfiguration.EnableCustomVariable), Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR, Locale.ALARM_SETTINGS_ENABLE_CUSTOM_VAR_DESCR)
                .AddBool(nameof(AlarmMainConfiguration.EnableTimer), Locale.ALARM_SETTINGS_ENABLE_TIMER, Locale.ALARM_SETTINGS_ENABLE_TIMER_DESCR)

                .Section(Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT)
                .AddString(nameof(AlarmMainConfiguration.AlarmNumFormat), description: Locale.ALARM_SETTINGS_ALARM_NUM_PLACEHOLDER_FORMAT_DESCR)

                .Section(Locale.ALARM_SETTINGS_FC)
                .AddString(nameof(AlarmMainConfiguration.FCBlockName), Locale.GENERICS_NAME)
                .AddUInt(nameof(AlarmMainConfiguration.FCBlockNumber), Locale.GENERICS_NUMBER)

                .Section(Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME)
                .AddString(nameof(AlarmMainConfiguration.OneEachSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_DESCR)
                .AddString(nameof(AlarmMainConfiguration.OneEachEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_ONE_EACH_SPARE)
                .AddString(nameof(AlarmMainConfiguration.GroupSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH, Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_DESCR)
                .AddString(nameof(AlarmMainConfiguration.GroupEmptyAlarmSegmentName), Locale.ALARM_SETTINGS_TAB_SEGMENT_NAME_GROUP_EACH_SPARE)

                .Section(Locale.ALARM_SETTINGS_UDT)
                .AddString(nameof(AlarmMainConfiguration.UDTBlockName), Locale.GENERICS_NAME, Locale.ALARM_SETTINGS_UDT_DESCR)

                .Section(Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE)
                .AddString(nameof(AlarmMainConfiguration.AlarmNameTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_NAME)
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplate), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_COMMENT)
                .AddString(nameof(AlarmMainConfiguration.AlarmCommentTemplateSpare), Locale.ALARM_SETTINGS_UDT_ALARM_VARIABLE_SPARE_COMMENT)

                .Section(Locale.GENERICS_HMI)
                .AddString(nameof(AlarmMainConfiguration.HmiNameTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_NAME, Locale.ALARM_SETTINGS_HMI_ITEM_NAME_DESCR)
                .AddString(nameof(AlarmMainConfiguration.HmiTextTemplate), Locale.ALARM_SETTINGS_HMI_ITEM_TEXT, Locale.ALARM_SETTINGS_HMI_ITEM_TEXT_DESCR)
                .AddString(nameof(AlarmMainConfiguration.HmiTriggerTagTemplate), Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG, Locale.ALARM_SETTINGS_HMI_TRIGGER_TAG_DESCR)
                .AddBool(nameof(AlarmMainConfiguration.HmiTriggerTagUseWordArray), Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY, Locale.ALARM_SETTINGS_HMI_USE_WORD_ARRAY_DESCR);

            settingsBindings
                .MacroSection(this.GetCurrentTabName, () => this.control.tabControl.SelectedTab != null, this.GetCurrentTabConfiguration, MainForm.Settings.PresetAlarmTabConfiguration, this.GetTabConfigurationDict)

                .Section(Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE)
                .AddEnum(nameof(AlarmTabConfiguration.GroupingType), description: Locale.ALARM_SETTINGS_TAB_GROUPING_TYPE_DESCR)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL1)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil1Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil1Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_COIL2)
                .AddString(nameof(AlarmTabConfiguration.DefaultCoil2Address), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .AddEnum(nameof(AlarmTabConfiguration.DefaultCoil2Type), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_TIMER)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
                .AddStringList(nameof(AlarmTabConfiguration.DefaultTimerType), ["TON", "TOF"], Locale.GENERICS_TYPE)
                .AddString(nameof(AlarmTabConfiguration.DefaultTimerValue), Locale.GENERICS_TYPE)

                .Section(Locale.ALARM_SETTINGS_TAB_TEMPLATE_DEFAULTS_CUSTOM_VAR)
                .AddString(nameof(AlarmTabConfiguration.DefaultCustomVarAddress), Locale.GENERICS_ADDRESS, Locale.GENERICS_DESCR_SET_SLASH_TO_DISABLE)
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


            settingsBindings
                .MacroSection(GetActiveTemplateName, () => shownTemplateForm != null && shownTemplateForm.Visible, GetActiveTemplateConfiguration, MainForm.Settings.PresetTemplateConfiguration)

                .Section(Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS)
                .AddBool(nameof(AlarmTemplateConfiguration.StandaloneAlarms), "", Locale.ALARM_SETTINGS_TEMPLATE_STANDALONE_ALARMS_DESC);
        }

        private string GetActiveTemplateName()
        {
            var selectedTemplate = this.templateHandler.SelectedTemplate;
            return selectedTemplate == null ? "" : selectedTemplate.Name;
        }

        private AlarmTemplateConfiguration? GetActiveTemplateConfiguration()
        {
            return this.templateHandler.SelectedTemplate?.TemplateConfig;
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

        private Dictionary<string, ObservableConfiguration> GetTabConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach (var tab in this.alarmTabList)
            {
                dict.Add(tab.Name, tab.TabConfig);
            }
            return dict;
        }
    }
}
