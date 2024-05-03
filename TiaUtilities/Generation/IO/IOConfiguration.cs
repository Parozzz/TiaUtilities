using DocumentFormat.OpenXml;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.AutoSave;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Localization;

namespace TiaXmlReader.Generation.IO
{
    public enum IOGroupingTypeEnum
    {
        [Localization("IO_CONFIG_GROUPING_BIT")] PER_BIT,
        [Localization("IO_CONFIG_GROUPING_BYTE")] PER_BYTE
    }

    public enum IOMemoryTypeEnum
    {
        [Localization("IO_CONFIG_MEMORY_DB")] DB = 0, //DEFAULT
        [Localization("IO_CONFIG_MEMORY_MERKER")] MERKER
    }

    public class IOConfiguration : IGenerationConfiguration, ISettingsAutoSave
    {
        [JsonProperty] public string FCBlockName = "fcTest_IO";
        [JsonProperty] public uint FCBlockNumber = 195;

        [JsonProperty] public IOMemoryTypeEnum MemoryType = IOMemoryTypeEnum.DB;
        [JsonProperty] public IOGroupingTypeEnum GroupingType = IOGroupingTypeEnum.PER_BYTE;

        [JsonProperty] public string DBName = "TestIO_DB";
        [JsonProperty] public uint DBNumber = 196;

        [JsonProperty] public string VariableTableName = "VariableTable";
        [JsonProperty] public uint VariableTableInputStartAddress = 100;
        [JsonProperty] public uint VariableTableOutputStartAddress = 1000;
        [JsonProperty] public uint VariableTableSplitEvery = 250;

        [JsonProperty] public string IOTableName = "IOTags";
        [JsonProperty] public uint IOTableSplitEvery = 250;

        [JsonProperty] public string DefaultIoName = "{memory_type}{byte}_{bit}";
        [JsonProperty] public string DefaultVariableName = "{io_name}";

        [JsonProperty] public string SegmentNameBitGrouping = "{memory_type}{byte}_{bit} - {comment}";
        [JsonProperty] public string SegmentNameByteGrouping = "{memory_type}B{byte}";

        [JsonProperty] public string PrefixInputDB = "IN.";
        [JsonProperty] public string PrefixInputMerker = "MI_";
        [JsonProperty] public string PrefixOutputDB = "OUT.";
        [JsonProperty] public string PrefixOutputMerker = "MO_";

    }
}
