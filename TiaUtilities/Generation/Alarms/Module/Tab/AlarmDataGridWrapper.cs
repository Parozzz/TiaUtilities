using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaXmlReader;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public class AlarmDataGridWrapper : ICleanable
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];
        private static readonly string[] ALARM_COIL_TYPE_ITEMS = Enum.GetNames(typeof(AlarmCoilType));

        private readonly GridDataPreviewer<AlarmData> previewer;
        private readonly GridHandler<AlarmData> gridHandler;

        public List<AlarmData> AlarmDataList { get => new(this.gridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        public AlarmDataGridWrapper(GenPlaceholderHandler placeholderHandler, GridBindContainer bindContainer) 
        {
            this.previewer = new();
            this.gridHandler = new(MainForm.Settings.GridSettings, bindContainer, this.previewer, placeholderHandler) { RowCount = 199 };
        }

        public void Init(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig)
        {
            #region DRAG
            this.gridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, this.gridHandler);
            this.gridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, this.gridHandler);
            #endregion

            #region COLUMNS
            this.gridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            this.gridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 200);
            this.gridHandler.AddTextBoxColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS, 145);
            this.gridHandler.AddTextBoxColumn(AlarmData.CUSTOM_VARIABLE_VALUE, 50);
            this.gridHandler.AddTextBoxColumn(AlarmData.COIL1_ADDRESS, 145);
            this.gridHandler.AddComboBoxColumn(AlarmData.COIL1_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.AddTextBoxColumn(AlarmData.COIL2_ADDRESS, 145);
            this.gridHandler.AddComboBoxColumn(AlarmData.COIL2_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 95);
            this.gridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            this.gridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 50);
            this.gridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            this.gridHandler.Init();

            #region HIDE_CUSTOM_VARIABLE/TIMER_COLUMNS
            this.ShowCustomVar(mainConfig.EnableCustomVariable);
            mainConfig.Subscribe(() => mainConfig.EnableCustomVariable, this.ShowCustomVar);

            this.ShowTimer(mainConfig.EnableTimer);
            mainConfig.Subscribe(() => mainConfig.EnableTimer, this.ShowTimer);
            #endregion

            #region PREVIEW
            this.previewer.Function = (column, alarmData) =>
            {
                if (string.IsNullOrEmpty(alarmData.AlarmVariable) || alarmData.IsEmpty())
                {
                    return null;
                }

                if (column == AlarmData.ALARM_VARIABLE)
                {
                    return new() { Prefix = tabConfig.AlarmAddressPrefix, Value = alarmData.AlarmVariable };
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

                return null;
            };
            #endregion

            #region ENABLE_CHECKBOX_IF_FILLED
            this.gridHandler.Events.CellChange += (sender, args) =>
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
                            this.gridHandler.DataSource[cellChange.RowIndex].Enable = true;
                        }
                        else if (cellChange.IsOldValueFullString() && cellChange.IsNewValueEmptyString())
                        {
                            this.gridHandler.DataSource[cellChange.RowIndex].Enable = false;
                        }
                    }
                }
            };
            #endregion

        }

        public DataGridView GetDataGridView()
        {
            return gridHandler.DataGridView;
        }

        public void AddScriptVariable(GridScriptVariable scriptVariable)
        {
            this.gridHandler.ScriptVariableList.Add(scriptVariable);
        }

        public GridSave<AlarmData> CreateSave() => this.gridHandler.CreateSave();
        public void LoadSave(GridSave<AlarmData> gridSave) => this.gridHandler.LoadSave(gridSave);

        public void ShowCustomVar(bool show)
        {
            if (!show)
            {
                this.gridHandler.HideColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS);
                this.gridHandler.HideColumn(AlarmData.CUSTOM_VARIABLE_VALUE);
            }
            else
            {
                this.gridHandler.ShowColumn(AlarmData.CUSTOM_VARIABLE_ADDRESS);
                this.gridHandler.ShowColumn(AlarmData.CUSTOM_VARIABLE_VALUE);
            }

            this.gridHandler.InitColumns();
        }

        public void ShowTimer(bool show)
        {
            if (!show)
            {
                this.gridHandler.HideColumn(AlarmData.TIMER_ADDRESS);
                this.gridHandler.HideColumn(AlarmData.TIMER_TYPE);
                this.gridHandler.HideColumn(AlarmData.TIMER_VALUE);
            }
            else
            {
                this.gridHandler.ShowColumn(AlarmData.TIMER_ADDRESS);
                this.gridHandler.ShowColumn(AlarmData.TIMER_TYPE);
                this.gridHandler.ShowColumn(AlarmData.TIMER_VALUE);
            }

            this.gridHandler.InitColumns();
        }

        public bool IsDirty() => this.gridHandler.IsDirty();

        public void Wash()
        {
            gridHandler.Wash();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData) => this.gridHandler.ProcessCmdKey(ref msg, keyData);
    }
}
