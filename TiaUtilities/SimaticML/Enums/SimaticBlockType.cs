using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.SimaticML.Enums
{

    public enum SimaticBlockType
    {
        DB = 0, //Default
        FB,
        FC,
        OB,
        UDT,
        FBT,//??
        FCT //??
    }

    public static class SimaticBlockTypeExtension
    {
        public static string GetSimaticMLString(this SimaticBlockType blockType)
        {
            switch (blockType)
            {
                case SimaticBlockType.DB: return "DB";
                case SimaticBlockType.FB: return "FB";
                case SimaticBlockType.FC: return "FC";
                case SimaticBlockType.OB: return "OB";
                case SimaticBlockType.UDT: return "UDT";
                case SimaticBlockType.FBT: return "FBT";
                case SimaticBlockType.FCT: return "FCT";
                default:
                    throw new Exception("SimaticML string not set for BlockType " + blockType.ToString());
            }
        }
    }


}

