using TiaUtilities.Generation.GridHandler.CustomColumns;
using TiaUtilities.Generation.GridHandler.CustomColumns.ButtonColumn;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler
{
    internal class ColumnInfo(DataGridViewColumn column, GridDataColumn dataColumn, int width)
    {
        public DataGridViewColumn Column { get; init; } = column;
        public GridDataColumn DataColumn { get; init; } = dataColumn;
        public int Width { get; init; } = width;
        public bool Visible { get; set; } = true;
    }

    public class GridColumnHandler(ExcelLikeDataGridView dataGridView)
    {
        private readonly List<ColumnInfo> columnInfoList = [];

        public DataGridViewTextBoxColumn AddTextBox(GridDataColumn dataColumn, int width)
        {
            return Add(new DataGridViewTextBoxColumn(), dataColumn, width);
        }

        public DataGridViewCheckBoxColumn AddCheckBox(GridDataColumn dataColumn, int width)
        {
            DataGridViewCheckBoxColumn checkBoxColumn = new()
            {
                FlatStyle = FlatStyle.Popup,
                CellTemplate = new DataGridViewCustomCheckBoxCell()
            };

            return Add(checkBoxColumn, dataColumn, width);
        }

        public DataGridViewEventableButtonColumn AddButton(GridDataColumn dataColumn, int width)
        {
            DataGridViewEventableButtonColumn column = new()
            {
                UseColumnTextForButtonValue = false,
            };
            return Add(column, dataColumn, width);
        }

        public DataGridViewComboBoxColumn AddComboBox(GridDataColumn dataColumn, int width, string[] items)
        {
            DataGridViewComboBoxColumn column = new()
            {
                FlatStyle = FlatStyle.Flat
            };
            column.Items.AddRange(items);

            return Add(column, dataColumn, width); ;
        }

        public CL Add<CL>(CL column, GridDataColumn dataColumn, int width) where CL : DataGridViewColumn
        {
            this.columnInfoList.Add(new ColumnInfo(column, dataColumn, width));
            return column;
        }

        public void Show(GridDataColumn dataColumn)
        {
            this.ChangeVisibility(dataColumn, visible: true);
        }

        public void Hide(GridDataColumn dataColumn)
        {
            this.ChangeVisibility(dataColumn, visible: false);
        }

        public void ChangeVisibility(GridDataColumn dataColumn, bool visible, bool init = false)
        {
            var columnInfo = columnInfoList.FirstOrDefault(i => i.DataColumn == dataColumn);
            if (columnInfo == null)
            {
                return;
            }

            columnInfo.Visible = visible;
            if (init)
            {
                this.InitializeColumns();
            }
        }

        public void InitializeColumns()
        {
            dataGridView.Columns.Clear();

            this.columnInfoList.Sort((one, two) => one.DataColumn.ColumnIndex.CompareTo(two.DataColumn.ColumnIndex));
            foreach (var columnInfo in this.columnInfoList)
            {
                var column = columnInfo.Column;

                column.Name = columnInfo.DataColumn.Name;
                column.DisplayIndex = columnInfo.DataColumn.ColumnIndex;
                column.DataPropertyName = columnInfo.DataColumn.PropertyInfoName;
                column.AutoSizeMode = columnInfo.Width <= 0 ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None;
                column.Width = columnInfo.Width;
                column.MinimumWidth = 15;
                column.SortMode = DataGridViewColumnSortMode.Programmatic;

                column.Visible = columnInfo.Visible;

                dataGridView.Columns.Add(column);
            }
        }
    }
}
