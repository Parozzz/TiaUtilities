using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation.Placeholders;

namespace TiaUtilities.Generation.IO
{
    public class IOTabConfiguration : ObservableConfiguration
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
    }
}
