using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.Alarms
{
    public class DeviceData : IGridData<AlarmConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static DeviceData()
        {
            var type = typeof(DeviceData);
            ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Address"));
            DESCRIPTION = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Description"));

            COLUMN_LIST = new List<GridDataColumn>();
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && field.FieldType == typeof(GridDataColumn))
                {
                    COLUMN_LIST.Add((GridDataColumn)field.GetValue(null));
                }
            }
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty]
        [Display(Description = "DEVICE_DATA_ADDRESS", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string Address { get; set; }

        [JsonProperty]
        [Display(Description = "DEVICE_DATA_DESCRIPTION", ResourceType = typeof(Localization.Alarm.AlarmGenerationLocalization))]
        public string Description { get; set; }

        public GridDataPreview GetPreview(int column, AlarmConfiguration config)
        {
            return null;
        }

        public void Clear()
        {
            this.Address = this.Description = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Address) && string.IsNullOrEmpty(this.Description);
        }
    }
}
