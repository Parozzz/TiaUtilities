using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.Generation.GridHandler.JSScript
{
    public class GridScriptVariable(string programmingName, string valueType)
    {
        public static GridScriptVariable CreateFromGrid<H>(GridDataColumn column, GridHandler<H> gridHandler) where H : IGridData
        {
            GridScriptVariable scriptVariable = new(column.ProgrammingFriendlyName, column.PropertyInfo.PropertyType.Name)
            {
                Get = row => column.GetValueFrom(gridHandler.DataSource[row]),
                CreateCachedCellChange = (row, v) =>
                {
                    var actValue = column.GetValueFrom(gridHandler.DataSource[row]);

                    GridCellChange cellChange = new(column, row)
                    {
                        OldValue = actValue,
                        NewValue = v
                    };
                    gridHandler.AddCachedCellChange(cellChange);
                }
            };
            return scriptVariable;
        }

        public static GridScriptVariable ReadOnlyValue(string programmingFriendlyName, Expression<Func<object>> getExpression)
        {
            var typeName = getExpression.Body.Type.Name;

            var func = getExpression.Compile();
            return new(programmingFriendlyName, typeName) { Get = row => func() };
        }

        public string Name { get; set; } = programmingName;
        public string ValueType { get; set; } = valueType;
        public Action<int, object?>? Set { get; set; }
        public Action<int, object?>? CreateCachedCellChange { get; set; }
        public Func<int, object?>? Get { get; set; }
    }
}
