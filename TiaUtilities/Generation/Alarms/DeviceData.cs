using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{
    public class DeviceData : IGridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn NAME;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static DeviceData()
        {
            var type = typeof(DeviceData);
            NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Name));
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Description));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Localization("DEVICE_DATA_NAME", append: " > " + GenPlaceholders.Alarms.DEVICE_NAME)] public string? Name { get; set; }
        [JsonProperty][Localization("DEVICE_DATA_DESCRIPTION", append: " > " + GenPlaceholders.Alarms.DEVICE_DESCRIPTION)] public string? Description { get; set; }
        public object? this[int column]
        {
            get
            {
                if (column < 0 || column >= COLUMN_LIST.Count)
                {
                    throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }

                return COLUMN_LIST[column].PropertyInfo.GetValue(this);
            }
        }

        public IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public void Clear()
        {
            this.Name = this.Description = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Description);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out object invalid);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
