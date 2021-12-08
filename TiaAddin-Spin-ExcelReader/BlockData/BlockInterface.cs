using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;
using static TiaAddin_Spin_ExcelReader.BlockInterface.Section;

namespace TiaAddin_Spin_ExcelReader
{
    public class BlockInterface
    {
        private readonly FCData fcData;
        private readonly Dictionary<SectionTypeEnum, Section> sectionDictionary;
        private readonly List<Section> sectionList;

        public XmlNode Node { get; private set; }

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

        internal void SetXmlNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("Interface"), "BlockInterface node name is not valid.");

            Node = node;
            foreach (XmlNode sectionNode in XmlSearchEngine.Of(node).AddSearch("Sections/Section").GetAllNodes())
            {
                var section = Section.Parse(sectionNode);
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

            public static Section Parse(XmlNode node)
            {
                bool parseOK = Enum.TryParse(node.Attributes["Name"]?.Value, true, out SectionTypeEnum sectionType);
                if (!parseOK)
                {
                    return null;
                }

                var section = new Section()
                {
                    Type = sectionType
                };

                foreach (XmlNode memberNode in XmlSearchEngine.Of(node).AddSearch("Member").GetAllNodes())
                {
                    var member = Member.Parse(memberNode);
                    if(member != null)
                    {
                        section.MemberList.Add(member);
                    }
                }

                return section;
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

            public class Member
            {

                public static Member Parse(XmlNode node)
                {
                    var parseOK = Util.TryNotNull(node.Attributes["Name"].Value, out string name);
                    parseOK &= Util.TryNotNull(node.Attributes["Datatype"].Value, out string datatype);
                    if (!parseOK)
                    {
                        return null;
                    }

                    var member = new Member()
                    {
                        Name = name,
                        Datatype = datatype
                    };

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "StartValue")
                        {
                            member.StartValue = childNode.InnerText;
                        }
                        else if (childNode.Name == "Member")
                        {
                            var childMember = Member.Parse(childNode);
                            if(childMember != null)
                            {
                                member.subMemberList.Add(childMember);
                            }
                        }
                        else if (childNode.Name == "AttributeList")
                        {
                            foreach (XmlNode attributeNode in childNode)
                            {
                                var memberAttribute = MemberAttribute.Parse(attributeNode);
                                if (memberAttribute != null)
                                {
                                    member.attributeList.Add(memberAttribute);
                                }
                            }
                        }
                        else if (childNode.Name == "Comment")
                        {
                            foreach (XmlNode languangeTextNode in XmlSearchEngine.Of(childNode).AddSearch("MultiLanguageText").GetAllNodes())
                            {
                                var lang = languangeTextNode.Attributes["Lang"]?.Value;
                                if (lang == null)
                                {
                                    continue;
                                }

                                var cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
                                if (cultureInfo != null)
                                {
                                    member.commentDictionary.Add(cultureInfo, languangeTextNode.InnerText);
                                }
                            }
                        }
                    }

                    return member;
                }

                public string Name { get; private set; }

                public string Datatype { get; private set; }

                public string StartValue { get; private set; }

                private readonly List<MemberAttribute> attributeList;
                private readonly Dictionary<CultureInfo, string> commentDictionary;
                private readonly List<Member> subMemberList;

                internal Member()
                {
                    attributeList = new List<MemberAttribute>();
                    commentDictionary = new Dictionary<CultureInfo, string>();
                    subMemberList = new List<Member>();
                }

                public class MemberAttribute
                {
                    public static MemberAttribute Parse(XmlNode node)
                    {
                        if (node.Name != "StringAttribute")
                        {
                            return null;
                        }

                        var parseOK = Util.TryNotNull(node.Attributes["Name"]?.Value, out string name);
                        parseOK = bool.TryParse(node.Attributes["SystemDefined"]?.Value, out bool systemDefined);
                        if (!parseOK)
                        {
                            return null;
                        }

                        return new MemberAttribute()
                        {
                            Name = name,
                            SystemDefined = systemDefined,
                            Value = node.InnerText
                        };
                    }

                    public string Name { get; private set; }
                    public bool SystemDefined { get; private set; }
                    public string Value { get; private set; }
                }
            }
        }
    }
}
