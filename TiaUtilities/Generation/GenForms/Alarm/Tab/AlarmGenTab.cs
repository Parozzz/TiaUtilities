using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
using TiaUtilities.Generation.GenForms.IO.Tab;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GenForms.Alarm.Tab
{
    public class AlarmGenTab
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

        public AlarmGenTab(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings, TabPage tabPage)
        {
            this.TabPage = tabPage;

            this.TabSettings = new();

            this.deviceGridHandler = new GridHandler<AlarmTabConfiguration, DeviceData>(jsErrorHandlingThread, gridSettings, TabConfig) { RowCount = 499 };
            this.alarmGridHandler = new GridHandler<AlarmTabConfiguration, AlarmData>(jsErrorHandlingThread, gridSettings, TabConfig) { RowCount = 199 };

            this.TabControl = new(deviceGridHandler.DataGridView, alarmGridHandler.DataGridView);
            this.configHandler = new(this.TabConfig, TabControl);
        }

        public void Init()
        { 

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

            this.TabControl.Init(); //This before configHandler.
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
            this.deviceGridHandler.Events.ScriptLoad += args => args.Script = TabSettings.DeviceJSScript;
            this.deviceGridHandler.Events.ScriptChanged += args => TabSettings.DeviceJSScript = args.Script;

            this.alarmGridHandler.Events.ScriptLoad += args => args.Script = TabSettings.AlarmJSScript;
            this.alarmGridHandler.Events.ScriptChanged += args => TabSettings.AlarmJSScript = args.Script;
            #endregion

            this.alarmGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            this.deviceGridHandler.DataGridView.AutoResizeColumnHeadersHeight();

            Translate();
        }

        private void Translate()
        {
            this.TabControl.Translate();
        }
        
        public AlarmGenTabSave CreateSave()
        {
            AlarmGenTabSave save = new() 
            {
                TabConfig = this.TabConfig,
                Name = TabPage.Text,
            };

            GenerationUtils.CopyJsonFieldsAndProperties(this.TabConfig, save.TabConfig);

            foreach (var entry in this.deviceGridHandler.DataSource.GetNotEmptyDataDict())
            {
                save.AddDeviceData(entry.Key, entry.Value);
            }

            foreach (var entry in this.alarmGridHandler.DataSource.GetNotEmptyDataDict())
            {
                save.AddAlarmData(entry.Key, entry.Value);
            }

            return save;
        }

        public void LoadSave(AlarmGenTabSave save)
        {
            this.TabPage.Text = save.Name;
            GenerationUtils.CopyJsonFieldsAndProperties(save.TabConfig, this.TabConfig);

            this.alarmGridHandler.DataGridView.SuspendLayout();
            this.deviceGridHandler.DataGridView.SuspendLayout();

            this.alarmGridHandler.DataSource.InitializeData(this.alarmGridHandler.RowCount);
            this.deviceGridHandler.DataSource.InitializeData(this.deviceGridHandler.RowCount);

            foreach (var entry in save.DeviceData)
            {
                var rowIndex = entry.Key;
                if (rowIndex >= 0 && rowIndex <= this.deviceGridHandler.RowCount)
                {
                    var data = this.deviceGridHandler.DataSource[rowIndex];
                    this.deviceGridHandler.DataHandler.CopyValues(entry.Value, data);
                }
            }

            foreach (var entry in save.AlarmData)
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

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.deviceGridHandler.ProcessCmdKey(ref msg, keyData) || this.alarmGridHandler.ProcessCmdKey(ref msg, keyData);
        }
    }
}
