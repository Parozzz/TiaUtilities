using SimaticML;
using SimaticML.Enums;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.GridHandler.CustomColumns.SuggestionColumn;
using TiaUtilities.JSScript;

namespace TiaUtilities.Generation.IO.Module.Tab
{
    public class IOGenTab : ICleanable, ISaveable<IOGenTabSave>
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly IOGenModule module;
        private readonly MultiGridOperationHandler multiGrid;
        private readonly IOMainConfiguration mainConfig;

        public TabPage TabPage { get; init; }
        public string Name { get => this.TabPage.Text; set => this.TabPage.Text = value; }

        public IOTabConfiguration TabConfig { get; init; }

        public GridDataPreviewer<IOData> Previewer { get; init; }
        public GridHandler<IOData> GridHandler { get; init; }

        private bool dirty = false;

        public IOGenTab(GridSettings gridSettings, MultiGridOperationHandler multiGrid, IOGenModule module, TabPage tabPage, IOMainConfiguration mainConfig)
        {
            this.module = module;
            this.multiGrid = multiGrid;
            this.mainConfig = mainConfig;

            this.TabPage = tabPage;

            this.TabConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOTabConfiguration, this.TabConfig);

            this.Previewer = new();

            IOGenPlaceholderHandler placeholdersHandler = new(this.Previewer, this.mainConfig, TabConfig);
            this.GridHandler = new(gridSettings, multiGrid, this.Previewer, placeholdersHandler, new IOGenComparer()) { InitializeRowCount = 2999 };
        }

        public void Init()
        {
            #region DRAG
            GridHandler.ExcelDragPreview += (sender, args) => IOGenUtils.DragPreview(args, GridHandler);
            GridHandler.ExcelDragDone += (sender, args) => IOGenUtils.DragDone(args, GridHandler);
            #endregion

            //Column initialization before gridHandler.Init()
            #region COLUMNS
            var addressColumn = this.GridHandler.Columns.AddTextBox(IOData.ADDRESS, 65);
            addressColumn.MaxInputLength = 10;

            this.GridHandler.Columns.AddCheckBox(IOData.NEGATED, 50);
            this.GridHandler.Columns.AddTextBox(IOData.IO_NAME, 110);

            var variableAddressColumn = this.GridHandler.Columns.Add(new SuggestionTextBoxColumn(), IOData.VARIABLE, 200);
            variableAddressColumn.SetGetItemsFunc(() => module.GetSuggestions(filterAlreadyUsed: true));

            this.GridHandler.Columns.AddTextBox(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.GridHandler.Columns.AddTextBox(IOData.COMMENT, 0);

            mainConfig.Subscribe(() => mainConfig.MemoryType, UpdateMerkerColumn);
            UpdateMerkerColumn(mainConfig.MemoryType);
            #endregion

            this.GridHandler.Init();

            #region GRID EVENTS - DUPLICATED IO VALUES
            this.GridHandler.DataChanged += (sender, args) =>
            {
                if (args.ChangedCellDataList.Any(c => c.ColumnIndex == IOData.VARIABLE))
                {
                    module.UpdateSuggestionColors();
                }

                if (args.ChangedCellDataList.Any(c => c.ColumnIndex == IOData.ADDRESS || c.ColumnIndex == IOData.IO_NAME))
                {
                    UpdateDuplicatedIOValues();
                }
            };
            this.GridHandler.PostSort += (sender, args) =>
            {
                module.UpdateSuggestionColors();
                UpdateDuplicatedIOValues();
            };
            this.GridHandler.DataLoaded += (sender, args) =>
            {
                module.UpdateSuggestionColors();
                UpdateDuplicatedIOValues();
            };
            #endregion

            #region PREVIEW
            this.Previewer.Function = (column, ioData) =>
            {
                var addressTag = SimaticTagAddress.FromAddress(ioData.Address);
                if (string.IsNullOrEmpty(ioData.Address) || ioData.IsEmpty() || addressTag == null)
                {
                    return null;
                }

                if (column == IOData.IO_NAME)
                {
                    if (string.IsNullOrEmpty(mainConfig.DefaultIoName) && string.IsNullOrEmpty(ioData.IOName))
                    {
                        return null;
                    }

                    return new() { DefaultValue = mainConfig.DefaultIoName, Value = ioData.IOName };
                }
                else if (column == IOData.VARIABLE)
                {
                    string defaultValue = "";
                    if (mainConfig.MemoryType == IOMemoryTypeEnum.DB)
                    {
                        defaultValue = addressTag.MemoryArea == SimaticMemoryArea.INPUT ? mainConfig.DefaultDBInputVariable : mainConfig.DefaultDBOutputVariable;
                    }
                    else if (mainConfig.MemoryType == IOMemoryTypeEnum.MERKER)
                    {
                        defaultValue = addressTag.MemoryArea == SimaticMemoryArea.INPUT ? mainConfig.DefaultMerkerInputVariable : mainConfig.DefaultMerkerOutputVariable;
                    }

                    if (string.IsNullOrEmpty(defaultValue) && string.IsNullOrEmpty(ioData.Variable))
                    {
                        return null;
                    }

                    return new() { DefaultValue = defaultValue, Value = ioData.Variable };
                }
                else if (column == IOData.MERKER_ADDRESS)
                {
                    var merkerTag = new SimaticTagAddress
                    {
                        MemoryArea = SimaticMemoryArea.MERKER,
                        ByteOffset = addressTag.ByteOffset + (addressTag.MemoryArea == SimaticMemoryArea.INPUT ? mainConfig.VariableTableInputStartAddress : mainConfig.VariableTableOutputStartAddress),
                        BitOffset = addressTag.BitOffset,
                        Length = 0 //BIT
                    };

                    return new() { DefaultValue = merkerTag.ToString(), Value = ioData.MerkerAddress };
                }

                return null;
            };
            #endregion

            #region GRID_SCRIPT_CUSTOM_VARIABLES
            this.GridHandler.ScriptVariableList.Add(JSScriptVariable.ReadOnlyValue("tabName", () => this.TabPage.Text));
            this.GridHandler.ScriptVariableList.Add(JSScriptVariable.ReadOnlyValue("suggestions", () => this.module.GetSuggestions(false).ToArray()));
            #endregion

            #region DIRTY
            this.TabPage.TextChanged += (sender, args) => dirty = true;
            #endregion
        }

        public void Selected()
        {
            this.multiGrid.SetActiveGrid(this.GridHandler);
        }

        public bool IsDirty() => this.dirty || this.TabConfig.IsDirty() || this.GridHandler.IsDirty();
        public void Wash()
        {
            this.dirty = false;
            this.TabConfig.Wash();
            this.GridHandler.Wash();
        }

        private void UpdateMerkerColumn(IOMemoryTypeEnum memoryType)
        {
            this.GridHandler.Columns.ChangeVisibility(IOData.MERKER_ADDRESS, visible: memoryType == IOMemoryTypeEnum.MERKER, init: true);
        }

        public IOGenTabSave CreateSave()
        {
            var save = new IOGenTabSave()
            {
                Name = TabPage.Text,
                IOGrid = GridHandler.CreateSave(),
            };

            GenUtils.CopyJsonFieldsAndProperties(TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(IOGenTabSave save)
        {
            TabPage.Text = save.Name;

            this.GridHandler.LoadSave(save.IOGrid);
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, TabConfig);
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return GridHandler.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateDuplicatedIOValues()
        {
            this.GridHandler.ViewManipulator.SuspendLayout();

            for(int rowIndex = 0; rowIndex < this.GridHandler.DataSource.Count; rowIndex++)
            {
                var addressCell = this.GridHandler.ViewManipulator.GetCell(rowIndex, IOData.ADDRESS);
                if (addressCell != null)
                {
                    addressCell.ToolTipText = string.Empty;
                    addressCell.Style.BackColor = SystemColors.ControlLightLight;
                    addressCell.Style.SelectionBackColor = Color.LightGray;
                }

                var ioNameCell = this.GridHandler.ViewManipulator.GetCell(rowIndex, IOData.IO_NAME);
                if (ioNameCell != null)
                {
                    ioNameCell.ToolTipText = string.Empty;
                    ioNameCell.Style.BackColor = SystemColors.ControlLightLight;
                    ioNameCell.Style.SelectionBackColor = Color.LightGray;
                }
            }

            var dataDict = this.GridHandler.DataSource.GetNotEmptyDataDict();

            var multipleIONameGroupingList = dataDict.Where(x => !string.IsNullOrEmpty(x.Key.IOName))
                                                        .GroupBy(x => x.Key.IOName)
                                                        .Where(g => g.Count() > 1)
                                                        .ToList();
            foreach (var grouping in multipleIONameGroupingList)
            {
                //+1 to row because rows index start from 0.
                var tooltipText = grouping
                    .Select(x => $"{x.Value + 1}) {x.Key.Address}-{x.Key.Comment}")
                    .Aggregate((a, b) => a + Environment.NewLine + b);

                foreach (var entry in grouping)
                {
                    var rowIndex = entry.Value;

                    var ioNameCell = this.GridHandler.ViewManipulator.GetCell(rowIndex, IOData.IO_NAME);
                    if(ioNameCell != null)
                    {
                        ioNameCell.ToolTipText = tooltipText;
                        ioNameCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                    }
                }
            }

            var multipleAddressGroupingList = dataDict.Where(x => !string.IsNullOrEmpty(x.Key.Address)).GroupBy(x => x.Key.Address).Where(g => g.Count() > 1).ToList();
            foreach (var grouping in multipleAddressGroupingList)
            {
                //+1 to row because rows index start from 0.
                var tooltipText = grouping
                    .Select(x => $"{x.Value + 1}) {x.Key.Address}-{x.Key.Comment}")
                    .Aggregate((a, b) => a + Environment.NewLine + b);

                foreach (var entry in grouping)
                {
                    var rowIndex = entry.Value;

                    var addressCell = this.GridHandler.ViewManipulator.GetCell(rowIndex, IOData.ADDRESS);
                    if(addressCell != null)
                    {
                        addressCell.ToolTipText = tooltipText;
                        addressCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                    }
                }
            }

            this.GridHandler.ViewManipulator.ResumeLayout(refresh: true);
        }
    }
}
