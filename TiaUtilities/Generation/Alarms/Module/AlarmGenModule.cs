using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.SettingsNew;
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

            this.control.tabControl.Deselecting += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
                {
                    tab.UnbindSettings(this.SettingsBindings);
                }
            };
            this.control.tabControl.Selected += (sender, args) =>
            {
                if (args.TabPage?.Tag is AlarmGenTab tab)
                {
                    tab.Selected();
                    tab.BindSettings(this.SettingsBindings);
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

            this.AddMainConfigurationSettingsBindings(this.SettingsBindings);
        }

        private void TabCreation(TabPage tabPage, AlarmGenTabSave? save = null)
        {
            tabPage.Text = save?.Name ?? "AlarmGen";

            AlarmGenTab alarmGenTab = new(this.gridBindContainer, this, this.mainConfig, this.templateHandler, tabPage);

            alarmGenTab.Init();
            if (save != null)
            {
                alarmGenTab.LoadSave(save);
            }
            tabPage.Tag = alarmGenTab;
            tabPage.Controls.Add(alarmGenTab.TabControl);

            genTabList.Add(alarmGenTab); //Do this AFTER. Otherwise the Selected event is called with Tag null.
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

            this.AddMainConfigurationSettingsBindings(this.SettingsBindings);

            //Seems that the Selected event is not called in this case. Doing it manually.
            if (this.control.tabControl.SelectedTab?.Tag is AlarmGenTab tab)
            {
                tab.Selected();
                tab.BindSettings(this.SettingsBindings);
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

        private void AddMainConfigurationSettingsBindings(SettingsBindings settingsBindings)
        {
            settingsBindings
                .MacroSection("AlarmGenControl", mainConfig, MainForm.Settings.PresetAlarmMainConfiguration)

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
        }
    }
}
