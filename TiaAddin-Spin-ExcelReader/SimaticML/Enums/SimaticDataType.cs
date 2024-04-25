using DocumentFormat.OpenXml.Office.CustomXsn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.SimaticML.Enums
{
    public class SimaticDataType
    {
        public static SimaticDataType VOID = new SimaticDataType("Void", 0);
        public static SimaticDataType BOOLEAN = new SimaticDataType("Bool", 0);
        public static SimaticDataType BYTE = new SimaticDataType("Byte", 1);
        public static SimaticDataType USINT = new SimaticDataType("USInt", 1);
        public static SimaticDataType WORD = new SimaticDataType("Word", 2);
        public static SimaticDataType INT = new SimaticDataType("Int", 2);
        public static SimaticDataType UINT = new SimaticDataType("UInt", 2);
        public static SimaticDataType DWORD = new SimaticDataType("DWord", 4);
        public static SimaticDataType DINT = new SimaticDataType("DInt", 4);
        public static SimaticDataType UDINT = new SimaticDataType("UDInt", 4);
        public static SimaticDataType LWORD = new SimaticDataType("LWord", 8);
        public static SimaticDataType REAL = new SimaticDataType("Real", 8);
        public static SimaticDataType LREAL = new SimaticDataType("LReal", 8);
        public static SimaticDataType TIMER = new SimaticDataType("Timer", 0);
        public static SimaticDataType COUNTER = new SimaticDataType("Counter", 0);
        public static SimaticDataType STRUCTURE = new SimaticDataType("Struct", 0);
        public static SimaticDataType VARIANT = new SimaticDataType("Any", 0);

        private static Dictionary<string, SimaticDataType> TYPE_DICT;
        static SimaticDataType()
        {
            TYPE_DICT = new Dictionary<string, SimaticDataType>();
            foreach (var field in typeof(SimaticDataType).GetFields().Where(f => f.IsStatic && f.FieldType == typeof(SimaticDataType)))
            {
                var dataType = (SimaticDataType)field.GetValue(null);
                SimaticDataType.AddDataType(dataType);
            }
        }

        public static SimaticDataType FromSimaticMLString(string simaticString, bool throwException = false)
        {
            if (!SimaticDataType.TYPE_DICT.ContainsKey(simaticString))
            {
                if(throwException)
                {
                    throw new ArgumentException("SimaticDataType " + simaticString + " has not been implemented yet.");
                }

                return SimaticDataType.VOID;
            }

            return SimaticDataType.TYPE_DICT[simaticString];
        }

        public static void AddDataType(SimaticDataType type)
        {
            var simaticString = type.GetSimaticMLString();
            if (TYPE_DICT.ContainsKey(simaticString))
            {
                throw new ArgumentException("SimaticDataType already exists inside TYPE_DICT");
            }

            TYPE_DICT.Add(simaticString, type);
        }

        private readonly string simaticMLString;
        private readonly uint simaticMLLength;
        public SimaticDataType(string simaticMLString, uint simaticLength)
        {
            this.simaticMLString = simaticMLString;
            this.simaticMLLength = simaticLength;
        }

        public string GetSimaticMLString()
        {
            return simaticMLString;
        }

        public uint GetSimaticLength()
        {
            return simaticMLLength;
        }

        public string GetSimaticLengthIdentifier()
        {
            return SimaticMLUtil.GetLengthIdentifier(simaticMLLength);
        }
    }
}
