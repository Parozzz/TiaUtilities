using TiaUtilities.Generation.Alarms.Configurations;
using TiaUtilities.Generation.Alarms.Data;
using TiaUtilities.Generation.Alarms.Module;
using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.Resources;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.Alarms.Template
{
    public partial class AlarmGenTemplateControl : UserControl
    {
        private static readonly string[] TIMERS_TYPES_ITEMS = ["TON", "TOF"];
        private static readonly string[] ALARM_COIL_TYPE_ITEMS = Enum.GetNames(typeof(AlarmCoilType));

        private readonly AlarmMainConfiguration mainConfig;
        private readonly AlarmTabConfiguration tabConfig;

        private readonly AlarmGenTemplateContainer templateContainer;
        private AlarmGenTemplate? SelectedTemplate { get => this.templateContainer.SelectedTemplate; set => this.templateContainer.SelectedTemplate = value; }

        private readonly GridDataPreviewer<TemplateData> previewer;
        private readonly GridHandler<TemplateData> gridHandler;

        public AlarmGenTemplateControl(AlarmMainConfiguration mainConfig, AlarmTabConfiguration tabConfig,
            MultiGridOperationHandler multiGrid, AlarmGenTemplateContainer templateContainer)
        {
            InitializeComponent();

            this.mainConfig = mainConfig;
            this.tabConfig = tabConfig;

            AlarmGenPlaceholdersHandler placeholdersHandler = new(mainConfig, tabConfig);
            //this.templateDataGridWrapper = new(placeholdersHandler, multiGrid);

            this.templateContainer = templateContainer;

            this.previewer = new();
            this.gridHandler = new(MainForm.Settings.GridSettings, multiGrid, previewer, placeholdersHandler)
            {
                InitializeRowCount = AlarmGenModule.TEMPLATE_GRID_ROW_COUNT
            };
        }

        public void Init()
        {
            #region GRID

            #region GRID_EXCEL_DRAG
            this.gridHandler.ExcelDragPreview += (sender, args) => GridUtils.DragPreview(args, gridHandler);
            this.gridHandler.ExcelDragDone += (sender, args) => GridUtils.DragDone(args, gridHandler);
            #endregion

            #region GRID_COLUMNS
            this.gridHandler.Columns.AddCheckBox(TemplateData.ENABLE, 40);
            this.gridHandler.Columns.AddTextBox(TemplateData.ALARM_VARIABLE, 200);
            this.gridHandler.Columns.AddCheckBox(TemplateData.ALARM_NEGATED, 55);
            this.gridHandler.Columns.AddTextBox(TemplateData.CUSTOM_VARIABLE_ADDRESS, 145);
            this.gridHandler.Columns.AddTextBox(TemplateData.CUSTOM_VARIABLE_VALUE, 50);
            this.gridHandler.Columns.AddTextBox(TemplateData.COIL1_ADDRESS, 145);
            this.gridHandler.Columns.AddComboBox(TemplateData.COIL1_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.Columns.AddTextBox(TemplateData.COIL2_ADDRESS, 145);
            this.gridHandler.Columns.AddComboBox(TemplateData.COIL2_TYPE, 65, ALARM_COIL_TYPE_ITEMS);
            this.gridHandler.Columns.AddTextBox(TemplateData.TIMER_ADDRESS, 95);
            this.gridHandler.Columns.AddComboBox(TemplateData.TIMER_TYPE, 55, TIMERS_TYPES_ITEMS);
            this.gridHandler.Columns.AddTextBox(TemplateData.TIMER_VALUE, 50);
            this.gridHandler.Columns.AddTextBox(TemplateData.HMI_ALARM_CLASS, 150);

            var hmiParametersColumn = this.gridHandler.Columns.AddButton(TemplateData.HMI_PARAMETERS, 150);
            hmiParametersColumn.ButtonPressed += (sender, args) =>
            {
                var cell = args.Cell;
                var rowIndex = args.Cell.RowIndex;
                var columnIndex = args.Cell.ColumnIndex;

                var gridForm = this.gridHandler.FindForm();
                if (gridForm != null)
                {
                    var location = Cursor.Position;
                    location.Offset(+5, +5);

                    var gridDataString = "" + this.gridHandler.DataSource[rowIndex][columnIndex];

                    var parametersForm = new AlarmGenHmiParametersForm(gridDataString) { Location = location };
                    parametersForm.FormClosed += (sender, args) =>
                    {
                        var jsonString = parametersForm.GetJsonSerializedItems();
                        cell.Value = jsonString;
                    };
                    parametersForm.Show(gridForm);
                }
            };

            this.gridHandler.Columns.AddTextBox(TemplateData.HMI_ALARM_TEXT, 250);
            this.gridHandler.Columns.AddTextBox(TemplateData.DESCRIPTION, 0);
            #endregion

            this.gridHandler.Init();

            #region HIDE_CUSTOM_VARIABLE/TIMER_COLUMNS        

            void ShowCustomVar(bool show)
            {
                if (!show)
                {
                    this.gridHandler.Columns.Hide(TemplateData.CUSTOM_VARIABLE_ADDRESS);
                    this.gridHandler.Columns.Hide(TemplateData.CUSTOM_VARIABLE_VALUE);
                }
                else
                {
                    this.gridHandler.Columns.Show(TemplateData.CUSTOM_VARIABLE_ADDRESS);
                    this.gridHandler.Columns.Show(TemplateData.CUSTOM_VARIABLE_VALUE);
                }
            }

            ShowCustomVar(mainConfig.EnableCustomVariable);
            mainConfig.Subscribe(() => mainConfig.EnableCustomVariable, ShowCustomVar);

            void ShowTimer(bool show)
            {
                if (!show)
                {
                    this.gridHandler.Columns.Hide(TemplateData.TIMER_ADDRESS);
                    this.gridHandler.Columns.Hide(TemplateData.TIMER_TYPE);
                    this.gridHandler.Columns.Hide(TemplateData.TIMER_VALUE);
                }
                else
                {
                    this.gridHandler.Columns.Show(TemplateData.TIMER_ADDRESS);
                    this.gridHandler.Columns.Show(TemplateData.TIMER_TYPE);
                    this.gridHandler.Columns.Show(TemplateData.TIMER_VALUE);
                }
            }

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

                var templateConfig = this.SelectedTemplate?.TemplateConfig ?? new();

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

            bool IsObjectStringEmpty(object? obj) => obj == null || (obj is string str && string.IsNullOrWhiteSpace(str));
            bool IsObjectStringFull(object? obj) => obj != null && obj is string str && !string.IsNullOrWhiteSpace(str);

            this.gridHandler.DataChanged += (sender, args) =>
            {
                foreach (var changedCellData in args.ChangedCellDataList)
                {
                    if (changedCellData.Column == TemplateData.ALARM_VARIABLE)
                    {//If an alarm variable is filled (Before empty and now full) i will automatically set the enable to be true. The opposite removes the enable. QOL
                        if (IsObjectStringEmpty(changedCellData.OldValue) && IsObjectStringFull(changedCellData.NewValue))
                        {
                            gridHandler.DataSource[changedCellData.RowIndex].Enable = true;
                        }
                        else if (IsObjectStringFull(changedCellData.OldValue) && IsObjectStringEmpty(changedCellData.NewValue))
                        {
                            gridHandler.DataSource[changedCellData.RowIndex].Enable = false;
                        }

                        this.gridHandler.ViewManipulator.RefreshRow(changedCellData.RowIndex);
                    }
                    else if (changedCellData.Column == TemplateData.DESCRIPTION)
                    {
                        this.gridHandler.ViewManipulator.RefreshRow(changedCellData.RowIndex);
                    }
                }
            };
            #endregion

            #endregion

            this.gridHandler.DataSource.ListChanged += (sender, args) =>
            {
                if (sender is IList<TemplateData> templates)
                {
                    switch (args.ListChangedType)
                    {
                        case System.ComponentModel.ListChangedType.ItemChanged:
                            if (this.SelectedTemplate == null || args.PropertyDescriptor == null)
                            {
                                return;
                            }

                            try
                            {
                                templates.TryGet(args.NewIndex, out var changedTemplateData);

                                TemplateData data;
                                if (this.SelectedTemplate.AlarmGridSave.RowData.TryGetValue(args.NewIndex, out var existingTemplateData))
                                {
                                    data = existingTemplateData;
                                }
                                else
                                {
                                    data = new();
                                    this.SelectedTemplate.AlarmGridSave.RowData.Add(args.NewIndex, data);
                                }

                                var value = args.PropertyDescriptor.GetValue(changedTemplateData);
                                args.PropertyDescriptor.SetValue(data, value);
                            }
                            catch (Exception ex)
                            {
                                Utils.ShowExceptionMessage(ex);
                            }

                            break;
                        case System.ComponentModel.ListChangedType.ItemAdded:
                        case System.ComponentModel.ListChangedType.ItemDeleted:
                        case System.ComponentModel.ListChangedType.ItemMoved:
                            break;
                        case System.ComponentModel.ListChangedType.PropertyDescriptorAdded:
                        case System.ComponentModel.ListChangedType.PropertyDescriptorChanged:
                        case System.ComponentModel.ListChangedType.PropertyDescriptorDeleted:
                            break;
                        case System.ComponentModel.ListChangedType.Reset:
                            break;
                    }
                }
            };

            this.mainPanel.Controls.Add(this.gridHandler.GetControl());

            #region ADD_REMOVE_RENAME_CLOSE_BUTTONS
            ImageList buttonsImages = new()
            {
                Images = { 
                    ImageResources.ADD_501366_007435, 
                    ImageResources.DELETE, 
                    ImageResources.RENAME, 
                    ImageResources.DUPLICATE 
                },
                ImageSize = new(18, 18),
            };

            this.addButton.ImageList = buttonsImages;
            this.addButton.ImageIndex = 0;
            this.addButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.addButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.addButton.Click += (sender, args) => this.templateContainer.Add();

            this.removeButton.ImageList = buttonsImages;
            this.removeButton.ImageIndex = 1;
            this.removeButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.removeButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.removeButton.Click += (sender, args) => this.templateContainer.RemoveSelectedTemplate();

            this.renameButton.ImageList = buttonsImages;
            this.renameButton.ImageIndex = 2;
            this.renameButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.renameButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.renameButton.Click += (sender, args) => this.templateContainer.RenameSelectedTemplate(this);

            this.cloneButton.ImageList = buttonsImages;
            this.cloneButton.ImageIndex = 3;
            this.cloneButton.ImageAlign = ContentAlignment.MiddleLeft;
            this.cloneButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.cloneButton.Click += (sender, args) => this.templateContainer.CloneSelectedTemplate();
            #endregion

            #region SELECT_COMBO_BOX
            this.selectComboBox.BackColor = Form.DefaultBackColor;
            this.selectComboBox.AutoWidthFromItems = true;
            this.selectComboBox.DisplayMember = this.selectComboBox.ValueMember = nameof(AlarmGenTemplate.Name);
            this.selectComboBox.DataSource = this.templateContainer.ReadOnlyBindingList;
            this.selectComboBox.SelectionChangeCommitted += (sender, args) =>
            {
                if (this.selectComboBox.SelectedItem is AlarmGenTemplate template)
                {
                    this.templateContainer.SelectedTemplate = template;
                }
            };
            #endregion

            //Load Current after init and save current when form is closed.
            this.templateContainer.SelectedChanged += (sender, args) => this.SelectedTemplateChanged();
            this.SelectedTemplateChanged();

            this.Translate();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        private void SelectedTemplateChanged()
        {
            if (this.SelectedTemplate == null)
            {
                return;
            }

            this.gridHandler.LoadSave(this.SelectedTemplate.AlarmGridSave);
            this.gridHandler.Wash();

            this.selectComboBox.SelectedItem = this.SelectedTemplate;
        }

        private void Translate()
        {
            this.Text = Locale.ALARM_TEMPLATE_FORM;
            this.selectLabel.Text = Locale.ALARM_TEMPLATE_SELECT_TEMPLATE;

            this.addButton.Text = "Add";
            this.removeButton.Text = "Remove";
            this.renameButton.Text = "Rename";
            this.cloneButton.Text = "Clone";
        }
    }
}
