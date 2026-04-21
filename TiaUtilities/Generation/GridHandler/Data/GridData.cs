using DocumentFormat.OpenXml.Vml.Office;
using System.Runtime.CompilerServices;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public abstract class GridData
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!

        private class StoredData
        {
            public required object? Value { get; set; }
            public required GridDataColumn Column { get; init; }
        }

        public event GridDataChangedEvent DataChanged = delegate { };
        private readonly Dictionary<string, StoredData> objectDict = [];

        public void ClearDataChangedDelegate()
        {
            this.DataChanged = delegate { };
        }

        public void Set(object? newValue, [CallerMemberName] string propertyName = "")
        {
            var getDone = objectDict.TryGetValue(propertyName, out var storedData);
            if (getDone)
            {
                if (storedData != null && Utils.AreDifferentObject(storedData.Value, newValue))
                {
                    var oldData = storedData.Value;
                    storedData.Value = newValue;
                    //Maybe the data changes is better to be called AFTER data is changed?
                    DataChanged.Invoke(this, new(this, propertyName, storedData.Column, oldData, newValue));
                }
            }
            else
            {
                try
                {
                    var column = this.GetColumnFromPropertyName(propertyName);
                    objectDict.Add(propertyName, new()
                    {
                        Column = column,
                        Value = newValue
                    });

                    //Maybe the data changes is better to be called AFTER data is changed?
                    DataChanged.Invoke(this, new(this, propertyName, column, OldValue: null, newValue));
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }


            }
        }

        private GridDataColumn GetColumnFromPropertyName(string propertyName)
        {
            var columns = this.GetColumns();
            foreach (var column in columns)
            {
                if (column.PropertyInfoName == propertyName)
                {
                    return column;
                }
            }
            //It must exists! 
            throw new ArgumentException($"{propertyName} DataGridColumn does not exists in {this.GetType().FullName}");
        }

        public object? Get([CallerMemberName] string key = "")
        {
            return objectDict.TryGetValue(key, out var storedData) ? storedData.Value : null;
        }

        public T? GetAs<T>([CallerMemberName] string key = "")
        {
            var obj = this.Get(key);
            return obj is T t ? t : default;
        }

        public object? this[int column]
        {
            get
            {
                var columns = this.GetColumns();
                if (column < 0 || column >= columns.Count)
                {
                    throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }

                return columns[column].PropertyInfo.GetValue(this);
            }
        }

        public abstract void Clear();

        public abstract bool IsEmpty();

        public abstract IReadOnlyList<GridDataColumn> GetColumns();

        public abstract GridDataColumn GetColumn(int column);

    }

    public delegate void GridDataChangedEvent(object? Sender, GridDataChangedEventArgs args);
    public record GridDataChangedEventArgs(GridData Data, string PropertyName, GridDataColumn Column, object? OldValue, object? NewValue);

}
