using System.Xml;
using SimaticML.API;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.nBlockAttributeList;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks
{
    public class BlockFC : XmlNodeConfiguration, IProgramBlock, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.FC";
        public static XmlNodeConfiguration? CreateObjectListConfiguration(XmlNode node)
        {
            return node.Name switch
            {
                MultilingualText.NODE_NAME => MultilingualText.CreateMultilingualText(node),
                CompileUnit.NODE_NAME => new CompileUnit(),
                _ => null,
            };
        }

        public MultilingualText Title { get => this.ComputeMultilingualText(MultilingualTextType.TITLE); }
        public MultilingualText Comment { get => this.ComputeMultilingualText(MultilingualTextType.COMMENT); }
        public BlockAttributeList AttributeList { get => this.blockAttributeList; }

        private readonly GlobalObjectData globalObjectData;
        private readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockFC() : base(BlockFC.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, BlockFC.CreateObjectListConfiguration, required: true);
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

            blockAttributeList.SetENOAutomatically = true;
            blockAttributeList.ProgrammingLanguage = SimaticProgrammingLanguage.LADDER;
            blockAttributeList.UDABlockProperties = SimaticMLAPI.UDA_BLOCK_PROPERTIES;
            blockAttributeList.MemoryLayout = "Optimized";
            blockAttributeList.HeaderAuthor = SimaticMLAPI.HEADER_AUTHOR;
            blockAttributeList.HeaderFamily = SimaticMLAPI.HEADER_FAMILY;

            //Generation section
            var _ = blockAttributeList.INPUT;
            _ = blockAttributeList.OUTPUT;
            _ = blockAttributeList.INOUT;
            _ = blockAttributeList.TEMP;
            _ = blockAttributeList.CONSTANT;
            blockAttributeList.RETURN.SetVoidReturnRetValMember();
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

        public CompileUnit AddCompileUnit()
        {
            var compileUnit = new CompileUnit();
            objectList.GetItems().Add(compileUnit);
            return compileUnit;
        }
    }

}
