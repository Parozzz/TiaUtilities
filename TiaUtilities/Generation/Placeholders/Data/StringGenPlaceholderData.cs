using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.Placeholders.Data
{
    public class StringGenPlaceholderData : IGenPlaceholderData
    {
        public string Value { get; set; }
        public Func<string, string> Function { get; set; }

        public StringGenPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function != null ? Function.Invoke(Value) : Value;
        }
    }

}
