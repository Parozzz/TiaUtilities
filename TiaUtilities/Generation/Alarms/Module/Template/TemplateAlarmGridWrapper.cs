using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms.Module.Template
{
    public class TemplateAlarmGridWrapper : ICleanable
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];
        private static readonly string[] ALARM_COIL_TYPE_ITEMS = Enum.GetNames(typeof(AlarmCoilType));

        private readonly GridDataPreviewer<AlarmData> previewer;
        private readonly GridHandler<AlarmData> gridHandler;

        public List<AlarmData> AlarmDataList { get => new(gridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        public TemplateAlarmGridWrapper(GenPlaceholderHandler placeholderHandler, GridBindContainer bindContainer)
        {
            previewer = new();
            gridHandler = new(MainForm.Settings.GridSettings, bindContainer, previewer, placeholderHandler) { RowCount = 199 };
        }

        public void Init(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig, Func<AlarmTemplateConfiguration> getTemplateConfig)
        {
            #region DRAG
            gridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, gridHandler);
            gridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, gridHandler);
            #endregion

            #region COLUMNS
            gridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            gridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 200);
            gridHandler.AddCheckBoxColumn(AlarmData.ALARM_NEGATED, 40);
            gridHandler.AddTextBoxColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS, 145);
            gridHandler.AddTextBoxColumn(AlarmData.CUSTOM_VARIABLE_VALUE, 50);
            gridHandler.AddTextBoxColumn(AlarmData.COIL1_ADDRESS, 145);
            gridHandler.AddComboBoxColumn(AlarmData.COIL1_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            gridHandler.AddTextBoxColumn(AlarmData.COIL2_ADDRESS, 145);
            gridHandler.AddComboBoxColumn(AlarmData.COIL2_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            gridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 95);
            gridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            gridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 50);
            gridHandler.AddTextBoxColumn(AlarmData.HMI_ALARM_CLASS, 150);
            gridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            gridHandler.Init();

            #region HIDE_CUSTOM_VARIABLE/TIMER_COLUMNS
            ShowCustomVar(mainConfig.EnableCustomVariable);
            mainConfig.Subscribe(() => mainConfig.EnableCustomVariable, ShowCustomVar);

            ShowTimer(mainConfig.EnableTimer);
            mainConfig.Subscribe(() => mainConfig.EnableTimer, ShowTimer);
            #endregion

            #region PREVIEW
            previewer.Function = (column, alarmData) =>
            {
                if (string.IsNullOrEmpty(alarmData.AlarmVariable) || alarmData.IsEmpty())
                {
                    return null;
                }

                var templateConfig = getTemplateConfig();

                if (column == AlarmData.ALARM_VARIABLE)
                {
                    var prefix = templateConfig.StandaloneAlarms ? "" : tabConfig.AlarmAddressPrefix;
                    return new() { Prefix = prefix, Value = alarmData.AlarmVariable };
                }
                else if (column == AlarmData.CUSTOM_VARIABLE_ADDRESS)
                {
                    return new() { DefaultValue = tabConfig.DefaultCustomVarAddress, Value = alarmData.CustomVariableAddress };
                }
                else if (column == AlarmData.CUSTOM_VARIABLE_VALUE)
                {
                    return new() { DefaultValue = tabConfig.DefaultCustomVarValue, Value = alarmData.CustomVariableValue };
                }
                else if (column == AlarmData.COIL1_ADDRESS)
                {
                    return new() { Prefix = tabConfig.Coil1AddressPrefix, DefaultValue = tabConfig.DefaultCoil1Address, Value = alarmData.Coil1Address };
                }
                else if (column == AlarmData.COIL1_TYPE && AlarmData.IsAddressValid(alarmData.Coil1Address))
                {
                    return new() { DefaultValue = tabConfig.DefaultCoil1Type.ToString(), Value = alarmData.Coil1Type };
                }
                else if (column == AlarmData.COIL2_ADDRESS)
                {
                    return new() { Prefix = tabConfig.Coil2AddressPrefix, DefaultValue = tabConfig.DefaultCoil2Address, Value = alarmData.Coil2Address };
                }
                else if (column == AlarmData.COIL2_TYPE && AlarmData.IsAddressValid(alarmData.Coil2Address))
                {
                    return new() { DefaultValue = tabConfig.DefaultCoil2Type.ToString(), Value = alarmData.Coil2Type };
                }
                else if (column == AlarmData.TIMER_ADDRESS)
                {
                    return new() { Prefix = tabConfig.TimerAddressPrefix, DefaultValue = tabConfig.DefaultTimerAddress, Value = alarmData.TimerAddress };
                }
                else if (string.IsNullOrEmpty(alarmData.TimerAddress) ? AlarmData.IsAddressValid(tabConfig.DefaultTimerAddress) : AlarmData.IsAddressValid(alarmData.TimerAddress))
                {
                    if (column == AlarmData.TIMER_TYPE)
                    {
                        return new() { DefaultValue = tabConfig.DefaultTimerType, Value = alarmData.TimerType };
                    }
                    else if (column == AlarmData.TIMER_VALUE)
                    {
                        return new() { DefaultValue = tabConfig.DefaultTimerValue, Value = alarmData.TimerValue };
                    }
                }
                else if (column == AlarmData.HMI_ALARM_CLASS)
                {
                    return new() { DefaultValue = tabConfig.DefaultHmiAlarmClass, Value = alarmData.HmiAlarmClass };
                }

                return null;
            };
            #endregion

            #region ENABLE_CHECKBOX_IF_FILLED
            gridHandler.Events.CellChange += (sender, args) =>
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
                            gridHandler.DataSource[cellChange.RowIndex].Enable = true;
                        }
                        else if (cellChange.IsOldValueFullString() && cellChange.IsNewValueEmptyString())
                        {
                            gridHandler.DataSource[cellChange.RowIndex].Enable = false;
                        }
                    }
                }
            };
            #endregion

        }
        public void Refresh()
        {
            this.gridHandler.Refresh();
        }

        public DataGridView GetDataGridView()
        {
            return gridHandler.DataGridView;
        }

        public void AddScriptVariable(GridScriptVariable scriptVariable)
        {
            gridHandler.ScriptVariableList.Add(scriptVariable);
        }

        public GridSave<AlarmData> CreateSave() => gridHandler.CreateSave();
        public void LoadSave(GridSave<AlarmData> gridSave) => gridHandler.LoadSave(gridSave);

        public void ShowCustomVar(bool show)
        {
            if (!show)
            {
                gridHandler.HideColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS);
                gridHandler.HideColumn(AlarmData.CUSTOM_VARIABLE_VALUE);
            }
            else
            {
                gridHandler.ShowColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS);
                gridHandler.ShowColumn(AlarmData.CUSTOM_VARIABLE_VALUE);
            }

            gridHandler.InitColumns();
        }

        public void ShowTimer(bool show)
        {
            if (!show)
            {
                gridHandler.HideColumn(AlarmData.TIMER_ADDRESS);
                gridHandler.HideColumn(AlarmData.TIMER_TYPE);
                gridHandler.HideColumn(AlarmData.TIMER_VALUE);
            }
            else
            {
                gridHandler.ShowColumn(AlarmData.TIMER_ADDRESS);
                gridHandler.ShowColumn(AlarmData.TIMER_TYPE);
                gridHandler.ShowColumn(AlarmData.TIMER_VALUE);
            }

            gridHandler.InitColumns();
        }

        public bool IsDirty() => gridHandler.IsDirty();

        public void Wash()
        {
            gridHandler.Wash();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData) => gridHandler.ProcessCmdKey(ref msg, keyData);
    }
}
