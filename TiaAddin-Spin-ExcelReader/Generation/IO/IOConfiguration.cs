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
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.IO
{
    public enum IOMemoryTypeEnum
    {
        [Display(Description = "MEMORY_TYPE_DB", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        DB = 0, //DEFAULT
        [Display(Description = "MEMORY_TYPE_MERKER", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        MERKER
    }

    public enum IOGroupingTypeEnum
    {
        [Display(Description = "GROUPING_TYPE_BIT", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        PER_BIT,
        [Display(Description = "GROUPING_TYPE_BYTE", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        PER_BYTE
    }

    public class IOConfiguration : IGenerationConfiguration
    {
        [JsonProperty] public string FCBlockName = "fcTest_IO";
        [JsonProperty] public uint FCBlockNumber = 195;

        [JsonProperty] public IOMemoryTypeEnum MemoryType = IOMemoryTypeEnum.DB;
        [JsonProperty] public IOGroupingTypeEnum GroupingType = IOGroupingTypeEnum.PER_BYTE;

        [JsonProperty] public string DBName = "TestIO_DB";
        [JsonProperty] public uint DBNumber = 196;

        [JsonProperty] public string VariableTableName = "VariableTable";
        [JsonProperty] public uint VariableTableStartAddress = 2890;
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
