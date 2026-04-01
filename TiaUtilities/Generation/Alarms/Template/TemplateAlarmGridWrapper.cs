using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Binds;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms.Template
{
    public class TemplateAlarmGridWrapper : ICleanable
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];
        private static readonly string[] ALARM_COIL_TYPE_ITEMS = Enum.GetNames(typeof(AlarmCoilType));

        private readonly GridDataPreviewer<TemplateData> previewer;
        private readonly GridHandler<TemplateData> gridHandler;

        public List<TemplateData> TemplateDataList { get => new(gridHandler.DataSource.GetNotEmptyClonedDataDict().Keys); } //Return CLONED data, otherwise operations on the xml generation will affect the table!

        public TemplateAlarmGridWrapper(GenPlaceholderHandler placeholderHandler, GridBindContainer bindContainer)
        {
            this.previewer = new();
            this.gridHandler = new(MainForm.Settings.GridSettings, bindContainer, previewer, placeholderHandler) { RowCount = AlarmGenModule.TEMPLATE_GRID_ROW_COUNT };
        }

        public void Init(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig, Func<AlarmTemplateConfiguration> getTemplateConfig)
        {
            #region DRAG
            this.gridHandler.Events.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, gridHandler);
            this.gridHandler.Events.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, gridHandler);
            #endregion

            #region COLUMNS
            this.gridHandler.AddCheckBoxColumn(TemplateData.ENABLE, 40);
            this.gridHandler.AddTextBoxColumn(TemplateData.ALARM_VARIABLE, 200);
            this.gridHandler.AddCheckBoxColumn(TemplateData.ALARM_NEGATED, 55);
            this.gridHandler.AddTextBoxColumn(TemplateData.CUSTOM_VARIABLE_ADDRESS, 145);
            this.gridHandler.AddTextBoxColumn(TemplateData.CUSTOM_VARIABLE_VALUE, 50);
            this.gridHandler.AddTextBoxColumn(TemplateData.COIL1_ADDRESS, 145);
            this.gridHandler.AddComboBoxColumn(TemplateData.COIL1_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.AddTextBoxColumn(TemplateData.COIL2_ADDRESS, 145);
            this.gridHandler.AddComboBoxColumn(TemplateData.COIL2_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.AddTextBoxColumn(TemplateData.TIMER_ADDRESS, 95);
            this.gridHandler.AddComboBoxColumn(TemplateData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            this.gridHandler.AddTextBoxColumn(TemplateData.TIMER_VALUE, 50);
            this.gridHandler.AddTextBoxColumn(TemplateData.HMI_ALARM_CLASS, 150);

            var hmiParametersColumn = this.gridHandler.AddButtonColumn(TemplateData.HMI_PARAMETERS, 150);
            hmiParametersColumn.ButtonDoublePressed += (sender, args) =>
            {
                var cell = args.Cell;
                var rowIndex = args.Cell.RowIndex;
                var columnIndex = args.Cell.ColumnIndex;

                var gridForm = this.gridHandler.DataGridView.FindForm();
                if(gridForm != null)
                {
                    var location = Cursor.Position;
                    location.Offset(+5, +5);

                    var gridDataString = "" + this.gridHandler.DataSource[rowIndex][columnIndex];

                    var parametersForm = new AlarmGenHmiParametersForm(gridDataString) { Location = location };
                    parametersForm.FormClosed += (sender, args) =>
                    {
                        var oldValue = gridDataString;
                        var newValue = parametersForm.GetJsonSerializedItems();

                        this.gridHandler.ChangeCell(new(cell) { OldValue = oldValue, NewValue = newValue });
                    };
                    parametersForm.Show(gridForm);
                }
            };

            this.gridHandler.AddTextBoxColumn(TemplateData.HMI_ALARM_TEXT, 250);
            this.gridHandler.AddTextBoxColumn(TemplateData.DESCRIPTION, 0);
            #endregion

            this.gridHandler.Init();

            #region HIDE_CUSTOM_VARIABLE/TIMER_COLUMNS
            ShowCustomVar(mainConfig.EnableCustomVariable);
            mainConfig.Subscribe(() => mainConfig.EnableCustomVariable, ShowCustomVar);

            ShowTimer(mainConfig.EnableTimer);
            mainConfig.Subscribe(() => mainConfig.EnableTimer, ShowTimer);
            #endregion

            #region PREVIEW
            this.previewer.Function = (column, templateData) =>
            {
                if (string.IsNullOrEmpty(templateData.AlarmVariable) || templateData.IsEmpty())
                {
                    return null;
                }

                var templateConfig = getTemplateConfig();

                if (column == TemplateData.ALARM_VARIABLE)
                {
                    var prefix = templateConfig.StandaloneAlarms ? "" : tabConfig.AlarmAddressPrefix;
                    return new() { Prefix = prefix, Value = templateData.AlarmVariable };
                }
                else if (column == TemplateData.CUSTOM_VARIABLE_ADDRESS)
                {
                    return new() { DefaultValue = tabConfig.DefaultCustomVarAddress, Value = templateData.CustomVariableAddress };
                }
                else if (column == TemplateData.CUSTOM_VARIABLE_VALUE)
                {
                    return new() { DefaultValue = tabConfig.DefaultCustomVarValue, Value = templateData.CustomVariableValue };
                }
                else if (column == TemplateData.COIL1_ADDRESS)
                {
                    return new() { Prefix = tabConfig.Coil1AddressPrefix, DefaultValue = tabConfig.DefaultCoil1Address, Value = templateData.Coil1Address };
                }
                else if (column == TemplateData.COIL1_TYPE && TemplateData.IsAddressValid(templateData.Coil1Address))
                {
                    return new() { DefaultValue = tabConfig.DefaultCoil1Type.ToString(), Value = templateData.Coil1Type };
                }
                else if (column == TemplateData.COIL2_ADDRESS)
                {
                    return new() { Prefix = tabConfig.Coil2AddressPrefix, DefaultValue = tabConfig.DefaultCoil2Address, Value = templateData.Coil2Address };
                }
                else if (column == TemplateData.COIL2_TYPE && TemplateData.IsAddressValid(templateData.Coil2Address))
                {
                    return new() { DefaultValue = tabConfig.DefaultCoil2Type.ToString(), Value = templateData.Coil2Type };
                }
                else if (column == TemplateData.TIMER_ADDRESS)
                {
                    return new() { Prefix = tabConfig.TimerAddressPrefix, DefaultValue = tabConfig.DefaultTimerAddress, Value = templateData.TimerAddress };
                }
                else if (string.IsNullOrEmpty(templateData.TimerAddress) ? TemplateData.IsAddressValid(tabConfig.DefaultTimerAddress) : TemplateData.IsAddressValid(templateData.TimerAddress))
                {
                    if (column == TemplateData.TIMER_TYPE)
                    {
                        return new() { DefaultValue = tabConfig.DefaultTimerType, Value = templateData.TimerType };
                    }
                    else if (column == TemplateData.TIMER_VALUE)
                    {
                        return new() { DefaultValue = tabConfig.DefaultTimerValue, Value = templateData.TimerValue };
                    }
                }
                else if (column == TemplateData.HMI_ALARM_CLASS)
                {
                    return new() { DefaultValue = tabConfig.DefaultHmiAlarmClass, Value = templateData.HmiAlarmClass };
                }
                else if (column == TemplateData.HMI_ALARM_TEXT)
                {
                    return new() { DefaultValue = templateData.Description, Value = templateData.HmiAlarmText };
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
                    if (TemplateData.ALARM_VARIABLE == cellChange.ColumnIndex)
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

        public GridSave<TemplateData> CreateSave() => gridHandler.CreateSave();
        public void LoadSave(GridSave<TemplateData> gridSave) => gridHandler.LoadSave(gridSave);

        public void ShowCustomVar(bool show)
        {
            if (!show)
            {
                gridHandler.HideColumn(TemplateData.CUSTOM_VARIABLE_ADDRESS);
                gridHandler.HideColumn(TemplateData.CUSTOM_VARIABLE_VALUE);
            }
            else
            {
                gridHandler.ShowColumn(TemplateData.CUSTOM_VARIABLE_ADDRESS);
                gridHandler.ShowColumn(TemplateData.CUSTOM_VARIABLE_VALUE);
            }

            gridHandler.InitColumns();
        }

        public void ShowTimer(bool show)
        {
            if (!show)
            {
                gridHandler.HideColumn(TemplateData.TIMER_ADDRESS);
                gridHandler.HideColumn(TemplateData.TIMER_TYPE);
                gridHandler.HideColumn(TemplateData.TIMER_VALUE);
            }
            else
            {
                gridHandler.ShowColumn(TemplateData.TIMER_ADDRESS);
                gridHandler.ShowColumn(TemplateData.TIMER_TYPE);
                gridHandler.ShowColumn(TemplateData.TIMER_VALUE);
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
