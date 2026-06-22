using SimaticML.API;
using SimaticML.Attributes;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Blocks.FlagNet.nAccess;
using SimaticML.Blocks.FlagNet.nCall;
using SimaticML.Blocks.FlagNet.nPart;
using SimaticML.LanguageText;
using SimaticML.nBlockAttributeList;
using SimaticML.TagTable;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace SimaticML.XMLClasses
{
    public static class XmlNodeGenerator
    {
        private static readonly Dictionary<string, XmlNodeGeneratorFunction> nodesGeneratorDict = [];

        private static void FillDict()
        {
            nodesGeneratorDict.Add(XMLTagTable.NODE_NAME, (node, parentConfiguration) => new XMLTagTable());
            nodesGeneratorDict.Add(XMLTag.NODE_NAME, (node, parentConfiguration) => new XMLTag());
            nodesGeneratorDict.Add(XMLUserConstant.NODE_NAME, (node, parentConfiguration) => new XMLUserConstant());

            nodesGeneratorDict.Add(Comment.NODE_NAME, (node, parentConfiguration) => new Comment());
            nodesGeneratorDict.Add(MultilingualText.NODE_NAME, (node, parentConfiguration) => MultilingualText.CreateMultilingualText(node));

            nodesGeneratorDict.Add(BlockAttributeList.NODE_NAME, BlockAttributeList.CreateBlockAttributeList);
            nodesGeneratorDict.Add(Member.NODE_NAME, (node, parentConfiguration) => Member.CreateMember(node));
            nodesGeneratorDict.Add(Section.NODE_NAME, (node, parentConfiguration) => new Section());
            nodesGeneratorDict.Add(SubElement.NODE_NAME, (node, parentConfiguration) => SubElement.CreateSubElement(node));

            //BLOCKS
            nodesGeneratorDict.Add(BlockGlobalDB.NODE_NAME, (node, parentConfiguration) => new BlockGlobalDB());
            nodesGeneratorDict.Add(BlockInstanceDB.NODE_NAME, (node, parentConfiguration) => new BlockInstanceDB());
            nodesGeneratorDict.Add(BlockUDT.NODE_NAME, (node, parentConfiguration) => new BlockUDT());
            nodesGeneratorDict.Add(BlockFC.NODE_NAME, (node, parentConfiguration) => new BlockFC());
            nodesGeneratorDict.Add(BlockFB.NODE_NAME, (node, parentConfiguration) => new BlockFB());
            nodesGeneratorDict.Add(CompileUnit.NODE_NAME, (node, parentConfiguration) => new CompileUnit());

            //FLAG NET
            nodesGeneratorDict.Add(LabelDeclaration.NODE_NAME, (node, parentConfiguration) => new LabelDeclaration());
            nodesGeneratorDict.Add(Wire.NODE_NAME, Wire.CreateWire);
            nodesGeneratorDict.Add(Access.NODE_NAME, (node, parentConfiguration) => new Access());
            /*
             * NOT IMPLEMENTED CORRECTLY!
            nodesGeneratorDict.Add(Call.NODE_NAME, (node, parentConfiguration) => new Call());
            nodesGeneratorDict.Add(CallInfo.NODE_NAME, (node, parentConfiguration) => new CallInfo());
            nodesGeneratorDict.Add(CallParameter.NODE_NAME, (node, parentConfiguration) => new CallParameter());
            nodesGeneratorDict.Add(CallInstance.NODE_NAME, (node, parentConfiguration) => new CallInstance());*/
            nodesGeneratorDict.Add(Part.NODE_NAME, (node, parentConfiguration) => new Part());

            //ATTRIBUTES
            nodesGeneratorDict.Add(BooleanAttribute.NODE_NAME, (node, parentConfiguration) => new BooleanAttribute());
            nodesGeneratorDict.Add(StringAttribute.NODE_NAME, (node, parentConfiguration) => new StringAttribute());
            nodesGeneratorDict.Add(IntegerAttribute.NODE_NAME, (node, parentConfiguration) => new IntegerAttribute());
        }
 
        public static bool TryGenerateFromNode(XmlNode node, XmlNodeConfiguration parentConfiguration, [NotNullWhen(true)] out XmlNodeConfiguration? configuration)
        {
            if(nodesGeneratorDict.Count == 0)
            {
                FillDict();
            }

            //configuration = null;

            var nodeName = node.Name;
            if (nodesGeneratorDict.TryGetValue(nodeName, out var generator))
            {
                var generatedConfiguration = generator(node, parentConfiguration);
                if(generatedConfiguration != null)
                {
                    configuration = generatedConfiguration;
                    return true;
                }
            }

            configuration = default;
            return false;
        }
    }

    public delegate XmlNodeConfiguration? XmlNodeGeneratorFunction(XmlNode node, XmlNodeConfiguration parentConfiguration);

}
