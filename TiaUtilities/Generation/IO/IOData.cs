using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Generation;
using TiaXmlReader.SimaticML.Enums;
using System.ComponentModel.DataAnnotations;
using TiaXmlReader.Localization;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.IO
{
    public class IOData : IGridData<IOConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn IO_NAME;
        public static readonly GridDataColumn VARIABLE;
        public static readonly GridDataColumn MERKER_ADDRESS;
        public static readonly GridDataColumn COMMENT;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static IOData()
        {
            var type = typeof(IOData);
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Address));
            IO_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.IOName), "ioName");
            //DB_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.DBName), "dbName");
            VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Variable));
            MERKER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.MerkerAddress), "merkerAddress");
            COMMENT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Comment));

            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty][Localization("IO_DATA_ADDRESS")] public string Address { get; set; }
        [JsonProperty][Localization("IO_DATA_IO_NAME")] public string IOName { get; set; }
        [JsonProperty][Localization("IO_DATA_VARIABLE")] public string Variable { get; set; }
        [JsonProperty][Localization("IO_DATA_MERKER_ADDRESS")] public string MerkerAddress { get; set; }
        [JsonProperty][Localization("IO_DATA_COMMENT")] public string Comment { get; set; }

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

        public GridDataPreview GetPreview(int column, IOConfiguration config)
        {
            var addressTag = SimaticTagAddress.FromAddress(this.Address);
            if (string.IsNullOrEmpty(this.Address) || this.IsEmpty() || addressTag == null)
            {
                return null;
            }

            if (column == IO_NAME)
            {
                if (string.IsNullOrEmpty(config.DefaultIoName) && string.IsNullOrEmpty(this.IOName))
                {
                    return null;
                }

                return new GridDataPreview()
                {
                    DefaultValue = config.DefaultIoName,
                    Value = this.IOName
                };
            }
            else if (column == VARIABLE)
            {
                if (string.IsNullOrEmpty(config.DefaultVariableName) && string.IsNullOrEmpty(this.Variable))
                {
                    return null;
                }

                var prefix = "";
                if(string.IsNullOrEmpty(this.Variable))
                {
                    if (config.MemoryType == IOMemoryTypeEnum.DB)
                    {
                        prefix = config.DBName + "." + (this.GetAddressMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB);
                    }
                    else if (config.MemoryType == IOMemoryTypeEnum.MERKER)
                    {
                        prefix = this.GetAddressMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputMerker : config.PrefixOutputMerker;
                    }
                }
                return new GridDataPreview()
                {
                    Prefix = prefix,
                    DefaultValue = config.DefaultVariableName,
                    Value = this.Variable
                };
            }
            else if (column == MERKER_ADDRESS)
            {
                var merkerTag = new SimaticTagAddress();
                merkerTag.MemoryArea = SimaticMemoryArea.MERKER;
                merkerTag.ByteOffset = addressTag.ByteOffset + (addressTag.MemoryArea == SimaticMemoryArea.INPUT ? config.VariableTableInputStartAddress : config.VariableTableOutputStartAddress);
                merkerTag.BitOffset = addressTag.BitOffset;
                merkerTag.Length = 0; //BIT
                return new GridDataPreview()
                {
                    DefaultValue = merkerTag.ToString(),
                    Value = this.MerkerAddress,
                };
            }

            return null;
        }

        public void LoadDefaults(IOConfiguration config, out bool ioNameDefault, out bool variableDefault, out bool merkerAddressDefault)
        {
            ioNameDefault = variableDefault = merkerAddressDefault = false;

            if (string.IsNullOrEmpty(IOName))
            {
                ioNameDefault = true;
                IOName = config.DefaultIoName;
            }

            if (string.IsNullOrEmpty(Variable))
            {
                variableDefault = true;
                Variable = config.DefaultVariableName;
            }

            var variableAddressTag = SimaticTagAddress.FromAddress(this.MerkerAddress);
            if(variableAddressTag == null || variableAddressTag.MemoryArea != SimaticMemoryArea.MERKER)
            {
                var addressTag = SimaticTagAddress.FromAddress(this.Address);
                if (addressTag != null)
                {
                    merkerAddressDefault = true;

                    addressTag.MemoryArea = SimaticMemoryArea.MERKER;
                    addressTag.ByteOffset += addressTag.MemoryArea == SimaticMemoryArea.INPUT ? config.VariableTableInputStartAddress : config.VariableTableOutputStartAddress;
                    this.MerkerAddress = addressTag.ToString();
                }
            }
        }

        public void ParsePlaceholders(GenerationPlaceholders placeholders)
        {
            Address = placeholders.Parse(Address);
            IOName = placeholders.Parse(IOName);
            //DBName = placeholders.Parse(DBName);
            Variable = placeholders.Parse(Variable);
            MerkerAddress = placeholders.Parse(MerkerAddress);
            Comment = placeholders.Parse(Comment);
        }

        public void Clear()
        {
            this.Address = this.IOName/* = this.DBName*/ = this.Variable = this.Comment = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName)/* && string.IsNullOrEmpty(DBName)*/ && string.IsNullOrEmpty(Variable);
        }

        public SimaticMemoryArea GetAddressMemoryArea()
        {
            return SimaticMemoryAreaUtil.GetFromAddress(Address);
        }

        public uint GetAddressBit()
        {
            var tag = GetTagAddress();
            return tag == null ? 0 : tag.BitOffset;
        }

        public uint GetAddressByte()
        {
            var tag = GetTagAddress();
            return tag == null ? 0 : tag.ByteOffset;
        }

        public SimaticTagAddress GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(Address);
        }
    }
}
