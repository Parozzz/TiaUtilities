using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader
{
    //ONLY SAFE INSIDE THE XML GENERATION THREAD
    public static class GlobalIDGenerator
    {
        private static uint counter = 0;

        public static void ResetID()
        {
            counter = 0;
        }

        public static uint GetNextID()
        {
            var ret = counter;
            counter++;
            return ret;
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
            base.value = "" + GlobalIDGenerator.GetNextID().ToString("X"); //HEX
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
            base.value = "" + GlobalIDGenerator.GetNextID().ToString(); //UINT
        }

        public uint GetUId()
        {
            return uint.Parse(base.value);
        }
    }

}
