using System.Linq.Expressions;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaUtilities.JSScript
{
    public class JSScriptVariable(string programmingName, string valueType)
    {
        public static JSScriptVariable CreateFromGrid<H>(GridDataColumn column, GridHandler<H> gridHandler) where H : GridRowData
        {
            JSScriptVariable scriptVariable = new(column.ProgrammingFriendlyName, column.PropertyInfo.PropertyType.Name)
            {
                Get = row => column.GetValue(gridHandler.DataSource[row]),
                CreateCachedCellChange = (row, v) =>
                {
                    var data = gridHandler.DataSource[row];
                    column.SetValue(data, v);
                }
            };
            return scriptVariable;
        }

        public static JSScriptVariable ReadOnlyValue(string programmingFriendlyName, Expression<Func<object>> getExpression)
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
