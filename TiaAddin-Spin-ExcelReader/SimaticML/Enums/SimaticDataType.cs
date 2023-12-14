using DocumentFormat.OpenXml.Office.CustomXsn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.SimaticML
{
    public enum SimaticDataType
    {
        VOID,
        BOOLEAN,
        BYTE,
        USINT,
        INT,
        UINT,
        DINT,
        UDINT,
        REAL,
        LREAL,
        TIMER,
        COUNTER,
        STRUCTURE,
    }

    public static class DataTypeExtension
    {
        public static string GetSimaticMLString(this SimaticDataType dataType)
        {
            switch (dataType)
            {
                case SimaticDataType.VOID: return "Void";
                case SimaticDataType.BOOLEAN: return "Bool";
                case SimaticDataType.BYTE: return "Byte";
                case SimaticDataType.USINT: return "USInt";
                case SimaticDataType.INT: return "Int";
                case SimaticDataType.UINT: return "UInt";
                case SimaticDataType.DINT: return "DInt";
                case SimaticDataType.UDINT: return "UDInt";
                case SimaticDataType.REAL: return "Real";
                case SimaticDataType.LREAL: return "LReal";
                case SimaticDataType.TIMER: return "Timer";
                case SimaticDataType.COUNTER: return "Counter";
                case SimaticDataType.STRUCTURE: return "Struct";
                default:
                    throw new Exception("SimaticML string not set for DataType " + dataType.ToString());
            }
        }

        public static uint GetSimaticLength(this SimaticDataType dataType)
        {
            switch (dataType)
            {
                case SimaticDataType.VOID: return 0;
                case SimaticDataType.BOOLEAN: return 0;
                case SimaticDataType.BYTE: return 1;
                case SimaticDataType.USINT: return 1;
                case SimaticDataType.INT: return 2;
                case SimaticDataType.UINT: return 2;
                case SimaticDataType.DINT: return 4;
                case SimaticDataType.UDINT: return 4;
                case SimaticDataType.REAL: return 4;
                case SimaticDataType.LREAL: return 8;
                case SimaticDataType.TIMER: return 0;
                case SimaticDataType.COUNTER: return 0;
                case SimaticDataType.STRUCTURE: return 0;
                default:
                    throw new Exception("SimaticML string not set for DataType " + dataType.ToString());
            }
        }

        public static string GetSimaticLengthIdentifier(this SimaticDataType dataType)
        {
            return SimaticMLUtil.GetLengthIdentifier(dataType.GetSimaticLength());
        }
    }

    public static class SimaticDataTypeUtil
    {
        public static SimaticDataType GetFromSimaticMLString(string str, bool throwException = false)
        {
            foreach(SimaticDataType dataType in Enum.GetValues(typeof(SimaticDataType)))
            {
                if(str.StartsWith(dataType.GetSimaticMLString())) //For arrays there will be other stuff after the type.
                {
                    return dataType;
                }
            }

            if(throwException)
            {
                throw new ArgumentException("SimaticDataType " + str + " has not been implemented yet.");
            }

            return SimaticDataType.VOID;
        }
    }
}
