using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.GenModules.Alarm;
using TiaUtilities.Generation.Alarms.Module.Tab;

namespace TiaUtilities.Generation.Alarms.Module
{
    public class AlarmGenModule : IGenModule
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];

        private readonly JavascriptErrorReportThread jsErrorHandlingThread;
        private readonly GridSettings gridSettings;

        private readonly GridScriptContainer scriptContainer;

        private readonly AlarmGenControl control;

        private readonly AlarmMainConfiguration mainConfig;

        private readonly List<AlarmGenTab> genTabList;

        public AlarmGenModule(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings)
        {
            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.gridSettings = gridSettings;

            this.scriptContainer = new();

            this.control = new();
            this.mainConfig = new();

            this.genTabList = [];
        }

        public void Init(GenModuleForm form)
        {
            this.control.BindConfig(this.mainConfig);

            this.control.tabControl.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;
                tabPage.Text = "AlarmGen";

                AlarmGenTab alarmGenTab = new(this.jsErrorHandlingThread, this.gridSettings, this.scriptContainer, this, this.mainConfig, tabPage);
                genTabList.Add(alarmGenTab);

                alarmGenTab.Init();
                tabPage.Tag = alarmGenTab;

                tabPage.Controls.Add(alarmGenTab.TabControl);
            };

            this.control.tabControl.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab alarmGenTab)
                {
                    genTabList.Remove(alarmGenTab);
                }
            };

            Translate();
        }

        public void Clear()
        {
            this.genTabList.Clear();
            this.control.tabControl.TabPages.Clear();
        }

        public bool IsDirty() => mainConfig.IsDirty() || genTabList.Any(x => x.IsDirty());
        public void Wash()
        {
            mainConfig.Wash();
            foreach (var tab in genTabList)
            {
                tab.Wash();
            }
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSave()
            {
                ScriptContainer = scriptContainer.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(mainConfig, projectSave.AlarmMainConfig);

            foreach (var tab in genTabList)
            {
                var tabSave = tab.CreateSave();
                projectSave.TabSaves.Add(tabSave);
            }

            return projectSave;
        }

        public void LoadSave(object? saveObject)
        {
            if (saveObject is not AlarmGenSave loadedSave)
            {
                return;
            }

            this.Clear();

            scriptContainer.LoadSave(loadedSave.ScriptContainer);
            
            GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, mainConfig);

            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new() { Text = tabSave.Name };

                AlarmGenTab alarmGenTab = new(this.jsErrorHandlingThread, this.gridSettings, this.scriptContainer, this, this.mainConfig, tabPage);
                genTabList.Add(alarmGenTab);

                alarmGenTab.Init();
                alarmGenTab.LoadSave(tabSave);
                tabPage.Tag = alarmGenTab;

                tabPage.Controls.Add(alarmGenTab.TabControl);

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
            return Localization.Get("ALARM_GEN_FORM");
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

        private void Translate()
        {
        }
    }
}
