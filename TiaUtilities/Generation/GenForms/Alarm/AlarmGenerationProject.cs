using TiaXmlReader.CustomControls;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;
using TiaXmlReader.Languages;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public class AlarmGenerationProject : IGenerationProject
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];

        private readonly AlarmGenerationSettings settings;

        public readonly GridHandler<AlarmConfiguration, DeviceData> deviceGridHandler;
        public readonly GridHandler<AlarmConfiguration, AlarmData> alarmGridHandler;

        private readonly AlarmConfigControl top;
        private readonly AlarmTabbedView bottom;

        private readonly AlarmGenerationProjectConfigHandler configHandler;
        private AlarmConfiguration AlarmConfig { get => settings.Configuration; }

        public AlarmGenerationProject(JavascriptErrorReportThread jsErrorHandlingThread, AlarmGenerationSettings settings, GridSettings gridSettings)
        {
            this.settings = settings;

            this.deviceGridHandler = new GridHandler<AlarmConfiguration, DeviceData>(jsErrorHandlingThread, gridSettings, AlarmConfig) { RowCount = 499 };
            this.alarmGridHandler = new GridHandler<AlarmConfiguration, AlarmData>(jsErrorHandlingThread, gridSettings, AlarmConfig) { RowCount = 199 };

            this.top = new();
            this.bottom = new(this);

            this.configHandler = new AlarmGenerationProjectConfigHandler(this.top, this.AlarmConfig);
        }

        public void Init(GenerationProjectForm form)
        {
            this.top.Init();
            this.bottom.Init();

            #region DRAG
            this.deviceGridHandler.SetDragPreviewAction(data => { GridUtils.DragPreview(data, this.deviceGridHandler); });
            this.deviceGridHandler.SetDragMouseUpAction(data => { GridUtils.DragMouseUp(data, this.deviceGridHandler); });

            this.alarmGridHandler.SetDragPreviewAction(data => { GridUtils.DragPreview(data, this.alarmGridHandler); });
            this.alarmGridHandler.SetDragMouseUpAction(data => { GridUtils.DragMouseUp(data, this.alarmGridHandler); });
            #endregion 

            //Column initialization before gridHandler.Init()
            #region COLUMNS
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 125);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.ADDRESS, 160);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            this.alarmGridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 250);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.COIL1_ADDRESS, 95);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.COIL2_ADDRESS, 95);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 95);
            this.alarmGridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 50);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            this.deviceGridHandler.Init();
            this.alarmGridHandler.Init();
            this.configHandler.Init();

            this.alarmGridHandler.Events.CellChange += args =>
            {
                if (args.CellChangeList == null)
                {
                    return;
                }

                foreach (var cellChange in args.CellChangeList)
                {
                    if (AlarmData.ALARM_VARIABLE == cellChange.ColumnIndex)
                    {//If an alarm variable is filled (Before empty and now full) i will automatically set the enable to be true. The opposite removes the enable. QOL
                        if (cellChange.IsOldValueEmptyString() && cellChange.IsNewValueFullString())
                        {
                            alarmGridHandler.DataSource[cellChange.RowIndex].Enable = true;
                        }
                        else if (cellChange.IsOldValueFullString() && cellChange.IsNewValueEmptyString())
                        {
                            alarmGridHandler.DataSource[cellChange.RowIndex].Enable = false;
                        }
                    }
                }
            };

            #region GRIDS_JSCRIPT_EVENTS
            this.deviceGridHandler.Events.ScriptLoad += args => args.Script = settings.DeviceJSScript;
            this.deviceGridHandler.Events.ScriptChanged += args => settings.DeviceJSScript = args.Script;

            this.alarmGridHandler.Events.ScriptLoad += args => args.Script = settings.AlarmJSScript;
            this.alarmGridHandler.Events.ScriptChanged += args => settings.AlarmJSScript = args.Script;
            #endregion

            form.Shown += (sender, args) =>
            {
                this.alarmGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
                this.deviceGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            };

            Translate();
        }

        public IGenerationProjectSave CreateProjectSave()
        {
            var projectSave = new AlarmGenerationProjectSave();

            GenerationUtils.CopyJsonFieldsAndProperties(this.AlarmConfig, projectSave.AlarmConfig);

            foreach (var entry in this.deviceGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddDeviceData(entry.Key, entry.Value);
            }

            foreach (var entry in this.alarmGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddAlarmData(entry.Key, entry.Value);
            }

            return projectSave;
        }

        public void ExportXML(string folderPath)
        {
            var alarmDataList = new List<AlarmData>(this.alarmGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys);  //Return CLONED data, otherwise operations on the xml generation will affect the table!
            var deviceDataList = new List<DeviceData>(this.deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys);  //Return CLONED data, otherwise operations on the xml generation will affect the table!

            var ioXmlGenerator = new AlarmXmlGenerator(this.AlarmConfig, alarmDataList, deviceDataList);
            ioXmlGenerator.GenerateBlocks();
            ioXmlGenerator.ExportXML(folderPath);
        }

        public Control? GetTopControl()
        {
            return top;
        }

        public Control? GetBottomControl()
        {
            return bottom;
        }

        public string GetFormLocalizatedName()
        {
            return Localization.Get("ALARM_GEN_FORM");
        }

        public IGenerationProjectSave? Load(ref string? filePath)
        {
            var loadedProjectSave = AlarmGenerationProjectSave.Load(ref filePath);
            if (loadedProjectSave != null)
            {
                GenerationUtils.CopyJsonFieldsAndProperties(loadedProjectSave.AlarmConfig, this.AlarmConfig);

                this.alarmGridHandler.DataGridView.SuspendLayout();
                this.deviceGridHandler.DataGridView.SuspendLayout();

                this.alarmGridHandler.DataSource.InitializeData(this.alarmGridHandler.RowCount);
                this.deviceGridHandler.DataSource.InitializeData(this.deviceGridHandler.RowCount);

                foreach (var entry in loadedProjectSave.DeviceData)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.deviceGridHandler.RowCount)
                    {
                        var data = this.deviceGridHandler.DataSource[rowIndex];
                        this.deviceGridHandler.DataHandler.CopyValues(entry.Value, data);
                    }
                }

                foreach (var entry in loadedProjectSave.AlarmData)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.alarmGridHandler.RowCount)
                    {
                        var data = this.alarmGridHandler.DataSource[rowIndex];
                        this.alarmGridHandler.DataHandler.CopyValues(entry.Value, data);
                    }
                }

                this.alarmGridHandler.DataGridView.Refresh();
                this.deviceGridHandler.DataGridView.Refresh();

                this.alarmGridHandler.DataGridView.ResumeLayout();
                this.deviceGridHandler.DataGridView.ResumeLayout();
            }
            return loadedProjectSave;
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.deviceGridHandler.ProcessCmdKey(ref msg, keyData) || this.alarmGridHandler.ProcessCmdKey(ref msg, keyData);
        }

        private void Translate()
        {
        }
    }
}
