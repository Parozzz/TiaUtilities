using Newtonsoft.Json;
using System.Diagnostics;
using System.Dynamic;
using TiaUtilities.JSScript;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridScriptExecutionData(IGridHandler gridHandler) : IJsScriptExecutionData
    {
        private static IJsScriptExecutionData.DataDescriptor CreateDescriptor(IGridHandler gridHandler)
        {
            IJsScriptExecutionData.DataDescriptor descriptor = new() { Name = "table" };

            var rows = descriptor.AddComplex("rows[x]");

            rows.SimpleProperties.Add("index", typeof(int).Name);

            var columns = gridHandler.DataSource.DataColumns;
            foreach (var column in columns)
            {
                rows.SimpleProperties.Add(column.ProgrammingFriendlyName, column.PropertyInfo.PropertyType.Name);
            }

            descriptor.ComplexProperties.Add(rows);

            return descriptor;
        }

        private class JsGridData
        {
            public List<dynamic> rows = [];
        }

        public Func<object> RequestData { get => RequestDataCallback; }
        public IJsScriptExecutionData.DataDescriptor Descriptor { get; init; } = CreateDescriptor(gridHandler);

        public Object RequestDataCallback()
        {
            var dict = gridHandler.DataSource.GetGenericNotEmptyDataDict();

            JsGridData jsGridData = new();
            foreach (var (data, row) in dict)
            {
                dynamic gridDataRow = new ExpandoObject();
                gridDataRow.index = row;

                foreach (var column in data.GetColumns())
                {
                    var dataValue = column.GetValueFrom(data);
                    ((IDictionary<string, object?>)gridDataRow).Add(column.ProgrammingFriendlyName, dataValue);
                }

                jsGridData.rows.Add(gridDataRow);
            }

            return jsGridData;
        }

        public void Done(Object? result)
        {
            if (result is not JsGridData jsGridData)
            {
                return;
            }

            var req = gridHandler.DataChangedHandler.Join();

            try
            {
                var dataSource = gridHandler.DataSource;
                var columns = gridHandler.DataSource.DataColumns;

                foreach (var row in jsGridData.rows)
                {
                    var index = row.index;
                    if (index is not int && index < 0 && index >= dataSource.Count)
                    {
                        continue;
                    }

                    var genericGridData = dataSource.GetGeneric((int)index);

                    foreach (var column in columns)
                    {
                        var tryGetOK = ((IDictionary<string, object?>)row).TryGetValue(column.ProgrammingFriendlyName, out var value);
                        if (tryGetOK)
                        {
                            genericGridData.Set(value, column.PropertyInfoName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            gridHandler.DataChangedHandler.End(req);

        }
    }
}
