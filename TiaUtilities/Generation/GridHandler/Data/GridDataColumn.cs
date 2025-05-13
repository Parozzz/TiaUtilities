using System.Reflection;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.Data
{
    public class GridDataColumn(string name, string dataPropertyName, int columnIndex, PropertyInfo propertyInfo, string programmingFriendlyName)
    {
        public static GridDataColumn GetFromReflection(Type type, int columnIndex, string propertyName, string? programmingFriendlyName = null)
        {
            var propertyInfo = type.GetProperty(propertyName) ?? throw new Exception("Invalid property name while creating GridDataColumn from reflection from type " + type.FullName);
            var dataColumn = new GridDataColumn(
                name: propertyInfo.GetTranslation(),
                dataPropertyName: propertyInfo.Name,
                columnIndex: columnIndex,
                propertyInfo: propertyInfo,
                programmingFriendlyName: programmingFriendlyName ?? propertyInfo.Name.ToLower()
            );
            return dataColumn;
        }

        public static List<GridDataColumn> GetStaticColumnList(Type type)
        {
            var columnList = new List<GridDataColumn>();
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && field.FieldType == typeof(GridDataColumn))
                {
                    var fieldValue = field.GetValue(null);
                    if (fieldValue == null)
                    {
                        Utils.ShowExceptionMessage(new Exception("GridDataColumn is null inside " + type.Name));
                        continue;
                    }

                    var column = (GridDataColumn)fieldValue;
                    columnList.Add(column);
                }
            }
            return columnList;
        }

        public string Name { get; init; } = name;
        public string DataPropertyName { get; init; } = dataPropertyName;
        public int ColumnIndex { get; init; } = columnIndex;
        public PropertyInfo PropertyInfo { get; init; } = propertyInfo;
        public string ProgrammingFriendlyName { get; init; } = programmingFriendlyName;

        public V? GetValueFrom<V>(IGridData? gridData)
        {
            if (gridData == null || PropertyInfo.DeclaringType != gridData.GetType())
            {
                return default;
            }

            var propertyValue = PropertyInfo.GetValue(gridData);
            if (propertyValue is not V value)
            {
                return default;
            }

            return value;
        }

        public object? GetValueFrom(IGridData gridData)
        {
            if (gridData == null || PropertyInfo.DeclaringType != gridData.GetType())
            {
                return default;
            }

            return PropertyInfo.GetValue(gridData);
        }

        public void SetValueTo(IGridData gridData, object? value)
        {
            if (gridData == null || PropertyInfo.DeclaringType != gridData.GetType())
            {
                return;
            }

            if (value == null)
            {
                if (!PropertyInfo.PropertyType.IsPrimitive)
                {
                    PropertyInfo.SetValue(gridData, value);
                }
            }
            else if (value.GetType() == PropertyInfo.PropertyType)
            {
                PropertyInfo.SetValue(gridData, value);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is GridDataColumn column &&
                   Name == column.Name &&
                   DataPropertyName == column.DataPropertyName &&
                   ColumnIndex == column.ColumnIndex &&
                   EqualityComparer<PropertyInfo>.Default.Equals(PropertyInfo, column.PropertyInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DataPropertyName, ColumnIndex, PropertyInfo);
        }

        #region OPERATORS GridDataColumn - int
        public static bool operator ==(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex == obj2;
        }

        public static bool operator !=(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex != obj2;
        }

        public static bool operator >(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex > obj2;
        }
        public static bool operator >=(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex >= obj2;
        }

        public static bool operator <(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex < obj2;
        }

        public static bool operator <=(GridDataColumn obj1, int obj2)
        {
            return obj1.ColumnIndex <= obj2;
        }
        #endregion

        #region OPERATORS int - GridDataColumn
        public static bool operator ==(int obj1, GridDataColumn obj2)
        {
            return obj1 == obj2.ColumnIndex;
        }

        public static bool operator !=(int obj1, GridDataColumn obj2)
        {
            return obj1 != obj2.ColumnIndex;
        }

        public static bool operator <(int obj1, GridDataColumn obj2)
        {
            return obj1 < obj2.ColumnIndex;
        }

        public static bool operator <=(int obj1, GridDataColumn obj2)
        {
            return obj1 <= obj2.ColumnIndex;
        }

        public static bool operator >(int obj1, GridDataColumn obj2)
        {
            return obj1 > obj2.ColumnIndex;
        }

        public static bool operator >=(int obj1, GridDataColumn obj2)
        {
            return obj1 >= obj2.ColumnIndex;
        }
        #endregion

    }
}
