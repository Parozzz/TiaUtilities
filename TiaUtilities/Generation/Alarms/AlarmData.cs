using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.Alarms
{
    public class AlarmData : IGridData<AlarmTabConfiguration>
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ENABLE;
        public static readonly GridDataColumn ALARM_VARIABLE;
        public static readonly GridDataColumn COIL1_ADDRESS;
        public static readonly GridDataColumn COIL2_ADDRESS;
        public static readonly GridDataColumn TIMER_ADDRESS;
        public static readonly GridDataColumn TIMER_TYPE;
        public static readonly GridDataColumn TIMER_VALUE;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static AlarmData()
        {
            var type = typeof(AlarmData);
            ENABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Enable));
            ALARM_VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.AlarmVariable), "alarmVariable");
            COIL1_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil1Address), "coil1Address");
            COIL2_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil2Address), "coil2Address");
            TIMER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerAddress), "timerAddress");
            TIMER_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerType), "timerType");
            TIMER_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerValue), "timerValue");
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Description));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Localization("ALARM_DATA_ENABLE")] public bool Enable { get; set; }
        [JsonProperty][Localization("ALARM_DATA_ALM_VARIABLE")] public string? AlarmVariable { get; set; }
        [JsonProperty][Localization("ALARM_DATA_COIL1_ADDRESS")] public string? Coil1Address { get; set; }
        [JsonProperty][Localization("ALARM_DATA_COIL2_ADDRESS")] public string? Coil2Address { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_ADDRESS")] public string? TimerAddress { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_TYPE")] public string? TimerType { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_VALUE")] public string? TimerValue { get; set; }
        [JsonProperty][Localization("ALARM_DATA_DESCRIPTION", append: " > " + GenerationPlaceholders.Alarms.ALARM_DESCRIPTION)] public string? Description { get; set; }

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

        public GridDataPreview? GetPreview(GridDataColumn column, AlarmTabConfiguration config)
        {
            return this.GetPreview(column.ColumnIndex, config);
        }

        private static readonly List<string?> INVALID_CHARS = ["\\", "/", "-", "_", ".", ","];
        public bool IsAddressValid(string? str)
        {
            return !INVALID_CHARS.Contains(str);
        }

        public GridDataPreview? GetPreview(int column, AlarmTabConfiguration config)
        {
            if (string.IsNullOrEmpty(AlarmVariable) || this.IsEmpty())
            {
                return null;
            }

            if(column == ALARM_VARIABLE)
            {
                return new GridDataPreview()
                {
                    Prefix = config.AlarmAddressPrefix,
                    Value = this.AlarmVariable
                };
            }
            else if (column == COIL1_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.CoilAddressPrefix,
                    DefaultValue = config.DefaultCoil1Address,
                    Value = this.Coil1Address
                };
            }
            else if (column == COIL2_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.SetCoilAddressPrefix,
                    DefaultValue = config.DefaultCoil2Address,
                    Value = this.Coil2Address
                };
            }
            else if (column == TIMER_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.TimerAddressPrefix,
                    DefaultValue = config.DefaultTimerAddress,
                    Value = this.TimerAddress
                };
            }
            else if (string.IsNullOrEmpty(this.TimerAddress) ? IsAddressValid(config.DefaultTimerAddress) : IsAddressValid(this.TimerAddress))
            {
                if (column == TIMER_TYPE)
                {
                    return new GridDataPreview()
                    {
                        DefaultValue = config.DefaultTimerType,
                        Value = this.TimerType
                    };
                }
                else if (column == TIMER_VALUE)
                {
                    return new GridDataPreview()
                    {
                        DefaultValue = config.DefaultTimerValue,
                        Value = this.TimerValue
                    };
                }
            }


            return null;
        }

        public void Clear()
        {
            this.AlarmVariable = this.Coil1Address = this.Coil2Address = this.TimerAddress = this.TimerType = this.TimerValue = this.Description = "";
            this.Enable = true;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmVariable) && string.IsNullOrEmpty(this.Coil1Address) && string.IsNullOrEmpty(this.Coil2Address) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.Description);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenerationUtils.CompareJsonFieldsAndProperties(this, obj, out object invalid);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
