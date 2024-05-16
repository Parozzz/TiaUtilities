using System.Xml;
using TiaXmlReader.SimaticML.nBlockAttributeList;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.XMLClasses;
using TiaXmlReader.Languages;

namespace TiaXmlReader.SimaticML.Blocks
{
    public class BlockUDT : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Types.PlcStruct";
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

        public BlockUDT() : base(NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            this.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, "");
            this.ComputeBlockComment().SetText(LocalizationVariables.CULTURE, "");

            blockAttributeList.ComputeSection(SectionTypeEnum.NONE);
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
