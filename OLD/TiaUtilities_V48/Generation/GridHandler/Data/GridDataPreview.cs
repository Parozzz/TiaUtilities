using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.GridHandler.Data
{
    public class GridDataPreview
    {
        public string Prefix { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public string Suffix { get; set; }

        public string ComposeDefaultValue()
        {
            return Prefix ?? "" + DefaultValue ?? "" + Suffix ?? "";
        }
    }
}
