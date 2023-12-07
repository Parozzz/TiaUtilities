using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.SimaticML
{

    public enum SimaticMemoryArea
    {
        MERKER,
        INPUT,
        OUTPUT,
        TIMER,
        COUNTER
    }

    public static class SimaticMemoryAreaExtension
    {
        public static string GetTIAMnemonic(this SimaticMemoryArea memoryArea)
        {
            switch (memoryArea)
            {
                case SimaticMemoryArea.INPUT: return "I";
                case SimaticMemoryArea.OUTPUT: return "Q";
                case SimaticMemoryArea.MERKER: return "M";
                case SimaticMemoryArea.TIMER: return "T";
                    case SimaticMemoryArea.COUNTER: return "C";
                default:
                    throw new Exception("SimaticML string not set for MemoryArea " + memoryArea.ToString());
            }
        }
    }
}
