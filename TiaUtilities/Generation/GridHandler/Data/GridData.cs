using DocumentFormat.OpenXml.Vml.Office;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public abstract class GridData : IGridData
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!

        private class StoredData
        {
            public required GridDataColumn Column { get; init; }
            public required object? Value { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        public event GridDataPropertyChangedEvent DataPropertyChanged = delegate { };

        private readonly Dictionary<string, StoredData> objectDict = [];

        public void Set(object? newValue, [CallerMemberName] string propertyName = "")
        {
            var getDone = objectDict.TryGetValue(propertyName, out var storedData);
            if (getDone)
            {
                if (storedData != null && Utils.AreDifferentObject(storedData.Value, newValue))
                {
                    var oldValue = storedData.Value;
                    storedData.Value = newValue;

                    //Maybe the data changes is better to be called AFTER data is changed?
                    this.CallDataChangedEvent(propertyName, storedData.Column,  oldValue, newValue);
                }
            }
            else
            {
                try
                {
                    var column = this.GetColumnFromPropertyName(propertyName);
                    objectDict.Add(propertyName, new() { Column = column, Value = newValue });

                    //Maybe the data changes is better to be called AFTER data is changed?
                    this.CallDataChangedEvent(propertyName, column, oldValue: null, newValue);
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            }
        }

        private void CallDataChangedEvent(string propertyName, GridDataColumn column, object? oldValue, object? newValue)
        {
            //Maybe the data changes is better to be called AFTER data is changed?
            this.DataPropertyChanged.Invoke(this, new(this, propertyName, column, oldValue, newValue));
            this.PropertyChanged?.Invoke(this, new(propertyName));
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

        private IReadOnlyList<GridDataColumn> ValidateColumns(int column)
        {
            var columns = this.GetColumns();
            if (column < 0 || column >= columns.Count)
            {
                throw new InvalidOperationException($"Invalid index for GridData{column}");
            }
            return columns;
        }

        public object? this[GridDataColumn c]
        {
            get => this[c.ColumnIndex];
            set => this[c.ColumnIndex] = value;
        }

        public object? this[int c]
        {
            get
            {
                var columns = ValidateColumns(c);
                return columns[c].PropertyInfo.GetValue(this);
            }

            set
            {
                var columns = ValidateColumns(c);

                var propertyInfo = columns[c].PropertyInfo;
                if(value != null && !propertyInfo.PropertyType.IsAssignableFrom(value.GetType()))
                {
                    throw new InvalidOperationException($"Invalid value type for SET GridData[{c}]. Found: {value?.GetType().FullName}, Expected: {propertyInfo.PropertyType.FullName}");
                }

                propertyInfo.SetValue(this, value);
            }
        }

        public abstract void Clear();

        public abstract bool IsEmpty();

        public abstract IReadOnlyList<GridDataColumn> GetColumns();

        public abstract GridDataColumn GetColumn(int column);

        public void Dispose()
        {
            this.PropertyChanged = delegate { };
            this.DataPropertyChanged = delegate { };
        }

    }

    public delegate void GridDataPropertyChangedEvent(object? Sender, GridDataPropertyChangedEventArgs args);
    public record GridDataPropertyChangedEventArgs(GridData Data, string PropertyName, GridDataColumn Column, object? OldValue, object? NewValue);

}
