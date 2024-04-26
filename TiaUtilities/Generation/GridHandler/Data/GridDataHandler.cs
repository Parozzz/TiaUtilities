using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Data
{
    public class GridDataHandler<C, T> where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly DataGridView dataGridView;
        public List<GridDataColumn> DataColumns { get; private set; }

        public GridDataHandler(DataGridView dataGridView, List<GridDataColumn> dataColumns)
        {
            this.dataGridView = dataGridView;
            this.DataColumns = dataColumns;
        }

        public T CreateInstance()
        {
            var type = typeof(T);
            return (T) type.Assembly.CreateInstance(type.FullName);
        }

        public void MoveValues(T copyFrom, T moveTo)
        {
            foreach (var dataColumn in DataColumns)
            {
                var copeFromValue = dataColumn.GetValueFrom<object>(copyFrom);
                dataColumn.SetValueTo(moveTo, copeFromValue);
            }
        }

        public List<GridCellChange> CreateCellChanges(int rowIndex, T data)
        {
            return CreateCellChanges(new Dictionary<int, T> { { rowIndex, data } });
        }

        public List<GridCellChange> CreateCellChanges(Dictionary<int, T> dataDict)
        {
            var cellChangeList = new List<GridCellChange>();

            foreach (var entry in dataDict)
            {
                var rowIndex = entry.Key;
                var data = entry.Value;

                foreach (var dataColumn in DataColumns)
                {
                    var value = dataColumn.GetValueFrom<object>(data);
                    var columnIndex = dataColumn.ColumnIndex;
                    cellChangeList.Add(new GridCellChange(dataGridView, columnIndex, rowIndex) { NewValue = value });
                }
            }

            return cellChangeList;
        }
    }
}
