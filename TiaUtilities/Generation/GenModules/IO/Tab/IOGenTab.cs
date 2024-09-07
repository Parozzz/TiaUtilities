using TiaUtilities.Generation.GridHandler.JSScript;
using TiaUtilities.Generation.IO;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Javascript;
using TiaUtilities.Generation.GenModules.IO;
using TiaUtilities.Generation.GenModules.IO.Tab;

namespace TiaUtilities.Generation.GenModules.IO.Tab
{
    public class IOGenTab : ICleanable
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly IOGenModule ioGenProject;
        public TabPage TabPage { get; init; }
        private readonly IOMainConfiguration mainConfig;

        public GridHandler<IOMainConfiguration, IOData> GridHandler { get; init; }

        private readonly SuggestionTextBoxColumn variableAddressColumn;

        public IOGenTabControl TabControl { get; init; }
        public IOTabConfiguration TabConfig { get; init; }

        public IOGenTab(IOGenModule ioGenProject, TabPage tabPage, IOMainConfiguration mainConfig, JavascriptErrorReportThread jsErrorHandlingThread, 
            GridSettings gridSettings, GridScriptContainer scriptContainer)
        {
            this.ioGenProject = ioGenProject;
            this.TabPage = tabPage;
            this.mainConfig = mainConfig;

            this.GridHandler = new GridHandler<IOMainConfiguration, IOData>(jsErrorHandlingThread, gridSettings, mainConfig, scriptContainer, new IOGenComparer()) { RowCount = 2999 };
            this.variableAddressColumn = new();
            
            this.TabControl = new(GridHandler.DataGridView);
            this.TabConfig = new();
        }

        public void Init()
        {
            #region DRAG
            this.GridHandler.Events.ExcelDragPreview += (sender, args) => IOGenUtils.DragPreview(args, this.GridHandler);
            this.GridHandler.Events.ExcelDragDone += (sender, args) => IOGenUtils.DragDone(args, this.GridHandler);
            #endregion

            //Column initialization before gridHandler.Init()
            #region COLUMNS
            var addressColumn = this.GridHandler.AddTextBoxColumn(IOData.ADDRESS, 65);
            addressColumn.MaxInputLength = 10;

            this.GridHandler.AddTextBoxColumn(IOData.IO_NAME, 110);
            this.GridHandler.AddCustomColumn(this.variableAddressColumn, IOData.VARIABLE, 200);
            this.variableAddressColumn.SetGetItemsFunc(() =>
            {
                var ioVariableEnumerable = this.GridHandler.DataSource.GetNotEmptyDataDict().Keys.Select(i => i.Variable);
                var suggestionEnumerable = this.ioGenProject.Suggestions;

                var notAddedSuggestionEnumerable = suggestionEnumerable.Select(k => k.Value).Except(ioVariableEnumerable).ToArray();
                return notAddedSuggestionEnumerable ?? [];
            });

            this.GridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.GridHandler.AddTextBoxColumn(IOData.COMMENT, 0);

            this.mainConfig.Subscribe(() => this.mainConfig.MemoryType, UpdateMerkerColumn);
            UpdateMerkerColumn(this.mainConfig.MemoryType);
            #endregion

            this.TabControl.Init();

            this.GridHandler.Init();
            this.TabControl.BindConfig(this.TabConfig);

            #region SUGGESTION_GRIDS_EVENTS
            this.GridHandler.Events.CellChange += (sender, args) =>
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
                    this.UpdateDuplicatedIOValues();
                }
            };
            this.GridHandler.Events.PostSort += (sender, args) =>
            {
                ioGenProject.UpdateSuggestionColors();
                this.UpdateDuplicatedIOValues();
            };
            #endregion

            #region JS_SCRIPT_EVENTS
            this.GridHandler.Events.ScriptShowVariable += (sender, args) => args.VariableList.Add("suggestions [array]");
            this.GridHandler.Events.ScriptAddVariables += (sender, args) => args.VariableDict.Add("suggestions", this.ioGenProject.Suggestions.Select(s => s.Value).ToArray());
            #endregion
        }

        public bool IsDirty() => this.TabConfig.IsDirty() || this.GridHandler.IsDirty();
        public void Wash()
        {
            this.TabConfig.Wash();
            this.GridHandler.Wash();
        }

        private void UpdateMerkerColumn(IOMemoryTypeEnum memoryType)
        {
            this.GridHandler.ChangeColumnVisibility(IOData.MERKER_ADDRESS, visible: memoryType == IOMemoryTypeEnum.MERKER, init: true);
        }

        public IOGenTabSave CreateSave()
        {
            var save = new IOGenTabSave()
            {
                Name = TabPage.Text,
                IOGrid = this.GridHandler.CreateSave(),
            };

            GenUtils.CopyJsonFieldsAndProperties(this.TabConfig, save.TabConfig);
            return save;
        }

        public void LoadSave(IOGenTabSave save)
        {
            this.TabPage.Text = save.Name;

            this.GridHandler.LoadSave(save.IOGrid);
            GenUtils.CopyJsonFieldsAndProperties(save.TabConfig, this.TabConfig);

            this.UpdateDuplicatedIOValues();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return this.GridHandler.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateDuplicatedIOValues()
        {
            this.GridHandler.DataGridView.SuspendLayout();

            foreach (DataGridViewRow row in this.GridHandler.DataGridView.Rows)
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

            var dataDict = this.GridHandler.DataSource.GetNotEmptyDataDict();

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

                    var ioNameCell = this.GridHandler.DataGridView.Rows[rowIndex].Cells[IOData.IO_NAME.ColumnIndex];
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

                    var addressCell = this.GridHandler.DataGridView.Rows[rowIndex].Cells[IOData.ADDRESS.ColumnIndex];
                    addressCell.ToolTipText = tooltipText;
                    addressCell.Style.BackColor = ControlPaint.LightLight(Color.Orange);
                }
            }

            this.GridHandler.DataGridView.ResumeLayout();
        }
    }
}
