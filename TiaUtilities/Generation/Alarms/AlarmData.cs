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
using TiaXmlReader.Localization;
using TiaXmlReader.Generation.Placeholders;

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
            ENABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Enable));
            ALARM_VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.AlarmVariable), "alarmVariable");
            COIL_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.CoilAddress), "coilAddress");
            SET_COIL_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.SetCoilAddress), "setCoilAddress");
            TIMER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerAddress), "timerAddress");
            TIMER_TYPE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerType), "timerType");
            TIMER_VALUE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.TimerValue), "timerValue");
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(AlarmData.Description));

            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty][Localization("ALARM_DATA_ENABLE")] public bool Enable { get; set; }
        [JsonProperty][Localization("ALARM_DATA_ALM_VARIABLE")] public string AlarmVariable { get; set; }
        [JsonProperty][Localization("ALARM_DATA_COIL_ADDRESS")] public string CoilAddress { get; set; }
        [JsonProperty][Localization("ALARM_DATA_SET_COIL_ADDRESS")] public string SetCoilAddress { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_ADDRESS")] public string TimerAddress { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_TYPE")] public string TimerType { get; set; }
        [JsonProperty][Localization("ALARM_DATA_TIMER_VALUE")] public string TimerValue { get; set; }
        [JsonProperty][Localization("ALARM_DATA_DESCRIPTION", append: " > " + GenerationPlaceholders.Alarms.ALARM_DESCRIPTION)] public string Description { get; set; }

        public object this[int column]
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

        public GridDataColumn GetColumn(int column)
        {
            return COLUMN_LIST[column];
        }
        public GridDataPreview GetPreview(GridDataColumn column, AlarmConfiguration config)
        {
            return this.GetPreview(column.ColumnIndex, config);
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
