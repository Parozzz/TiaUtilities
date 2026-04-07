using TiaUtilities.Configuration;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.Alarms.Template;
using TiaUtilities.Generation.Alarms.Xml;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
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
            #region TOP_BUTTONS_STRIP
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
            #endregion

            this.gridBindContainer.Init(form);

            #region TEMPLATE_HANDLER
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
            #endregion

            #region TAB_CONTROL
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
            #endregion

            #region SETTINGS_BINDINGS 
            void placeholderRequestEvent(object? sender, EventArgs args) => this.OpenPlaceholderViewer();
            this.SettingsBindings.PlaceholderViewerRequestEvent += placeholderRequestEvent;

            form.FormClosed += (sender, args) =>
            {
                this.SettingsBindings.PlaceholderViewerRequestEvent -= placeholderRequestEvent;
            };
            #endregion

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

        public void OpenPlaceholderViewer()
        {
            new PlaceholderViewerForm(GenPlaceholders.Alarms.PLACEHOLDER_LIST).Show();
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
            AlarmGenUtils.AddMainConfigBindings(settingsBindings, this.mainConfig);

            AlarmGenUtils.AddTabConfigSettings(settingsBindings, 
                this.GetCurrentTabName, 
                this.IsAnyTabSelected, 
                this.GetCurrentTabConfiguration, 
                this.GetTabConfigurationDict);

            AlarmGenUtils.AddTemplateConfigSettings(settingsBindings,
                this.GetActiveTemplateName,
                this.IsTemplateVisible,
                this.GetActiveTemplateConfiguration,
                this.GetTemplateConfigurationDict);
        }

        private string GetCurrentTabName()
        {
            var tabPage = this.control.tabControl.SelectedTab;
            return tabPage == null ? "" : tabPage.Text;
        }

        private bool IsAnyTabSelected()
        {
            return this.control.tabControl.SelectedTab != null;
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
                if (!dict.TryAdd(tab.Name, tab.TabConfig))
                {
                    dict.Add(tab.Name + "*", tab.TabConfig);
                }
            }
            return dict;
        }

        private string GetActiveTemplateName()
        {
            var selectedTemplate = this.templateHandler.SelectedTemplate;
            return selectedTemplate == null ? "" : selectedTemplate.Name;
        }

        private bool IsTemplateVisible()
        {
            return shownTemplateForm != null && shownTemplateForm.Visible;
        }

        private AlarmTemplateConfiguration? GetActiveTemplateConfiguration()
        {
            return this.templateHandler.SelectedTemplate?.TemplateConfig;
        }

        private Dictionary<string, ObservableConfiguration> GetTemplateConfigurationDict()
        {
            Dictionary<string, ObservableConfiguration> dict = [];
            foreach(var template in this.templateHandler.BindingList)
            {
                if(!dict.TryAdd(template.Name, template.TemplateConfig))
                {
                    dict.Add(template.Name + "*", template.TemplateConfig);
                }
            }
            return dict;
        }
    }
}
