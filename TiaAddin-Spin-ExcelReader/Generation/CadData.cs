using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation
{

    public class CadData
    {
        public enum AddressType
        {
            INPUT,
            OUTPUT,
            UNDEFINED
        }

        public static AddressType GetAddressType(string address)
        {
            if(string.IsNullOrEmpty(address))
            {
                return AddressType.UNDEFINED;
            }

            if (address.ToUpper().StartsWith("E") || address.ToLower().StartsWith("I"))
            {
                return AddressType.INPUT;
            }
            else if (address.ToUpper().StartsWith("A") || address.ToLower().StartsWith("Q"))
            {
                return AddressType.OUTPUT;
            }

            return AddressType.UNDEFINED;
        }

        public static uint GetAddressBit(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            var substring = address.Substring(1);
            return uint.Parse(substring.Split('.')[0]);
        }

        public static uint GetAddressByte(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }

            var substring = address.Substring(1);
            return uint.Parse(substring.Split('.')[1]);
        }

        public String Address { get; set; }
        public String Comment1 { get; set; }
        public String Comment2 { get; set; }
        public String Comment3 { get; set; }
        public String Comment4 { get; set; }
        public String CadPage { get; set; }
        public String CadPanel { get; set; }
        public String CadType { get; set; }

        public AddressType GetAddressType()
        {
            return CadData.GetAddressType(Address);
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
