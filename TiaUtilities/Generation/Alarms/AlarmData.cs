﻿using Newtonsoft.Json;
using TiaUtilities.Languages;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;

namespace TiaUtilities.Generation.Alarms
{
    public class AlarmData : IGridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ENABLE;
        public static readonly GridDataColumn ALARM_VARIABLE;
        public static readonly GridDataColumn ALARM_NEGATED;
        public static readonly GridDataColumn CUSTOM_VARIABLE_ADDRESS;
        public static readonly GridDataColumn CUSTOM_VARIABLE_VALUE;
        public static readonly GridDataColumn COIL1_ADDRESS;
        public static readonly GridDataColumn COIL1_TYPE;
        public static readonly GridDataColumn COIL2_ADDRESS;
        public static readonly GridDataColumn COIL2_TYPE;
        public static readonly GridDataColumn TIMER_ADDRESS;
        public static readonly GridDataColumn TIMER_TYPE;
        public static readonly GridDataColumn TIMER_VALUE;
        public static readonly GridDataColumn HMI_ALARM_CLASS;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static AlarmData()
        {
            var type = typeof(AlarmData);
            ENABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Enable));
            ALARM_VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.AlarmVariable));
            ALARM_NEGATED = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.AlarmNegated), "negated");
            CUSTOM_VARIABLE_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.CustomVariableAddress), "customVarAddress");
            CUSTOM_VARIABLE_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.CustomVariableValue), "customVarValue");
            COIL1_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil1Address), "coil1Address");
            COIL1_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil1Type), "coil1Type");
            COIL2_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil2Address), "coil2Address");
            COIL2_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Coil2Type), "coil2Type");
            TIMER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerAddress), "timerAddress");
            TIMER_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerType), "timerType");
            TIMER_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerValue), "timerValue");
            HMI_ALARM_CLASS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.HmiAlarmClass), "hmiClass");
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Description));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_ENABLE))] public bool Enable { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_ALM_VARIABLE))] public string? AlarmVariable { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_ALM_NEGATED))] public bool AlarmNegated { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_CUSTOM_VAR_ADDR))] public string? CustomVariableAddress { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_CUSTOM_VAR_VALUE))] public string? CustomVariableValue { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_COIL1_ADDRESS))] public string? Coil1Address { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_COIL1_TYPE))] public string? Coil1Type { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_COIL2_ADDRESS))] public string? Coil2Address { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_COIL2_TYPE))] public string? Coil2Type { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_TIMER_ADDRESS))] public string? TimerAddress { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_TIMER_TYPE))] public string? TimerType { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_TIMER_VALUE))] public string? TimerValue { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_HMI_CLASS))] public string? HmiAlarmClass { get; set; }
        [JsonProperty][Locale(nameof(Locale.ALARM_DATA_DESCRIPTION), append: " > " + GenPlaceholders.Alarms.ALARM_DESCRIPTION)] public string? Description { get; set; }

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

        public static bool IsAddressValid(string? str)
        {
            return !GenUtils.DATA_INVALID_CHARS.Contains(str);
        }

        public void Clear()
        {
            this.Enable = this.AlarmNegated = false;
            this.AlarmVariable = this.CustomVariableAddress = this.CustomVariableValue = this.Coil1Address = this.Coil1Type
               = this.Coil2Address = this.Coil2Type = this.TimerAddress = this.TimerType = this.TimerValue = this.Description = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmVariable) &&
                string.IsNullOrEmpty(this.CustomVariableAddress) && string.IsNullOrEmpty(this.CustomVariableValue) &&
                string.IsNullOrEmpty(this.Coil1Address) && string.IsNullOrEmpty(this.Coil1Type) &&
                string.IsNullOrEmpty(this.Coil2Address) && string.IsNullOrEmpty(this.Coil2Type) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.Description);
        }

        public AlarmData Clone()
        {
            return new()
            {
                Enable = this.Enable,
                AlarmVariable = this.AlarmVariable,
                AlarmNegated = this.AlarmNegated,
                CustomVariableAddress = this.CustomVariableAddress,
                CustomVariableValue = this.CustomVariableValue,
                Coil1Address = this.Coil1Address,
                Coil1Type = this.Coil1Type,
                Coil2Address = this.Coil2Address,
                Coil2Type = this.Coil2Type,
                TimerAddress = this.TimerAddress,
                TimerType = this.TimerType,
                TimerValue = this.TimerValue,
                Description = this.Description
            };
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
