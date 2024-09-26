using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.IO
{
    public enum IOGroupingTypeEnum
    {
        [Locale(nameof(Locale.IO_CONFIG_GROUPING_TYPE_BIT))] PER_BIT = 0, //DEFAULT
        [Locale(nameof(Locale.IO_CONFIG_GROUPING_TYPE_BYTE))] PER_BYTE
    }

    public enum IOMemoryTypeEnum
    {
        [Locale(nameof(Locale.IO_CONFIG_MEMORY_TYPE_DB))] DB = 0, //DEFAULT
        [Locale(nameof(Locale.IO_CONFIG_MEMORY_TYPE_MERKER))] MERKER
    }

    public class IOMainConfiguration : ObservableConfiguration
    {
        [JsonProperty] public IOMemoryTypeEnum MemoryType { get => this.GetAs<IOMemoryTypeEnum>(); set => this.Set(value); }
        [JsonProperty] public IOGroupingTypeEnum GroupingType { get => this.GetAs<IOGroupingTypeEnum>(); set => this.Set(value); }

        [JsonProperty] public string DBName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint DBNumber { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public bool GenerateDefinedVariableAnyway { get => this.GetAs<bool>(); set => this.Set(value); }

        [JsonProperty] public string IOTableName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint IOTableSplitEvery { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string DefaultIoName { get => this.GetAs<string>(); set => this.Set(value); }


        [JsonProperty] public string VariableTableName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint VariableTableInputStartAddress { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public uint VariableTableOutputStartAddress { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public uint VariableTableSplitEvery { get => this.GetAs<uint>(); set => this.Set(value); }

        [JsonProperty] public string DefaultDBInputVariable { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultDBOutputVariable { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultMerkerInputVariable { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DefaultMerkerOutputVariable { get => this.GetAs<string>(); set => this.Set(value); }

        public IOMainConfiguration()
        {
            this.MemoryType = IOMemoryTypeEnum.DB;
            this.GroupingType = IOGroupingTypeEnum.PER_BYTE;

            this.DBName = "TestIO_DB";
            this.DBNumber = 196;
            this.GenerateDefinedVariableAnyway = false;

            this.IOTableName = $"IN_OUT_{GenPlaceholders.Generation.TAB_NAME}Table";
            this.IOTableSplitEvery = 250;
            this.DefaultIoName = "{memory_type}{byte}_{bit}";

            this.VariableTableName = "VariableTable";
            this.VariableTableInputStartAddress = 100;
            this.VariableTableOutputStartAddress = 1000;
            this.VariableTableSplitEvery = 250;

            this.DefaultDBInputVariable = "{config_db_name}.IN.{io_name}";
            this.DefaultDBOutputVariable = "{config_db_name}.OUT.{io_name}";
            this.DefaultMerkerInputVariable = "MI_{byte}_{bit}";
            this.DefaultMerkerOutputVariable = "MO_{byte}_{bit}";
        }
    }
}
