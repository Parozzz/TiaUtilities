using SimaticML;
using SimaticML.Enums;
using TiaUtilities.Generation.GenModules.IO.Tab;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.Placeholders;
using TiaXmlReader;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.IO.Module.Tab
{
    public class IOGenTab : ICleanable
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly IOGenModule module;
        private readonly GridScript gridScript;

        private readonly IOMainConfiguration mainConfig;
        private readonly SuggestionTextBoxColumn variableAddressColumn;

        public TabPage TabPage { get; init; }

        public IOTabConfiguration TabConfig { get; init; }

        public GridDataPreviewer<IOData> Previewer { get; init; }
        public GridHandler<IOData> GridHandler { get; init; }

        public IOGenTabControl TabControl { get; init; }

        private bool dirty = false;

        public IOGenTab(GridSettings gridSettings, GridScript gridScript, GridFindForm findForm, IOGenModule module, TabPage tabPage, IOMainConfiguration mainConfig)
        {
            this.module = module;
            this.gridScript = gridScript;
            this.TabPage = tabPage;

            this.mainConfig = mainConfig;
            this.variableAddressColumn = new();

            this.TabConfig = new();
            GenUtils.CopyJsonFieldsAndProperties(MainForm.Settings.PresetIOTabConfiguration, this.TabConfig);

            this.Previewer = new();

            IOGenPlaceholderHandler placeholdersHandler = new(this.Previewer, this.mainConfig, TabConfig);
            this.GridHandler = new(gridSettings, gridScript, findForm, this.Previewer, placeholdersHandler, new IOGenComparer()) { RowCount = 2999 };

            this.TabControl = new(this.GridHandler.DataGridView);
        }

        public void Init()
        {
            #region DRAG
            GridHandler.Events.ExcelDragPreview += (sender, args) => IOGenUtils.DragPreview(args, GridHandler);
            GridHandler.Events.ExcelDragDone += (sender, args) => IOGenUtils.DragDone(args, GridHandler);
            #endregion

            //Column initialization before gridHandler.Init()
            #region COLUMNS
            var addressColumn = GridHandler.AddTextBoxColumn(IOData.ADDRESS, 65);
            addressColumn.MaxInputLength = 10;

            this.GridHandler.AddCheckBoxColumn(IOData.NEGATED, 50);

            this.GridHandler.AddTextBoxColumn(IOData.IO_NAME, 110);
            this.GridHandler.AddCustomColumn(variableAddressColumn, IOData.VARIABLE, 200);
            variableAddressColumn.SetGetItemsFunc(() =>
            {
                var ioVariableEnumerable = GridHandler.DataSource.GetNotEmptyDataDict().Keys.Select(i => i.Variable);
                var suggestionEnumerable = module.Suggestions;

                var notAddedSuggestionEnumerable = suggestionEnumerable.Select(k => k.Value).Except(ioVariableEnumerable).ToArray();
                return notAddedSuggestionEnumerable;
            });

            this.GridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.GridHandler.AddTextBoxColumn(IOData.COMMENT, 0);

            mainConfig.Subscribe(() => mainConfig.MemoryType, UpdateMerkerColumn);
            UpdateMerkerColumn(mainConfig.MemoryType);
            #endregion

            GridHandler.Init();

            TabControl.Init();
            TabControl.BindConfig(TabConfig);

            #region SUGGESTION_GRIDS_EVENTS
            GridHandler.Events.CellChange += (sender, args) =>
            {
                if (args.CellChangeList == null)
                {
                    return;
                }

                if (args.CellChangeList.Where(c => c.ColumnIndex == IOData.VARIABLE).Any())
                {
                    module.UpdateSuggestionColors();
                }

                if (args.CellChangeList.Where(c => c.ColumnIndex == IOData.ADDRESS || c.ColumnIndex == IOData.IO_NAME).Any())
                {
                    UpdateDuplicatedIOValues();
                }
            };
            GridHandler.Events.PostSort += (sender, args) =>
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
            this.GridHandler.ScriptVariableList.Add(GridScriptVariable.ReadOnlyValue("tabName", () => this.TabPage.Text));
            this.GridHandler.ScriptVariableList.Add(GridScriptVariable.ReadOnlyValue("suggestions", () => this.module.Suggestions.Select(s => s.Value).ToArray()));
            #endregion

            #region DIRTY
            this.TabPage.TextChanged += (sender, args) => dirty = true;
            #endregion
        }

        public void Selected()
        {
            this.gridScript.BindHandler(this.GridHandler);
        }

        public bool IsDirty() => this.dirty || TabConfig.IsDirty() || GridHandler.IsDirty();
        public void Wash()
        {
            this.dirty = false;
            TabConfig.Wash();
            GridHandler.Wash();
        }

        private void UpdateMerkerColumn(IOMemoryTypeEnum memoryType)
        {
            GridHandler.ChangeColumnVisibility(IOData.MERKER_ADDRESS, visible: memoryType == IOMemoryTypeEnum.MERKER, init: true);
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

            GridHandler.LoadSave(save.IOGrid);
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, TabConfig);

            UpdateDuplicatedIOValues();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return GridHandler.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateDuplicatedIOValues()
        {
            GridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in GridHandler.DataGridView.Rows)
            {
                var addressCell = row.Cells[IOData.ADDRESS.ColumnIndex];
                addressCell.ToolTipText = string.Empty;
                addressCell.Style.BackColor = SystemColors.ControlLightLight;
                addressCell.Style.SelectionBackColor = Color.LightGray;

                var ioNameCell = row.Cells[IOData.IO_NAME.ColumnIndex];
                ioNameCell.ToolTipText = string.Empty;
                ioNameCell.Style.BackColor = SystemColors.ControlLightLight;
                ioNameCell.Style.SelectionBackColor = Color.LightGray;
            }

            var dataDict = GridHandler.DataSource.GetNotEmptyDataDict();

            var multipleIONameGroupingList = dataDict.Where(x => !string.IsNullOrEmpty(x.Key.IOName)).GroupBy(x => x.Key.IOName).Where(g => g.Count() > 1).ToList();
            foreach (var grouping in multipleIONameGroupingList)
            {
                //+1 to row because rows index start from 0.
                var tooltipText = grouping
                    .Select(x => $"{x.Value + 1}) {x.Key.Address}-{x.Key.Comment}")
                    .Aggregate((a, b) => a + Environment.NewLine + b);

                foreach (var entry in grouping)
                {
                    var rowIndex = entry.Value;

                    var ioNameCell = GridHandler.DataGridView.Rows[rowIndex].Cells[IOData.IO_NAME.ColumnIndex];
                    ioNameCell.ToolTipText = tooltipText;
                    ioNameCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
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

                    var addressCell = GridHandler.DataGridView.Rows[rowIndex].Cells[IOData.ADDRESS.ColumnIndex];
                    addressCell.ToolTipText = tooltipText;
                    addressCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                }
            }

            GridHandler.DataGridView.ResumeLayout();
        }
    }
}
