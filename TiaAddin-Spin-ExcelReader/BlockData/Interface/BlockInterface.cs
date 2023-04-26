using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace SpinXmlReader.Block
{
    public class BlockInterface : IXMLNodeSerializable
    {
        private readonly Dictionary<SectionTypeEnum, Section> sectionDictionary;
        private readonly List<Section> sectionList;

        private BlockInterface()
        {
            this.sectionDictionary = new Dictionary<SectionTypeEnum, Section>();
            this.sectionList = new List<Section>();
        }

        internal BlockInterface(XmlNode node) : this()
        {
            DoXmlNode(node);
        }

        public BlockInterface(bool isFC) : this()
        {
            foreach (SectionTypeEnum type in Enum.GetValues(typeof(SectionTypeEnum)))
            {
                //Static exists only for FBs
                if (type == SectionTypeEnum.STATIC && !isFC)
                {
                    continue;
                }

                this.AddSection(new Section(type));
            }
        }

        public Section GetByType(SectionTypeEnum type)
        {
            return sectionDictionary[type];
        }

        public void ParseNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("Interface"), "BlockInterface node name is not valid.");

            sectionDictionary.Clear();
            sectionList.Clear();
            foreach (XmlNode sectionNode in XmlSearchEngine.Of(node).GetAllNodes("Sections/Section"))
            {
                this.AddSection(new Section(sectionNode));
            }
        }

        public XmlNode GenerateNode(XmlDocument document)
        {
            return XmlNodeBuilder.CreateNewWithNamespace(document, "Sections", TiaXmlReader.Properties.Resources.SECTIONS_NAMESPACE)
                .AppendSerializableCollectionAsChild(sectionList)
                .GetNode();
        }

        private void AddSection(Section section)
        {
            sectionList.Add(section);
            sectionDictionary.Add(section.Type, section);
        }

        XmlNode IXMLNodeSerializable.GenerateNode(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
