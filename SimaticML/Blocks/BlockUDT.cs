using SimaticML.API;
using SimaticML.LanguageText;
using SimaticML.nBlockAttributeList;
using SimaticML.XMLClasses;
using System.Xml;

namespace SimaticML.Blocks
{
    public class BlockUDT : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Types.PlcStruct";
        public static XmlNodeConfiguration? CreateObjectListConfiguration(XmlNode node)
        {
            return node.Name switch
            {
                MultilingualText.NODE_NAME => MultilingualText.CreateMultilingualText(node),
                _ => null,
            };
        }

        public MultilingualText Title { get => this.ComputeMultilingualText(MultilingualTextType.TITLE); }
        public MultilingualText Comment { get => this.ComputeMultilingualText(MultilingualTextType.COMMENT); }
        public BlockAttributeList AttributeList { get => this.blockAttributeList; }

        private readonly GlobalObjectData globalObjectData;
        private readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockUDT() : base(NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList(isUDT: true));

            this.objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            this.Title[SimaticMLAPI.CULTURE] = "";
            this.Comment[SimaticMLAPI.CULTURE] = "";

            //Generation section
            var _ = blockAttributeList.NONE;
        }

        private MultilingualText ComputeMultilingualText(MultilingualTextType textType) //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.TextType == textType)
                {
                    return text;
                }
            }

            var comment = new MultilingualText(textType);
            objectList.GetItems().Add(comment);
            return comment;
        }
    }
}
