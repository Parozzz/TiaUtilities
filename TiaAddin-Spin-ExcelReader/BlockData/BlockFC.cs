using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class BlockFC : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.FC";
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

        public BlockFC() : base(BlockFC.NODE_NAME, main: true)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, BlockFC.CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public MultilingualText ComputeBlockTitle() //Add if does not exists.
        {
            foreach(var item in objectList.GetItems())
            {
                if(item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.TITLE)
                {
                    return text;
                }
            }

            var title = new MultilingualText(MultilingualTextType.TITLE);
            objectList.GetItems().Add(title);
            return title;
        }

        public MultilingualText ComputeBlockComment() //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.COMMENT)
                {
                    return text;
                }
            }

            var comment = new MultilingualText(MultilingualTextType.COMMENT);
            objectList.GetItems().Add(comment);
            return comment;
        }

        public BlockAttributeList GetBlockAttributes()
        {
            return blockAttributeList;
        }

        public CompileUnit AddCompileUnit()
        {
            var compileUnit = new CompileUnit();
            objectList.GetItems().Add(compileUnit);
            return compileUnit;
        }
    }

}
