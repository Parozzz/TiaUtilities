using System;
using System.Collections.Generic;
using System.Xml;
using static SpinAddIn.BlockData.BlockInterface.Section;

namespace SpinAddIn.BlockData
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
            var nsmgr = new XmlNamespaceManager(fcData.GetXmlDocument().NameTable);
            nsmgr.AddNamespace("irfcv4", "http://www.siemens.com/automation/Openness/SW/Interface/v4");

            var sectionNodeList = node.SelectNodes("irfcv4:Sections/irfcv4:Section", nsmgr);
            if(sectionNodeList != null && sectionNodeList.Count > 0)
            {
                foreach (XmlNode sectionNode in sectionNodeList)
                {
                    var section = new Section().ParseXmlNode(sectionNode);
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
                var name = node.Attributes["Name"].Value;
                if (Enum.TryParse(name, true, out SectionTypeEnum sectionType))
                {
                    Type = sectionType;
                }

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
                        SubMemberList.Add(new Member().ParseXmlNode(childNode));
                    }

                    return this;
                }
            }
        }
    }
}
