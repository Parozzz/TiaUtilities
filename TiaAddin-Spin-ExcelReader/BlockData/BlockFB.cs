using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class BlockFB : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.FB";
        public static XmlNodeConfiguration CreateObjectListConfiguration(XmlNode node)
        {
            switch(node.Name)
            {
                case MultilingualText.NODE_NAME:
                    return MultilingualText.CreateMultilingualText(node);
                case CompileUnit.NODE_NAME:
                    return new CompileUnit();
            }

            return null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockFB() : base(BlockFB.NODE_NAME, main: true)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, BlockFB.CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public MultilingualText GetBlockTitle()
        {
            foreach(var item in objectList.GetItems())
            {
                if(item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.TITLE)
                {
                    return text;
                }
            }

            return null;
        }

        public MultilingualText GetBlockComment()
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.COMMENT)
                {
                    return text;
                }
            }

            return null;
        }

        public BlockAttributeList GetAttributes()
        {
            return blockAttributeList;
        }
    }

}
