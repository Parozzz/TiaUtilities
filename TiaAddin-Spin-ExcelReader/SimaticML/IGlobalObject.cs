using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.SimaticML
{
    public class IDGenerator
    {
        public uint counter = 0;
        public void Reset()
        {
            counter = 0;
        }

        public uint GetNext()
        {
            return counter++;
        }

        public string GetNextHex()
        {
            return counter++.ToString("X");
        }

        public void SetHighest(uint highest)
        {
            if (highest > counter)
            {
                counter = highest;
            }
        }
    }

    public interface IGlobalObject
    {
        GlobalObjectData GetGlobalObjectData();
    }

    public class GlobalObjectData : XmlAttributeConfiguration
    {
        public GlobalObjectData() : base("ID", required: true)
        {
            //base.value = "" + GlobalIDGenerator.GetNextID().ToString("X"); //HEX
        }

        public string GetHexId()
        {
            return base.value;
        }

        public uint GetId()
        {
            return uint.Parse(base.value);
        }
    }

    public interface ILocalObject
    {
        LocalObjectData GetLocalObjectData();
    }

    public class LocalObjectData : XmlAttributeConfiguration
    {
        public LocalObjectData() : base("UId", required: true)
        {
        }

        public uint GetUId()
        {
            return uint.Parse(base.value);
        }
    }

}
