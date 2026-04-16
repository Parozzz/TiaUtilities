using Newtonsoft.Json;
using TiaUtilities.Languages;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.Placeholders;
using System.Text.Json.Nodes;

namespace TiaUtilities.Generation.Alarms.Data
{
    public class TemplateData : GridData
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
        public static readonly GridDataColumn HMI_PARAMETERS;
        public static readonly GridDataColumn HMI_ALARM_TEXT;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static TemplateData()
        {
            var type = typeof(TemplateData);
            ENABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Enable));
            ALARM_VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.AlarmVariable));
            ALARM_NEGATED = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.AlarmNegated), "negated");
            CUSTOM_VARIABLE_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.CustomVariableAddress), "customVarAddress");
            CUSTOM_VARIABLE_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.CustomVariableValue), "customVarValue");
            COIL1_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Coil1Address), "coil1Address");
            COIL1_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Coil1Type), "coil1Type");
            COIL2_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Coil2Address), "coil2Address");
            COIL2_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Coil2Type), "coil2Type");
            TIMER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.TimerAddress), "timerAddress");
            TIMER_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.TimerType), "timerType");
            TIMER_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.TimerValue), "timerValue");
            HMI_ALARM_CLASS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.HmiAlarmClass), "hmiClass");
            HMI_PARAMETERS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.HmiParametersJsonString));
            HMI_ALARM_TEXT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.HmiAlarmText), "hmiAlarmText");
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(TemplateData.Description));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_ENABLE))] public bool Enable { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_ALM_VARIABLE))] public string? AlarmVariable { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_ALM_NEGATED))] public bool AlarmNegated { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_CUSTOM_VAR_ADDR))] public string? CustomVariableAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_CUSTOM_VAR_VALUE))] public string? CustomVariableValue { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_COIL1_ADDRESS))] public string? Coil1Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_COIL1_TYPE))] public string? Coil1Type { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_COIL2_ADDRESS))] public string? Coil2Address { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_COIL2_TYPE))] public string? Coil2Type { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_TIMER_ADDRESS))] public string? TimerAddress { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_TIMER_TYPE))] public string? TimerType { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_TIMER_VALUE))] public string? TimerValue { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_HMI_CLASS))] public string? HmiAlarmClass { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_HMI_PARAMETERS), append: $" > {GenPlaceholders.Alarms.HMI_PARAMETER}")] public string? HmiParametersJsonString { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_HMI_TEXT), append: $" > {GenPlaceholders.Alarms.ALARM_HMI_TEXT}")] public string ? HmiAlarmText { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty][Locale(nameof(Locale.ALARM_TEMPLATE_DATA_DESCRIPTION), append: $" > {GenPlaceholders.Alarms.ALARM_DESCRIPTION}")] public string? Description { get => this.GetAs<string>(); set => this.Set(value); }

        public override IReadOnlyList<GridDataColumn> GetColumns()
        {
            return COLUMN_LIST;
        }

        public override GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }

        public static bool IsAddressValid(string? str)
        {
            return !GenUtils.DATA_INVALID_CHARS.Contains(str);
        }

        public override void Clear()
        {
            this.Enable = this.AlarmNegated = false;
            this.AlarmVariable = this.CustomVariableAddress = this.CustomVariableValue = this.Coil1Address 
                = this.Coil1Type = this.Coil2Address = this.Coil2Type = this.TimerAddress = this.TimerType = this.TimerValue 
                = this.HmiParametersJsonString = this.HmiAlarmText = this.Description = null;
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmVariable) &&
                string.IsNullOrEmpty(this.CustomVariableAddress) && string.IsNullOrEmpty(this.CustomVariableValue) &&
                string.IsNullOrEmpty(this.Coil1Address) && string.IsNullOrEmpty(this.Coil1Type) &&
                string.IsNullOrEmpty(this.Coil2Address) && string.IsNullOrEmpty(this.Coil2Type) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.HmiParametersJsonString) && string.IsNullOrEmpty(this.HmiAlarmText) && string.IsNullOrEmpty(this.Description);
        }

        public TemplateData Clone()
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
                HmiAlarmText = this.HmiAlarmText,
                HmiParametersJsonString = this.HmiParametersJsonString,
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
