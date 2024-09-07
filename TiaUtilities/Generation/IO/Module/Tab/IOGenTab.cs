using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.IO;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Javascript;
using TiaUtilities.Generation.GenModules.IO.Tab;
using TiaUtilities.Generation.IO.Module;
using SimaticML.Enums;
using SimaticML;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.IO.Module.Tab
{
    public class IOGenTab : ICleanable
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly IOGenModule ioGenProject;
        public TabPage TabPage { get; init; }
        private readonly IOMainConfiguration mainConfig;
        public GridDataPreviewer<IOData> Previewer { get; init; }

        public IOTabConfiguration TabConfig { get; init; }
        private readonly SuggestionTextBoxColumn variableAddressColumn;

        public GridHandler<IOData> GridHandler { get; init; }

        public IOGenTabControl TabControl { get; init; }


        public IOGenTab(JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings, GridScriptContainer scriptContainer,
                IOGenModule ioGenProject, TabPage tabPage, IOMainConfiguration mainConfig)
        {
            this.ioGenProject = ioGenProject;
            this.TabPage = tabPage;
            this.mainConfig = mainConfig;

            this.Previewer = new();
            this.TabConfig = new();
            this.variableAddressColumn = new();

            IOGenPlaceholderHandler placeholdersHandler = new(this.Previewer, this.mainConfig, TabConfig);
            this.GridHandler = new(jsErrorHandlingThread, gridSettings, scriptContainer, this.Previewer, placeholdersHandler, new IOGenComparer()) { RowCount = 2999 };

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

            GridHandler.AddTextBoxColumn(IOData.IO_NAME, 110);
            GridHandler.AddCustomColumn(variableAddressColumn, IOData.VARIABLE, 200);
            variableAddressColumn.SetGetItemsFunc(() =>
            {
                var ioVariableEnumerable = GridHandler.DataSource.GetNotEmptyDataDict().Keys.Select(i => i.Variable);
                var suggestionEnumerable = ioGenProject.Suggestions;

                var notAddedSuggestionEnumerable = suggestionEnumerable.Select(k => k.Value).Except(ioVariableEnumerable).ToArray();
                return notAddedSuggestionEnumerable ?? [];
            });

            GridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            GridHandler.AddTextBoxColumn(IOData.COMMENT, 0);

            mainConfig.Subscribe(() => mainConfig.MemoryType, UpdateMerkerColumn);
            UpdateMerkerColumn(mainConfig.MemoryType);
            #endregion

            TabControl.Init();

            GridHandler.Init();
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
                    ioGenProject.UpdateSuggestionColors();
                }

                if (args.CellChangeList.Where(c => c.ColumnIndex == IOData.ADDRESS || c.ColumnIndex == IOData.IO_NAME).Any())
                {
                    UpdateDuplicatedIOValues();
                }
            };
            GridHandler.Events.PostSort += (sender, args) =>
            {
                ioGenProject.UpdateSuggestionColors();
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

            #region JS_SCRIPT_EVENTS
            GridHandler.Events.ScriptShowVariable += (sender, args) => args.VariableList.Add("suggestions [array]");
            GridHandler.Events.ScriptAddVariables += (sender, args) => args.VariableDict.Add("suggestions", ioGenProject.Suggestions.Select(s => s.Value).ToArray());
            #endregion
        }

        public bool IsDirty() => TabConfig.IsDirty() || GridHandler.IsDirty();
        public void Wash()
        {
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
