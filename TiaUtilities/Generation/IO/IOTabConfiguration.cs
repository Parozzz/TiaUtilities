using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Placeholders;
using TiaXmlReader.GenerationForms;

namespace TiaUtilities.Generation.IO
{
    public class IOTabConfiguration : ObservableConfiguration, IGenerationConfiguration
    {
        [JsonProperty] public string FCBlockName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint FCBlockNumber { get => this.GetAs<uint>(); set => this.Set(value); }

        [JsonProperty] public string SegmentNameBitGrouping { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string SegmentNameByteGrouping { get => this.GetAs<string>(); set => this.Set(value); }

        public IOTabConfiguration()
        {
            FCBlockName = $"fc{GenPlaceholders.Generation.TAB_NAME}";
            FCBlockNumber = 195;

            SegmentNameBitGrouping = "{memory_type}{byte}_{bit} - {comment}";
            SegmentNameByteGrouping = "{memory_type}B{byte}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out _);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
