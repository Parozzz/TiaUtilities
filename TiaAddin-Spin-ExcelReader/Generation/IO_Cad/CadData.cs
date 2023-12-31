using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.Generation.IO_Cad
{
    public enum CadDataMemoryArea { INPUT = 1, SAFE_INPUT = 2, OUTPUT = 3, SAFE_OUTPUT = 4, UNDEFINED = 0 } //Default = Undefined
    // Define an extension method in a non-nested static class.
    public static class CadDataMemoryAreaExtension
    {
        public static string GetInitial(this CadDataMemoryArea type)
        {
            switch (type)
            {
                case CadDataMemoryArea.INPUT:
                case CadDataMemoryArea.SAFE_INPUT:
                    return "I";
                case CadDataMemoryArea.OUTPUT:
                case CadDataMemoryArea.SAFE_OUTPUT:
                    return "Q";
                default:
                    return "NULL";
            }
        }

        public static SimaticMemoryArea GetSimatic(this CadDataMemoryArea type)
        {
            switch (type)
            {
                case CadDataMemoryArea.INPUT:
                case CadDataMemoryArea.SAFE_INPUT:
                    return SimaticMemoryArea.INPUT;
                case CadDataMemoryArea.OUTPUT:
                case CadDataMemoryArea.SAFE_OUTPUT:
                    return SimaticMemoryArea.OUTPUT;
                default:
                    return SimaticMemoryArea.UNDEFINED;
            }
        }
    }

    public class CadData
    {
        public static CadDataMemoryArea GetMemoryArea(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return CadDataMemoryArea.UNDEFINED;
            }

            if (address.ToUpper().StartsWith("E") || address.ToLower().StartsWith("I"))
            {
                return CadDataMemoryArea.INPUT;
            }
            else if (address.ToUpper().StartsWith("SI") || address.ToUpper().StartsWith("SE"))
            {
                return CadDataMemoryArea.SAFE_INPUT;
            }
            else if (address.ToUpper().StartsWith("A") || address.ToUpper().StartsWith("Q"))
            {
                return CadDataMemoryArea.OUTPUT;
            }
            else if (address.ToUpper().StartsWith("SQ") || address.ToUpper().StartsWith("SA"))
            {
                return CadDataMemoryArea.SAFE_OUTPUT;
            }

            return CadDataMemoryArea.UNDEFINED;
        }

        public static string GetCadMemoryType(string address)
        {
            int firstDigit = 0;
            for (var x = 0; x < address.Length; x++)
            {
                var c = address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            return address.Substring(0, firstDigit);
        }

        public static uint GetAddressBit(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < address.Length; x++)
            {
                var c = address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[1]);
        }

        public static uint GetAddressByte(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < address.Length; x++)
            {
                var c = address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[0]);
        }

        public string IOName;
        public string VariableName;
        public string DBName;
        public string CadAddress;
        public string IOComment;
        public string VariableComment;
        public string Comment1;
        public string Comment2;
        public string Comment3;
        public string Comment4;
        public string Mnemonic;
        public string WireNum;
        public string Page;
        public string Panel;

        //Returns if any change has been made.
        public bool ParsePlaceholders(GenerationPlaceholders placeholders) => placeholders.ParseWithResult(ref IOName)
                                                                                || placeholders.ParseWithResult(ref VariableName)
                                                                                || placeholders.ParseWithResult(ref DBName)
                                                                                || placeholders.ParseWithResult(ref IOComment)
                                                                                || placeholders.ParseWithResult(ref VariableComment)
                                                                                || placeholders.ParseWithResult(ref Comment1)
                                                                                || placeholders.ParseWithResult(ref Comment2)
                                                                                || placeholders.ParseWithResult(ref Comment3)
                                                                                || placeholders.ParseWithResult(ref Comment4);

        public SimaticMemoryArea GetSimaticMemoryArea()
        {
            return CadData.GetMemoryArea(CadAddress).GetSimatic();
        }

        public CadDataMemoryArea GetCadMemoryArea()
        {
            return CadData.GetMemoryArea(CadAddress);
        }

        public uint GetAddressBit()
        {
            return CadData.GetAddressBit(CadAddress);
        }

        public uint GetAddressByte()
        {
            return CadData.GetAddressByte(CadAddress);
        }
    }
}
