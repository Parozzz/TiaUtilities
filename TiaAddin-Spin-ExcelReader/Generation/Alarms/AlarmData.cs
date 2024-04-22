using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.Alarms
{
    public class AlarmData : IGridData<AlarmConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ENABLE;
        public static readonly GridDataColumn ALARM_VARIABLE;
        public static readonly GridDataColumn COIL_ADDRESS;
        public static readonly GridDataColumn SET_COIL_ADDRESS;
        public static readonly GridDataColumn TIMER_ADDRESS;
        public static readonly GridDataColumn TIMER_TYPE;
        public static readonly GridDataColumn TIMER_VALUE;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static AlarmData()
        {
            var type = typeof(AlarmData);
            ENABLE = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Enable"));
            ALARM_VARIABLE = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("AlarmVariable"));
            COIL_ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("CoilAddress"));
            SET_COIL_ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("SetCoilAddress"));
            TIMER_ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("TimerAddress"));
            TIMER_TYPE = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("TimerType"));
            TIMER_VALUE = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("TimerValue"));
            DESCRIPTION = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Description"));

            COLUMN_LIST = new List<GridDataColumn>();
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && field.FieldType == typeof(GridDataColumn))
                {
                    COLUMN_LIST.Add((GridDataColumn) field.GetValue(null));
                }
            }
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_ENABLE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public bool Enable { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_ALARM_VARIABLE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string AlarmVariable { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_COIL_ADDRESS", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string CoilAddress { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_SET_COIL_ADDRESS", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string SetCoilAddress { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_TIMER_ADDRESS", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string TimerAddress { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_TIMER_TYPE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string TimerType { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_TIMER_VALUE", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string TimerValue { get; set; }

        [JsonProperty]
        [Display(Description = "ALARM_DATA_DESCRIPTION", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string Description { get; set; }

        public object this[int i]
        {
            get
            {
                if (i < 0 || i >= COLUMN_LIST.Count)
                {
                    throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }

                return COLUMN_LIST[i].PropertyInfo.GetValue(this);
            }
        }

        public GridDataPreview GetPreview(int column, AlarmConfiguration config)
        {
            if(string.IsNullOrEmpty(AlarmVariable) || this.IsEmpty())
            {
                return null;
            }

            if(column == COIL_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.CoilAddressPrefix,
                    DefaultValue = config.DefaultCoilAddress,
                    Value = this.CoilAddress
                };
            }
            else if(column == SET_COIL_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.SetCoilAddressPrefix,
                    DefaultValue = config.DefaultSetCoilAddress,
                    Value = this.SetCoilAddress
                };
            }
            else if(column == TIMER_ADDRESS)
            {
                return new GridDataPreview()
                {
                    Prefix = config.TimerAddressPrefix,
                    DefaultValue = config.DefaultTimerAddress,
                    Value = this.TimerAddress
                };
            }
            else if (column == TIMER_TYPE)
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

            return null;
        }

        public void Clear()
        {
            this.AlarmVariable = this.CoilAddress = this.SetCoilAddress = this.TimerAddress = this.TimerType = this.TimerValue = this.Description = "";
            this.Enable = true;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.AlarmVariable) && string.IsNullOrEmpty(this.CoilAddress) && string.IsNullOrEmpty(this.SetCoilAddress) &&
                string.IsNullOrEmpty(this.TimerAddress) && string.IsNullOrEmpty(this.TimerType) && string.IsNullOrEmpty(this.TimerValue) &&
                string.IsNullOrEmpty(this.Description);
        }
    }
}
