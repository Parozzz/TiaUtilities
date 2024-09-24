using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FastColoredTextBoxNS;
using InfoBox;
using System.Data;
using TiaUtilities.Generation.GridHandler.Find_Replace;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Utility;
using static TiaUtilities.Generation.GridHandler.GridFindForm;

namespace TiaUtilities.Generation.GridHandler
{
    public partial class GridFindForm : Form
    {
        public class FindData(GridDataColumn column, int row, string text)
        {
            public GridDataColumn Column { get; init; } = column;
            public int Row { get; init; } = row;
            public string Text { get; init; } = text;
        }

        private GridFindHandlerBind? bind;

        public GridFindForm()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.FormClosed += (sender, args) => this.bind = null;
            this.FindButton.Click += (sender, args) => this.TryFindText(startFromNextCell: true);
            this.ReplaceButton.Click += (sender, args) =>
            {
                if (this.bind == null || this.TryFindText(startFromNextCell: false) is not FindData findData)
                {
                    return;
                }

                this.bind.ClearCachedCellChange();
                var addOK = this.AddSearchReplaceCellChange(findData);
                this.bind.ApplyCachedCellChange();

                if(addOK)
                {
                    this.TryFindText(startFromNextCell: true);
                }
            };
            this.ReplaceAllButton.Click += (sender, args) =>
            {
                if(this.bind == null)
                {
                    return;
                }

                this.bind.DataGridView.SuspendLayout();
                this.bind.ClearCachedCellChange();

                int count = 0;

                //The first time, i call it with "startFromNextCell" false so it will also check the current cell!
                if (TryFindText(startFromNextCell: false, showInfoOnFail: false) is FindData firstFindData)
                {
                    this.AddSearchReplaceCellChange(firstFindData);
                    count++;
                }

                while (TryFindText(startFromNextCell: true, showInfoOnFail: false) is FindData findData)
                {
                    this.AddSearchReplaceCellChange(findData);
                    count++;
                }

                this.bind.ApplyCachedCellChange();
                this.bind.DataGridView.ResumeLayout(performLayout: true);

                var title = Locale.GRID_FIND_FORM_NAME;
                var searchCompletedText = Locale.GRID_FIND_REPLACE_ALL_COMPLETED.Replace("{count}", count.ToString());
                InformationBox.Show(searchCompletedText, title,
                        buttons: InformationBoxButtons.OK,
                        icon: InformationBoxIcon.Information,
                        titleStyle: InformationBoxTitleIconStyle.SameAsBox);
            };

            Translate();
        }

        private void Translate()
        {
            this.Text = Locale.GRID_FIND_FORM_NAME;
            this.FindLabel.Text = this.FindButton.Text = Locale.GRID_FIND_FIND_BUTTON;
            this.ReplaceLabel.Text = this.ReplaceButton.Text = Locale.GRID_FIND_REPLACE_BUTTON;
            this.ReplaceAllButton.Text = Locale.GRID_FIND_REPLACE_ALL_BUTTON;
            this.MatchCaseCheckBox.Text = Locale.GRID_FIND_MATCH_CASE_CHECKBOX;
        }

        public void ShowBinded<T>(GridHandler<T> gridHandler) where T : IGridData
        {
            var form = gridHandler.DataGridView.FindForm();
            if(form == null)
            {
                return;
            }

            this.bind = GridFindHandlerBind.Bind(gridHandler);
            this.Show(form);
        }

        private bool AddSearchReplaceCellChange(FindData? findData)
        {
            if (findData == null || this.bind == null)
            {
                return false;
            }

            var searchText = this.FindTextBox.Text;
            if(string.IsNullOrEmpty(searchText))
            {
                return false;
            }

            var replaceText = this.ReplaceTextBox.Text ?? "";
            var newText = findData.Text.Replace(searchText, replaceText, StringComparison.OrdinalIgnoreCase);

            GridCellChange cellChange = new(findData.Column, findData.Row) { NewValue = newText };
            this.bind.AddCachedCellChange(cellChange);

            return true;
        }

        private FindData? TryFindText(bool showInfoOnFail = true, bool startFromNextCell = true)
        {
            if(this.bind == null)
            {
                return null;
            }

            var matchCase = this.MatchCaseCheckBox.Checked;
            var findText = this.FindTextBox.Text;
            if (string.IsNullOrEmpty(findText))
            {
                return null;
            }

            var currentCell = bind.DataGridView.CurrentCell;

            var startRow = currentCell?.RowIndex ?? 0;
            var startColumn = (currentCell?.ColumnIndex ?? 0); 

            if(startFromNextCell)
            {
                startColumn++;
                if (startColumn >= this.bind.DataGridView.ColumnCount)
                {
                    startRow++;
                    startColumn = 0;
                }
            }

            FindData? findData = null;

            var notEmptyRowIndexes = this.bind.GetNotEmptyRowIndexesStartingAt(startRow);
            for (int x = 0; x < notEmptyRowIndexes.Count; x++)
            {
                var rowIndex = notEmptyRowIndexes.ElementAt(x);

                var stringColumns = this.bind.DataColumns.Where(c => c.PropertyInfo.PropertyType == typeof(string));
                if(x == 0)
                { //Only for the starting row! From the second i want to check all columns.
                    stringColumns = stringColumns.Where(c => c.ColumnIndex >= startColumn);
                }

                foreach (var column in stringColumns)
                {
                    var strValue = this.bind.GetColumnStringData(column, rowIndex);
                    if (strValue == null)
                    {
                        continue;
                    }

                    if (strValue.Contains(findText, matchCase ? StringComparison.Ordinal : StringComparison.CurrentCultureIgnoreCase))
                    {
                        findData = new(column, rowIndex, strValue);
                        break;
                    }
                }

                if (findData != null)
                {
                    //I directly edit the CurrentCell to avoid calling unwanted refresh!
                    this.bind.DataGridView.CurrentCell = this.bind.DataGridView.Rows[findData.Row].Cells[findData.Column.ColumnIndex];
                    break;
                }
            }

            if (findData == null && showInfoOnFail)
            {
                var title = Locale.GRID_FIND_FORM_NAME;
                var searchCompletedText = Locale.GRID_FIND_SEARCH_COMPLETED;

                InformationBox.Show(searchCompletedText, title,
                        buttons: InformationBoxButtons.OK,
                        icon: InformationBoxIcon.Information,
                        titleStyle: InformationBoxTitleIconStyle.SameAsBox);
            }

            return findData;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    this.FindButton.PerformClick();
                    return true;
                case Keys.Enter | Keys.Control:
                    this.ReplaceButton.PerformClick();
                    return true;
                case Keys.Escape:
                    this.Close();
                    return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
