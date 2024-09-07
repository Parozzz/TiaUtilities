using Newtonsoft.Json;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;
using SimaticML;
using SimaticML.Enums;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.IO
{
    public class IOData : IGridData
    {
        private readonly static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn IO_NAME;
        public static readonly GridDataColumn VARIABLE;
        public static readonly GridDataColumn MERKER_ADDRESS;
        public static readonly GridDataColumn COMMENT;
        public static readonly IReadOnlyList<GridDataColumn> COLUMN_LIST;

        static IOData()
        {
            var type = typeof(IOData);
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Address));
            IO_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.IOName), "ioName");
            VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Variable));
            MERKER_ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.MerkerAddress), "merkerAddress");
            COMMENT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Comment));

            var columnList = GridDataColumn.GetStaticColumnList(type);
            columnList.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
            COLUMN_LIST = columnList.AsReadOnly();
        }

        [JsonProperty][Localization("IO_DATA_ADDRESS")] public string? Address { get; set; }
        [JsonProperty][Localization("IO_DATA_IO_NAME", append: " > " + GenPlaceholders.IO.IONAME)] public string? IOName { get; set; }
        [JsonProperty][Localization("IO_DATA_VARIABLE", append: " > " + GenPlaceholders.IO.VARIABLE)] public string? Variable { get; set; }
        [JsonProperty][Localization("IO_DATA_MERKER_ADDRESS")] public string? MerkerAddress { get; set; }
        [JsonProperty][Localization("IO_DATA_COMMENT", append: " > " + GenPlaceholders.IO.COMMENT)] public string? Comment { get; set; }

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

        public void LoadDefaults(GridDataPreviewer<IOData> previewer, IOMainConfiguration config, out bool ioNameDefault, out bool variableDefault, out bool merkerAddressDefault)
        {
            ioNameDefault = variableDefault = merkerAddressDefault = false;

            if (string.IsNullOrEmpty(IOName))
            {
                ioNameDefault = true;

                var preview = previewer.RequestPreview(IOData.IO_NAME, this);
                IOName = preview?.ComposeDefaultValue();
            }

            if (string.IsNullOrEmpty(Variable))
            {
                variableDefault = true;

                var preview = previewer.RequestPreview(IOData.VARIABLE, this);
                Variable = preview?.ComposeDefaultValue();
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

        public void ParsePlaceholders(IOGenPlaceholderHandler placeholders)
        {
            Address = placeholders.Parse(Address);
            IOName = placeholders.Parse(IOName);
            Variable = placeholders.Parse(Variable);
            MerkerAddress = placeholders.Parse(MerkerAddress);
            Comment = placeholders.Parse(Comment);
        }

        public void Clear()
        {
            this.Address = this.IOName = this.Variable = this.Comment = null;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(Variable);
        }

        public SimaticMemoryArea GetAddressMemoryArea()
        {
            var tagAddress = this.GetTagAddress();
            return tagAddress == null ? default : tagAddress.MemoryArea;
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

        public SimaticTagAddress? GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(Address);
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
