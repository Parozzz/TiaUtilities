using System.Globalization;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML
{
    public class IDGenerator
    {
        private readonly uint startValue;
        private uint counter;

        public IDGenerator(uint startValue = 0)
        {
            this.startValue = startValue;
            this.counter = startValue;
        }

        public void Reset()
        {
            counter = startValue;
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
            return uint.Parse(base.value, NumberStyles.HexNumber);
        }
    }
    /*
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
    }*/

}
