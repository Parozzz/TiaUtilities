using TiaUtilities.Generation.GenModules.Alarm.Tab;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmGenTab : ICleanable
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];
        private static readonly string[] ALARM_COIL_TYPE_ITEMS = [AlarmCoilType.NONE.ToString(), AlarmCoilType.SET.ToString(), AlarmCoilType.RESET.ToString(), AlarmCoilType.COIL.ToString()];

        private readonly AlarmGenModule module;

        public TabPage TabPage { get; init; }
        public AlarmTabConfiguration TabConfig { get; init; }
        public GridDataPreviewer<DeviceData> DeviceDataPreview { get; init; }
        public GridDataPreviewer<AlarmData> AlarmDataPreview { get; init; }

        private readonly GridHandler<DeviceData> deviceGridHandler;
        private readonly GridHandler<AlarmData> alarmGridHandler;
        public AlarmGenTabControl TabControl { get; init; }

        public List<DeviceData> DeviceDataList { get => new(deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!
        public List<AlarmData> AlarmDataList { get => new(alarmGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        private bool dirty = false;

        public AlarmGenTab(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings, GridScriptContainer scriptContainer, AlarmGenModule module, AlarmMainConfiguration mainConfig, TabPage tabPage)
        {
            this.module = module;
            TabPage = tabPage;

            this.TabConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetAlarmTabConfiguration, this.TabConfig);
            this.DeviceDataPreview = new();
            this.AlarmDataPreview = new();

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, this.TabConfig);
            deviceGridHandler = new(jsErrorHandlingThread, gridSettings, scriptContainer, this.DeviceDataPreview, placeholdersHandler) { RowCount = 499 };
            alarmGridHandler = new(jsErrorHandlingThread, gridSettings, scriptContainer, this.AlarmDataPreview, placeholdersHandler) { RowCount = 199 };

            TabControl = new(deviceGridHandler.DataGridView, alarmGridHandler.DataGridView);
        }

        public void Init()
        {
            #region DRAG
            deviceGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, deviceGridHandler);
            deviceGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, deviceGridHandler);

            alarmGridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, alarmGridHandler);
            alarmGridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, alarmGridHandler);
            #endregion 

            //Column initialization before gridHandler.Init()
            #region COLUMNS
            deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 125);
            deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            alarmGridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            alarmGridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 200);
            alarmGridHandler.AddTextBoxColumn(AlarmData.COIL1_ADDRESS, 145);
            alarmGridHandler.AddComboBoxColumn(AlarmData.COIL1_TYPE, 55, ALARM_COIL_TYPE_ITEMS);
            alarmGridHandler.AddTextBoxColumn(AlarmData.COIL2_ADDRESS, 145);
            alarmGridHandler.AddComboBoxColumn(AlarmData.COIL2_TYPE, 55, ALARM_COIL_TYPE_ITEMS);
            alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 95);
            alarmGridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 50);
            alarmGridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            TabControl.Init(); //This before configHandler.
            TabControl.BindConfig(this.TabConfig);

            deviceGridHandler.Init();
            alarmGridHandler.Init();

            #region PREVIEW
            this.AlarmDataPreview.Function = (column, alarmData) =>
            {
                if (string.IsNullOrEmpty(alarmData.AlarmVariable) || alarmData.IsEmpty())
                {
                    return null;
                }

                if (column == AlarmData.ALARM_VARIABLE)
                {
                    return new() { Prefix = TabConfig.AlarmAddressPrefix, Value = alarmData.AlarmVariable };
                }
                else if (column == AlarmData.COIL1_ADDRESS)
                {
                    return new() { Prefix = TabConfig.Coil1AddressPrefix, DefaultValue = TabConfig.DefaultCoil1Address, Value = alarmData.Coil1Address };
                }
                else if (column == AlarmData.COIL1_TYPE && AlarmData.IsAddressValid(alarmData.Coil1Address))
                {
                    return new() { DefaultValue = TabConfig.DefaultCoil1Type.ToString(), Value = alarmData.Coil1Type };
                }
                else if (column == AlarmData.COIL2_ADDRESS)
                {
                    return new() { Prefix = TabConfig.Coil2AddressPrefix, DefaultValue = TabConfig.DefaultCoil2Address, Value = alarmData.Coil2Address };
                }
                else if (column == AlarmData.COIL2_TYPE && AlarmData.IsAddressValid(alarmData.Coil2Address))
                {
                    return new() { DefaultValue = TabConfig.DefaultCoil2Type.ToString(), Value = alarmData.Coil2Type };
                }
                else if (column == AlarmData.TIMER_ADDRESS)
                {
                    return new() { Prefix = TabConfig.TimerAddressPrefix, DefaultValue = TabConfig.DefaultTimerAddress, Value = alarmData.TimerAddress };
                }
                else if (string.IsNullOrEmpty(alarmData.TimerAddress) ? AlarmData.IsAddressValid(TabConfig.DefaultTimerAddress) : AlarmData.IsAddressValid(alarmData.TimerAddress))
                {
                    if (column == AlarmData.TIMER_TYPE)
                    {
                        return new() { DefaultValue = TabConfig.DefaultTimerType, Value = alarmData.TimerType };
                    }
                    else if (column == AlarmData.TIMER_VALUE)
                    {
                        return new() { DefaultValue = TabConfig.DefaultTimerValue, Value = alarmData.TimerValue };
                    }
                }

                return null;
            };

            this.DeviceDataPreview.Function = (column, deviceData) => null;
            #endregion

            #region ENABLE_CHECKBOX_IF_FILLED
            alarmGridHandler.Events.CellChange += (sender, args) =>
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
            #endregion

            #region JS_SCRIPT_EVENTS
            this.alarmGridHandler.Events.ScriptShowVariable += (sender, args) =>
            {
                args.VariableList.Add("tabName [string]");
            };
            this.alarmGridHandler.Events.ScriptAddVariables += (sender, args) =>
            {
                args.VariableDict.Add("tabName", this.TabPage.Text);
            };

            this.deviceGridHandler.Events.ScriptShowVariable += (sender, args) =>
            {
                args.VariableList.Add("tabName [string]");
            };
            this.deviceGridHandler.Events.ScriptAddVariables += (sender, args) =>
            {
                args.VariableDict.Add("tabName", this.TabPage.Text);
            };
            #endregion

            #region DIRTY
            this.TabPage.TextChanged += (sender, args) => this.dirty = true;
            #endregion

            alarmGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            deviceGridHandler.DataGridView.AutoResizeColumnHeadersHeight();

            Translate();
        }

        private void Translate()
        {
            TabControl.Translate();
        }

        public bool IsDirty() => this.dirty || this.TabConfig.IsDirty() || this.deviceGridHandler.IsDirty() || this.alarmGridHandler.IsDirty();
        public void Wash()
        {
            this.dirty = false;
            this.TabConfig.Wash();
            this.deviceGridHandler.Wash();
            this.alarmGridHandler.Wash();
        }

        public AlarmGenTabSave CreateSave()
        {
            AlarmGenTabSave save = new()
            {
                TabConfig = TabConfig,
                Name = TabPage.Text,
                DeviceGrid = deviceGridHandler.CreateSave(),
                AlarmGrid = alarmGridHandler.CreateSave()
            };

            GenUtils.CopyJsonFieldsAndProperties(TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(AlarmGenTabSave save)
        {
            TabPage.Text = save.Name;
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, TabConfig);

            alarmGridHandler.LoadSave(save.AlarmGrid);
            deviceGridHandler.LoadSave(save.DeviceGrid);
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return deviceGridHandler.ProcessCmdKey(ref msg, keyData) || alarmGridHandler.ProcessCmdKey(ref msg, keyData);
        }
    }
}
