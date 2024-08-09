using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation;

namespace TiaUtilities.Generation.IO
{
    public class IOTabConfiguration
    {
        [JsonProperty] public string FCBlockName = "fcTest_IO";
        [JsonProperty] public uint FCBlockNumber = 195;

        [JsonProperty] public string SegmentNameBitGrouping = "{memory_type}{byte}_{bit} - {comment}";
        [JsonProperty] public string SegmentNameByteGrouping = "{memory_type}B{byte}";

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
