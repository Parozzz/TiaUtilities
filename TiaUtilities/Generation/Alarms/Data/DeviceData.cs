using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.Data
{
    public class DeviceData : GridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn NAME;
        public static readonly GridDataColumn TEMPLATE;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly GridDataColumn PLACEHOLDERS;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static DeviceData()
        {
            var type = typeof(DeviceData);
            NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Name));
            TEMPLATE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Template));
            PLACEHOLDERS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Placeholders));
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Description));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Locale(nameof(Locale.DEVICE_DATA_NAME), append: $" > {GenPlaceholders.Alarms.DEVICE_NAME}")] public string? Name { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.DEVICE_DATA_DESCRIPTION), append: $" > {GenPlaceholders.Alarms.DEVICE_DESCRIPTION}")] public string? Description { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.DEVICE_DATA_TEMPLATE), append: $" > {GenPlaceholders.Alarms.DEVICE_TEMPLATE}")] public string? Template { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.DEVICE_DATA_PLACEHOLDERS), append: $" > {GenPlaceholders.Alarms.DEVICE_PLACEHOLDERS_GENERIC} ({GenPlaceholders.Alarms.DEVICE_PLACEHOLDERS_GENERIC_SPLITTER})")] public string? Placeholders { get => this.GetAs<string>(); set => this.Set(value); }

        public override IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public override GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public override void Clear()
        {
            this.Name = this.Description = this.Placeholders = null;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Name) && string.IsNullOrEmpty(this.Description) && string.IsNullOrEmpty(this.Placeholders);
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
