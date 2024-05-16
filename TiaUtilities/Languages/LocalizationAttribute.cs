using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Utility;
using TiaXmlReader.Utility.Extensions;
using System.Windows.Forms;

namespace TiaXmlReader.Languages
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public class LocalizationAttribute(string jsonKey, string append = "") : Attribute
    {
        private readonly string jsonKey = jsonKey;
        private readonly string append = append;

        public string GetTranslation() //Can be null!
        {
            return Localization.Get(this.jsonKey, this.append);
        }
    }
}
