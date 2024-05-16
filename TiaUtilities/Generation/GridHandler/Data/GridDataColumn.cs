using System.Reflection;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Generation.GridHandler.Data
{
    public class GridDataColumn
    {
        public static GridDataColumn GetFromReflection(Type type, int columnIndex, string propertyName, string programmingFriendlyName = null)
        {
            var propertyInfo = type.GetProperty(propertyName) ?? throw new Exception("Invalid property name while creating GridDataColumn from reflection from type " + type.FullName);
            var dataColumn = new GridDataColumn()
            {
                Name = propertyInfo.GetTranslation(),
                DataPropertyName = propertyInfo.Name,
                ColumnIndex = columnIndex,
                PropertyInfo = propertyInfo,
                ProgrammingFriendlyName = programmingFriendlyName ?? propertyInfo.Name.ToLower(),
            };
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
                    if(fieldValue == null)
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

        public string Name { get; set; }
        public string DataPropertyName { get; set; }
        public int ColumnIndex { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string ProgrammingFriendlyName { get; set; }

        public V? GetValueFrom<V>(object obj)
        {
            return obj != null && PropertyInfo.DeclaringType == obj.GetType() && PropertyInfo.GetValue(obj) is V value ? value : default;
        }

        public void SetValueTo(object obj, object? value)
        {
            if (obj == null || PropertyInfo.DeclaringType != obj.GetType())
            {
                return;
            }

            if (value == null)
            {
                if (!PropertyInfo.PropertyType.IsPrimitive)
                {
                    PropertyInfo.SetValue(obj, value);
                }
            }
            else if (value.GetType() == PropertyInfo.PropertyType)
            {
                PropertyInfo.SetValue(obj, value);
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
            int hashCode = 921909018;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DataPropertyName);
            hashCode = hashCode * -1521134295 + ColumnIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyInfo>.Default.GetHashCode(PropertyInfo);
            return hashCode;
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
