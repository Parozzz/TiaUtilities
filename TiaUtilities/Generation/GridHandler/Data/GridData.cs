using System.Runtime.CompilerServices;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public abstract class GridData
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!

        public event GridDataChangedEvent DataChanged = delegate { };
        private readonly Dictionary<string, object?> objectDict = [];

        public void ClearDataChangedDelegate()
        {
            this.DataChanged = delegate { };
        }

        public void Set(object? value, [CallerMemberName] string propertyName = "")
        {
            var added = objectDict.TryAdd(propertyName, value);
            if(added)
            {
                DataChanged.Invoke(this, new(this, propertyName, oldValue: null, value));
            }
            else
            {
                var oldValue = objectDict[propertyName];
                if (Utils.AreDifferentObject(oldValue, value))
                {
                    DataChanged.Invoke(this, new(this, propertyName, oldValue, value));
                    objectDict[propertyName] = value;
                }
            }
        }

        public object? Get([CallerMemberName] string key = "")
        {
            return objectDict.TryGetValue(key, out var value) ? value : null;
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
    public class GridDataChangedEventArgs(GridData data, string propertyName, object? oldValue, object? newValue)
    {
        public GridData Data { get; init; } = data;
        public string PropertyName { get; init; } = propertyName;
        public object? OldValue { get; init; } = oldValue;
        public object? NewValue { get; init; } = newValue;

        public void RestoreOldValue()
        {
            var property = this.Data.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if(property != null && property.CanWrite)
            {
                property.SetValue(this.Data, this.OldValue);
            }
        }

        public void RestoreNewValue()
        {
            var property = this.Data.GetType().GetProperty(this.PropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property != null && property.CanWrite)
            {
                property.SetValue(this.Data, this.NewValue);
            }
        }
    }
    
}
