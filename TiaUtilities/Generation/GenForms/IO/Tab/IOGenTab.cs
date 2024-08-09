using TiaUtilities.Generation.IO;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.CustomColumns;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Javascript;

namespace TiaUtilities.Generation.GenForms.IO.Tab
{
    public class IOGenTab
    {
        private const int MERKER_ADDRESS_COLUMN_SIZE = 80;

        private readonly IOGenProject ioGenProject;
        public TabPage TabPage { get; init; }

        public GridHandler<IOMainConfiguration, IOData> GridHandler { get; init; }
        private readonly SuggestionTextBoxColumn variableAddressColumn;

        public IOGenTabControl GenTabControl { get; init; }
        public IOTabConfiguration TabConfig { get; init; }
        private readonly IOGenTabConfigHandler genTabConfigHandler;

        private string jsScript = "";

        public IOGenTab(IOGenProject ioGenProject, TabPage tabPage, IOMainConfiguration mainConfig, JavascriptErrorReportThread jsErrorHandlingThread, GridSettings gridSettings)
        {
            this.ioGenProject = ioGenProject;
            this.TabPage = tabPage;

            this.GridHandler = new GridHandler<IOMainConfiguration, IOData>(jsErrorHandlingThread, gridSettings, mainConfig, new IOGenComparer()) { RowCount = 2999 };
            this.variableAddressColumn = new();

            this.GenTabControl = new(GridHandler.DataGridView);
            this.TabConfig = new();
            this.genTabConfigHandler = new(GenTabControl, TabConfig);
        }

        public void Init()
        {
            #region DRAG
            this.GridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.GridHandler); });
            this.GridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.GridHandler); });
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
                return notAddedSuggestionEnumerable;
            });

            this.GridHandler.AddTextBoxColumn(IOData.MERKER_ADDRESS, MERKER_ADDRESS_COLUMN_SIZE);
            this.GridHandler.AddTextBoxColumn(IOData.COMMENT, 0);
            #endregion

            this.GenTabControl.Init();

            this.GridHandler.Init();
            this.genTabConfigHandler.Init();

            #region GRIDS_JSSCRIPT_EVENTS
            this.GridHandler.Events.ScriptLoad += args => args.Script = this.jsScript;
            this.GridHandler.Events.ScriptChanged += args => this.jsScript = args.Script;
            #endregion

            #region SUGGESTION_GRIDS_EVENTS
            this.GridHandler.Events.CellChange += args =>
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
            this.GridHandler.Events.PostSort += args =>
            {
                ioGenProject.UpdateSuggestionColors();
                this.UpdateDuplicatedIOValues();
            };
            #endregion

            #region JS_SCRIPT_EVENTS
            this.GridHandler.Events.ScriptShowVariable += args => args.VariableList.Add("suggestions [array]");
            this.GridHandler.Events.ScriptAddVariables += args => args.VariableDict.Add("suggestions", this.ioGenProject.Suggestions.Select(s => s.Value).ToArray());
            #endregion
        }

        public IOGenTabSave CreateSave()
        {
            var save = new IOGenTabSave()
            {
                Name = TabPage.Text,
                JSScript = this.jsScript,
            };

            GenerationUtils.CopyJsonFieldsAndProperties(this.TabConfig, save.TabConfig);

            foreach (var entry in this.GridHandler.DataSource.GetNotEmptyClonedDataDict())
            {
                save.AddIOData(entry.Key, entry.Value);
            }

            return save;
        }

        public void LoadSave(IOGenTabSave save)
        {
            this.GridHandler.DataGridView.SuspendLayout();

            this.TabPage.Text = save.Name;

            this.jsScript = save.JSScript;
            GenerationUtils.CopyJsonFieldsAndProperties(save.TabConfig, this.TabConfig);

            this.GridHandler.DataSource.Clear();

            foreach (var entry in save.IOData)
            {
                var rowIndex = entry.Key;
                var data = entry.Value;
                if (rowIndex >= 0 && rowIndex <= this.GridHandler.RowCount)
                {
                    this.GridHandler.DataHandler.CopyValues(data, this.GridHandler.DataSource[rowIndex]);
                }
            }

            this.GridHandler.DataGridView.Refresh();
            this.GridHandler.DataGridView.ResumeLayout();

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
