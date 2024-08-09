using InfoBox;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.IO;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public class AlarmGenProject : IGenerationProject
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];

        private readonly JavascriptErrorReportThread jsErrorHandlingThread;
        private readonly GridSettings gridSettings;


        private readonly AlarmGenConfigTopControl configControlTop;
        private readonly TabbedView tabbedViewBottom;

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmGenConfigHandler configHandler;

        private readonly List<AlarmGenTab> genTabList;

        public AlarmGenProject(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings)
        {
            this.jsErrorHandlingThread = jsErrorHandlingThread;
            this.gridSettings = gridSettings;

            this.configControlTop = new();
            this.tabbedViewBottom = new();

            this.mainConfig = new();
            this.configHandler = new AlarmGenConfigHandler(this.configControlTop, this.mainConfig);

            this.genTabList = [];
        }

        public void Init(GenerationProjectForm form)
        {
            this.tabbedViewBottom.TabAdded += (sender, args) =>
            {
                var tabPage = args.TabPage;
                tabPage.Text = "AlarmGen";

                AlarmGenTab alarmGenTab = new(jsErrorHandlingThread, this.gridSettings, tabPage);
                alarmGenTab.Init();
                this.genTabList.Add(alarmGenTab);

                tabPage.Tag = alarmGenTab;
                tabPage.Controls.Add(alarmGenTab.TabControl);
            };

            this.tabbedViewBottom.TabRemoved += (sender, args) =>
            {
                if (args.TabPage.Tag is AlarmGenTab alarmGenTab)
                {
                    var result = InformationBox.Show($"Are you sure you want to close {args.TabPage.Text}?", buttons: InformationBoxButtons.YesNo);
                    if (result == InformationBoxResult.No)
                    {
                        args.Handled = true;
                        return;
                    }

                    this.genTabList.Remove(alarmGenTab);
                }
            };

            this.configHandler.Init();

            Translate();
        }

        public IGenerationProjectSave CreateProjectSave()
        {
            var projectSave = new AlarmGenSave();

            GenerationUtils.CopyJsonFieldsAndProperties(this.mainConfig, projectSave.AlarmMainConfig);

            foreach (var tab in genTabList)
            {
                var tabSave = tab.CreateSave();
                projectSave.TabSaves.Add(tabSave);
            }

            return projectSave;
        }

        public IGenerationProjectSave? Load(ref string? filePath)
        {
            var loadedProjectSave = AlarmGenSave.Load(ref filePath);
            if (loadedProjectSave != null)
            {
                GenerationUtils.CopyJsonFieldsAndProperties(loadedProjectSave.AlarmMainConfig, this.mainConfig);

                this.tabbedViewBottom.ClearAllTabs();
                foreach (var tabSave in loadedProjectSave.TabSaves)
                {
                    var tabPage = this.tabbedViewBottom.AddTab();

                    AlarmGenTab alarmGenTab = new(jsErrorHandlingThread, this.gridSettings, tabPage);
                    alarmGenTab.Init();
                    alarmGenTab.LoadSave(tabSave);
                    this.genTabList.Add(alarmGenTab);

                    tabPage.Tag = alarmGenTab;
                    tabPage.Controls.Add(alarmGenTab.TabControl);
                }
            }
            return loadedProjectSave;
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
            return tabbedViewBottom;
        }

        public string GetFormLocalizatedName()
        {
            return Localization.Get("ALARM_GEN_FORM");
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var visibleTag = this.tabbedViewBottom.GetVisibleTab();
            if (visibleTag != null && visibleTag.Tag is AlarmGenTab alarmGenTab)
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
