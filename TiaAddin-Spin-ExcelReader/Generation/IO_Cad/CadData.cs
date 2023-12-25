using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.Generation.IO_Cad
{
    public enum CadDataSiemensMemoryType { INPUT = 0, SAFE_INPUT = 1, OUTPUT = 2, SAFE_OUTPUT = 3, UNDEFINED = 99 }
    // Define an extension method in a non-nested static class.
    public static class CadDataMemoryTypeExtension
    {
        public static string GetInitial(this CadDataSiemensMemoryType type)
        {
            switch(type)
            {
                case CadDataSiemensMemoryType.INPUT:
                case CadDataSiemensMemoryType.SAFE_INPUT:
                    return "I";
                case CadDataSiemensMemoryType.OUTPUT:
                case CadDataSiemensMemoryType.SAFE_OUTPUT:
                    return "Q";
                default:
                    return "NULL";
            }
        }

        public static SimaticMemoryArea GetSimatic(this CadDataSiemensMemoryType type)
        {
            switch (type)
            {
                case CadDataSiemensMemoryType.INPUT:
                case CadDataSiemensMemoryType.SAFE_INPUT:
                    return SimaticMemoryArea.INPUT;
                case CadDataSiemensMemoryType.OUTPUT:
                case CadDataSiemensMemoryType.SAFE_OUTPUT:
                    return SimaticMemoryArea.OUTPUT;
                default:
                    return SimaticMemoryArea.UNDEFINED;
            }
        }
    }

    public class CadData
    {
        public static CadDataSiemensMemoryType GetSiemensMemoryType(string address)
        {
            if(string.IsNullOrEmpty(address))
            {
                return CadDataSiemensMemoryType.UNDEFINED;
            }

            if (address.ToUpper().StartsWith("E") || address.ToLower().StartsWith("I"))
            {
                return CadDataSiemensMemoryType.INPUT;
            }
            else if(address.ToUpper().StartsWith("SI") || address.ToUpper().StartsWith("SE"))
            {
                return CadDataSiemensMemoryType.SAFE_INPUT;
            }
            else if (address.ToUpper().StartsWith("A") || address.ToUpper().StartsWith("Q"))
            {
                return CadDataSiemensMemoryType.OUTPUT;
            }
            else if (address.ToUpper().StartsWith("SQ") || address.ToUpper().StartsWith("SA"))
            {
                return CadDataSiemensMemoryType.SAFE_OUTPUT;
            }

            return CadDataSiemensMemoryType.UNDEFINED;
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
            for(var x = 0; x < address.Length; x++)
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

        public String Address { get; set; }
        public String Comment1 { get; set; }
        public String Comment2 { get; set; }
        public String Comment3 { get; set; }
        public String Comment4 { get; set; }
        public String CadPage { get; set; }
        public String CadPanel { get; set; }
        public String CadType { get; set; }

        public CadDataSiemensMemoryType GetAddressType()
        {
            return CadData.GetSiemensMemoryType(Address);
        }

        public uint GetAddressBit()
        {
            return CadData.GetAddressBit(Address);
        }

        public uint GetAddressByte()
        {
            return CadData.GetAddressByte(Address);
        }
    }
}
