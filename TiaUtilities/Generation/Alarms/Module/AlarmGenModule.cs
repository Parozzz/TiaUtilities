using TiaUtilities.Generation.Alarms.Module.Tab;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.GenModules.Alarm;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Languages;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        private readonly GridBindContainer gridBindContainer;
        private GridScriptHandler GridScriptHandler { get => this.gridBindContainer.GridScriptHandler; }

        private readonly AlarmGenControl control;
        private readonly AlarmMainConfiguration mainConfig;

        private readonly List<AlarmGenTab> genTabList;

        public AlarmGenModule(JavascriptErrorReportThread jsErrorHandlingThread)
        {
            this.gridBindContainer = new(jsErrorHandlingThread);

            this.control = new();
            this.mainConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmMainConfiguration, this.mainConfig);

            this.genTabList = [];
        }

        public void Init(GenModuleForm form)
        {
            this.gridBindContainer.Init(form);

            this.control.BindConfig(this.mainConfig);

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
                if(this.control.tabControl.TabCount == 0)
                { //Check required because Load could be called before form is shown!
                    TabPage tabPage = new();
                    TabCreation(tabPage);
                    this.control.tabControl.TabPages.Add(tabPage);
                }
            };
        }

        private void TabCreation(TabPage tabPage, AlarmGenTabSave? save = null)
        {
            tabPage.Text = save?.Name ?? "AlarmGen";

            AlarmGenTab alarmGenTab = new(MainForm.Settings.GridSettings, this.gridBindContainer, this, this.mainConfig, tabPage);
            genTabList.Add(alarmGenTab);

            alarmGenTab.Init();
            if (save != null)
            {
                alarmGenTab.LoadSave(save);
            }
            tabPage.Tag = alarmGenTab;
            tabPage.Controls.Add(alarmGenTab.TabControl);
        }

        public void Clear()
        {
            this.genTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => this.mainConfig.IsDirty() || this.genTabList.Any(x => x.IsDirty()) || this.GridScriptHandler.IsDirty();
        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var tab in this.genTabList)
            {
                tab.Wash();
            }
            this.GridScriptHandler.Wash();
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSave()
            {
                ScriptSave = this.GridScriptHandler.CreateSave()
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
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, mainConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new();
                TabCreation(tabPage, tabSave);
                this.control.tabControl.TabPages.Add(tabPage);
            }
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new AlarmXmlGenerator(mainConfig);
            ioXmlGenerator.Init();
            foreach (var tab in genTabList)
            {
                ioXmlGenerator.GenerateAlarms(tab.TabPage.Text, tab.TabConfig, tab.AlarmDataList, tab.DeviceDataList);
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
    }
}
