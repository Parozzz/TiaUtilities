using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.GenModules;
using TiaUtilities.Generation.GenModules.Alarm;
using TiaUtilities.Generation.GenModules.Alarm.Tab;

namespace TiaUtilities.Generation.GenModules.Alarm
{
    public class AlarmGenModule : IGenModule
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];

        private readonly JavascriptErrorReportThread jsErrorHandlingThread;
        private readonly GridSettings gridSettings;

        private readonly GridScriptContainer scriptContainer;

        private readonly AlarmGenConfigTopControl configControlTop;
        private readonly InteractableTabControl controlControlBottom;

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenConfigHandler configHandler;

        private readonly List<AlarmGenTab> genTabList;

        public AlarmGenModule(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings)
        {
            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.gridSettings = gridSettings;

            this.scriptContainer = new();

            this.configControlTop = new();
            this.controlControlBottom = new() { RequireConfirmationBeforeClosing = true };

            this.mainConfig = new();
            this.configHandler = new AlarmGenConfigHandler(this.configControlTop, this.mainConfig);

            this.genTabList = [];
        }

        public void Init(GenModuleForm form)
        {
            this.controlControlBottom.TabPreAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;
                tabPage.Text = "AlarmGen";

                AlarmGenTab alarmGenTab = new(jsErrorHandlingThread, this.gridSettings, tabPage, scriptContainer);
                alarmGenTab.Init();
                this.genTabList.Add(alarmGenTab);

                tabPage.Controls.Add(alarmGenTab.TabControl);
                tabPage.Tag = alarmGenTab;
            };

            this.controlControlBottom.TabPreRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab alarmGenTab)
                {
                    this.genTabList.Remove(alarmGenTab);
                }
            };

            this.configHandler.Init();

            Translate();
        }

        public bool IsDirty()
        {
            var dirty = this.mainConfig.IsDirty();
            foreach (var genTab in this.genTabList)
            {
                dirty |= genTab.IsDirty();
            }
            return dirty;
        }

        public void Wash()
        {
            this.mainConfig.Wash();
            foreach (var genTab in this.genTabList)
            {
                genTab.Wash();
            }
        }

        public object CreateSave()
        {
            var projectSave = new AlarmGenSave()
            {
                ScriptContainer = this.scriptContainer.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(this.mainConfig, projectSave.AlarmMainConfig);

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

            this.scriptContainer.LoadSave(loadedSave.ScriptContainer);

            GenUtils.CopyJsonFieldsAndProperties(loadedSave.AlarmMainConfig, this.mainConfig);

            this.controlControlBottom.TabPages.Clear();
            foreach (var tabSave in loadedSave.TabSaves)
            {
                TabPage tabPage = new()
                {
                    Text = tabSave.Name
                };
                this.controlControlBottom.TabPages.Add(tabPage);

                AlarmGenTab alarmGenTab = new(jsErrorHandlingThread, this.gridSettings, tabPage, scriptContainer);
                alarmGenTab.Init();
                alarmGenTab.LoadSave(tabSave);
                this.genTabList.Add(alarmGenTab);

                tabPage.Tag = alarmGenTab;
                tabPage.Controls.Add(alarmGenTab.TabControl);
            }
        }

        public void ExportXML(string folderPath)
        {
            var ioXmlGenerator = new AlarmXmlGenerator(this.mainConfig);
            ioXmlGenerator.Init();
            foreach (var tab in genTabList)
            {
                ioXmlGenerator.GenerateAlarms(tab.TabPage.Text, tab.TabConfig, tab.AlarmDataList, tab.DeviceDataList);
            }
            ioXmlGenerator.ExportXML(folderPath);
        }

        public Control? GetTopControl()
        {
            return configControlTop;
        }

        public Control? GetBottomControl()
        {
            return controlControlBottom;
        }

        public string GetFormLocalizatedName()
        {
            return Localization.Get("ALARM_GEN_FORM");
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var selectedTab = this.controlControlBottom.SelectedTab;
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
