using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML;

namespace TiaXmlReader.SimaticML
{
    public class IDGenerator
    {
        private uint counter;

        public IDGenerator(uint counter = 0)
        {
            this.counter = counter;
        }

        public void Reset()
        {
            counter = 0;
        }

        public uint GetNext()
        {
            return counter++;
        }

        public string GetNextString()
        {
            return "" + counter++;
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
        public LocalObjectData(IDGenerator iDGenerator) : base("UId", required: true)
        {
            this.SetValue("" + iDGenerator.GetNext());
        }

        public uint GetUId()
        {
            return uint.Parse(base.value);
        }
    }

}
