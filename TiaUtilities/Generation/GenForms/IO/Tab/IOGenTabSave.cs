using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GenForms.Alarm.Tab;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.GridHandler;

namespace TiaUtilities.Generation.GenForms.IO.Tab
{
    public class IOGenTabSave
    {
        public const string EXTENSION = "json";

        [JsonProperty] public string Name { get; set; } = "IOGenTab";
        [JsonProperty] public IOTabConfiguration TabConfig { get; set; } = new();
        [JsonProperty] public GridSave<IOMainConfiguration, IOData> IOGrid { get; set; } = new();

        public static IOGenTabSave Load(ref string? filePath)
        {
            return GenUtils.Deserialize<IOGenTabSave>(ref filePath, EXTENSION) ?? new IOGenTabSave();
        }

        public bool Populate(ref string? filePath)
        {
            return GenUtils.Populate(this, ref filePath, EXTENSION);
        }

        public bool Save(ref string? filePath, bool saveAs = false)
        {
            return SavesLoader.Save(this, ref filePath, EXTENSION, saveAs);
        }

        public override bool Equals(object? obj)
        {
            return obj is IOGenTabSave compare &&
                Name.Equals(compare.Name) &&
                TabConfig.Equals(compare.TabConfig) &&
                IOGrid.Equals(compare.IOGrid);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
