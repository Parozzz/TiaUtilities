using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation.Placeholders.Data
{
    public class UIntGenerationPlaceholderData : IGenerationPlaceholderData
    {
        public uint Value { get; set; }
        public Func<uint, string> Function { get; set; }

        public UIntGenerationPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function.Invoke(Value);
        }
    }

}
