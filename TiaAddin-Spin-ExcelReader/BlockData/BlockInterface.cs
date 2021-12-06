using System;
using System.Collections.Generic;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;
using static TiaAddin_Spin_ExcelReader.BlockData.BlockInterface.Section;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class BlockInterface
    {
        private readonly FCData fcData;
        private readonly Dictionary<SectionTypeEnum, Section> sectionDictionary;
        private readonly List<Section> sectionList;

        internal BlockInterface(FCData fcData)
        {
            this.fcData = fcData;
            this.sectionDictionary = new Dictionary<SectionTypeEnum, Section>();
            this.sectionList = new List<Section>();
        }
        public Section GetByType(SectionTypeEnum type)
        {
            return sectionDictionary[type];
        }

        internal void ParseXmlNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("Interface"), "BlockInterface node name is not valid.");

            foreach (XmlNode sectionNode in XmlSearchEngine.Of(node).AddSearch("Sections/Section").GetAllNodes())
            {
                var section = new Section().ParseXmlNode(sectionNode);
                if (section != null)
                {
                    sectionList.Add(section);
                    sectionDictionary.Add(section.Type, section);
                }
            }
        }

        public class Section
        {
            public enum SectionTypeEnum
            {
                INPUT,
                OUTPUT,
                INOUT,
                STATIC,
                TEMP,
                CONSTANT,
                RETURN
            }

            public SectionTypeEnum Type { get; internal set; }

            public readonly List<Member> MemberList;

            public Section()
            {
                MemberList = new List<Member>();
            }

            public ICollection<Member> GetMembers()
            {
                return MemberList;
            }

            internal Section ParseXmlNode(XmlNode node)
            {
                var name = node.Attributes["Name"]?.Value;

                bool conversionOK = true;
                conversionOK &= Enum.TryParse(name, true, out SectionTypeEnum sectionType);
                if (!conversionOK)
                {
                    return null;
                }

                Type = sectionType;
                foreach (XmlNode memberNode in node.ChildNodes)
                {
                    MemberList.Add(new Member().ParseXmlNode(memberNode));
                }

                return this;
            }

            public class Member
            {
                public string Name { get; private set; }

                public string Datatype { get; private set; }

                public string StartValue { get; private set; }

                public readonly List<Member> SubMemberList;
                internal Member()
                {
                    SubMemberList = new List<Member>();
                }

                internal Member ParseXmlNode(XmlNode node)
                {
                    Name = node.Attributes["Name"].Value;
                    Datatype = node.Attributes["Datatype"].Value;

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "StartValue")
                        {
                            StartValue = childNode.InnerText;
                        }
                        else if (childNode.Name == "Member")
                        {
                            SubMemberList.Add(new Member().ParseXmlNode(childNode));
                        }

                    }

                    return this;
                }
            }
        }
    }
}
