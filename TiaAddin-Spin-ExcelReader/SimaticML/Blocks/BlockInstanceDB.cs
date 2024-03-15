using SpinXmlReader.SimaticML;
using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.nBlockAttributeList;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class BlockInstanceDB : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.InstanceDB";
        public static XmlNodeConfiguration CreateObjectListConfiguration(XmlNode node)
        {
            switch (node.Name)
            {
                case MultilingualText.NODE_NAME:
                    return MultilingualText.CreateMultilingualText(node);
            }

            return null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockInstanceDB() : base(BlockInstanceDB.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, BlockInstanceDB.CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            this.ComputeBlockTitle().SetText(Constants.DEFAULT_CULTURE, "");
            this.ComputeBlockComment().SetText(Constants.DEFAULT_CULTURE, "");

            blockAttributeList.ComputeSection(SectionTypeEnum.STATIC);

            blockAttributeList.SetBlockProgrammingLanguage(SimaticProgrammingLanguage.DB)
                .SetHeaderAuthor(Constants.HEADER_AUTHOR)
                .SetHeaderFamily(Constants.HEADER_FAMILY);
        }

        public MultilingualText ComputeBlockTitle() //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.TITLE)
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

        public BlockAttributeList GetAttributes()
        {
            return blockAttributeList;
        }
    }

}
