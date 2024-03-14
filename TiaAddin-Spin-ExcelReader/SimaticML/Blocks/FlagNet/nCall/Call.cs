using SpinXmlReader.Block;
using SpinXmlReader.SimaticML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nCall
{

    public class Call : XmlNodeConfiguration, ILocalObject
    { 
        public const string NODE_NAME = "Call";

        private readonly LocalObjectData localObjectData;

        private readonly XmlAttributeConfiguration callName;
        private readonly XmlAttributeConfiguration blockType;

        public Call(CompileUnit compileUnit) : base(Call.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData(compileUnit.LocalIDGenerator));

            callName = this.AddAttribute("Name");
            blockType = this.AddAttribute("BlockType", required: true);
            //==== INIT CONFIGURATION ====
        }

        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }

        public Call SetCallName(string callName)
        {
            this.callName.SetValue(callName);
            return this;
        }

        public string GetCallName()
        {
            return this.callName.GetValue();
        }

        public Call SetBlockType(SimaticBlockType blockType)
        {
            this.blockType.SetValue(blockType.GetSimaticMLString());
            return this;
        }

        public SimaticBlockType GetBlockType()
        {
            foreach(SimaticBlockType blockType in Enum.GetValues(typeof(SimaticBlockType)))
            {
                if(this.blockType.GetValue() == blockType.GetSimaticMLString())
                {
                    return blockType;
                }
            }

            return default;
        }
    }
}
