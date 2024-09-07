using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTab : ICleanable
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];

        public TabPage TabPage { get; init; }

        public AlarmGenTabSettings TabSettings { get; init; }
        private readonly GridHandler<AlarmTabConfiguration, DeviceData> deviceGridHandler;
        private readonly GridHandler<AlarmTabConfiguration, AlarmData> alarmGridHandler;
        public AlarmGenTabControl TabControl { get; init; }
        private readonly AlarmGenTabConfigHandler configHandler;

        public List<DeviceData> DeviceDataList { get => new(this.deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!
        public List<AlarmData> AlarmDataList { get => new(this.alarmGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!
        public AlarmTabConfiguration TabConfig { get => TabSettings.TabConfiguration; }

        public AlarmGenTab(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings, TabPage tabPage, GridScriptContainer scriptContainer)
        {
            this.TabPage = tabPage;

            this.TabSettings = new();

            this.deviceGridHandler = new GridHandler<AlarmTabConfiguration, DeviceData>(jsErrorHandlingThread, gridSettings, TabConfig, scriptContainer) { RowCount = 499 };
            this.alarmGridHandler = new GridHandler<AlarmTabConfiguration, AlarmData>(jsErrorHandlingThread, gridSettings, TabConfig, scriptContainer) { RowCount = 199 };

            this.TabControl = new(deviceGridHandler.DataGridView, alarmGridHandler.DataGridView);
            this.configHandler = new(this.TabConfig, TabControl);
        }

        public void Init()
        {
            #region DRAG
            this.deviceGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, this.deviceGridHandler);
            this.deviceGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, this.deviceGridHandler);

            this.alarmGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, this.alarmGridHandler);
            this.alarmGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, this.alarmGridHandler);
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

            this.TabControl.Init(); //This before configHandler.
            this.configHandler.Init();

            this.alarmGridHandler.Events.CellChange += (sender, args) =>
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

            this.alarmGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            this.deviceGridHandler.DataGridView.AutoResizeColumnHeadersHeight();

            Translate();
        }

        private void Translate()
        {
            this.TabControl.Translate();
        }

        public bool IsDirty() => this.TabConfig.IsDirty() || this.deviceGridHandler.IsDirty() || this.alarmGridHandler.IsDirty();
        public void Wash()
        {
            this.TabConfig.Wash();
            this.deviceGridHandler.Wash();
            this.alarmGridHandler.Wash();
        }

        public AlarmGenTabSave CreateSave()
        {
            AlarmGenTabSave save = new() 
            {
                TabConfig = this.TabConfig,
                Name = TabPage.Text,
                DeviceGrid = this.deviceGridHandler.CreateSave(),
                AlarmGrid = this.alarmGridHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(this.TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(AlarmGenTabSave save)
        {
            this.TabPage.Text = save.Name;
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, this.TabConfig);

            this.alarmGridHandler.LoadSave(save.AlarmGrid);
            this.deviceGridHandler.LoadSave(save.DeviceGrid);
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.deviceGridHandler.ProcessCmdKey(ref msg, keyData) || this.alarmGridHandler.ProcessCmdKey(ref msg, keyData);
        }
    }
}
