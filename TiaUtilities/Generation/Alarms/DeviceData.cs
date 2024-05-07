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
using TiaXmlReader.Localization;

namespace TiaXmlReader.Generation.Alarms
{
    public class DeviceData : IGridData<AlarmConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn NAME;
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn DESCRIPTION;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static DeviceData()
        {
            var type = typeof(DeviceData);
            NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Name));
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Address));
            DESCRIPTION = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(DeviceData.Description));

            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty][Localization("DEVICE_DATA_NAME")] public string Name { get; set; }
        [JsonProperty][Localization("DEVICE_DATA_ADDRESS")] public string Address { get; set; }
        [JsonProperty][Localization("DEVICE_DATA_DESCRIPTION")] public string Description { get; set; }
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
