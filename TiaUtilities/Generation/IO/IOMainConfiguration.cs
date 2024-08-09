﻿using Esprima.Ast;
using Newtonsoft.Json;
using TiaXmlReader.AutoSave;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.IO
{
    public enum IOGroupingTypeEnum
    {
        [Localization("IO_CONFIG_GROUPING_TYPE_BIT")] PER_BIT = 0, //DEFAULT
        [Localization("IO_CONFIG_GROUPING_TYPE_BYTE")] PER_BYTE
    }

    public enum IOMemoryTypeEnum
    {
        [Localization("IO_CONFIG_MEMORY_TYPE_DB")] DB = 0, //DEFAULT
        [Localization("IO_CONFIG_MEMORY_TYPE_MERKER")] MERKER
    }

    public class IOMainConfiguration : IGenerationConfiguration, ISettingsAutoSave
    {
        [JsonProperty] public IOMemoryTypeEnum MemoryType = IOMemoryTypeEnum.DB;
        [JsonProperty] public IOGroupingTypeEnum GroupingType = IOGroupingTypeEnum.PER_BYTE;

        [JsonProperty] public string DBName = "TestIO_DB";
        [JsonProperty] public uint DBNumber = 196;
        [JsonProperty] public bool GenerateDefinedVariableAnyway = false;

        [JsonProperty] public string VariableTableName = "VariableTable";
        [JsonProperty] public uint VariableTableInputStartAddress = 100;
        [JsonProperty] public uint VariableTableOutputStartAddress = 1000;
        [JsonProperty] public uint VariableTableSplitEvery = 250;

        [JsonProperty] public string IOTableName = "IOTags";
        [JsonProperty] public uint IOTableSplitEvery = 250;

        [JsonProperty] public string DefaultIoName = "{memory_type}{byte}_{bit}";
        [JsonProperty] public string DefaultDBInputVariable = "{config_db_name}.IN.{io_name}";
        [JsonProperty] public string DefaultDBOutputVariable = "{config_db_name}.OUT.{io_name}";
        [JsonProperty] public string DefaultMerkerInputVariable = "MI_{byte}_{bit}";
        [JsonProperty] public string DefaultMerkerOutputVariable = "MO_{byte}_{bit}";

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenerationUtils.CompareJsonFieldsAndProperties(this, obj, out _);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
