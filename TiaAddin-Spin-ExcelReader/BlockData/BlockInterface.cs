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
        private readonly Dictionary<SectionTypeEnum, Section> sectionDictionary;
        private readonly List<Section> sectionList;

        internal BlockInterface()
        {
            this.sectionDictionary = new Dictionary<SectionTypeEnum, Section>();
            this.sectionList = new List<Section>();
        }

        public Section GetByType(SectionTypeEnum type)
        {
            return sectionDictionary[type];
        }

        internal void DoXmlNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("Interface"), "BlockInterface node name is not valid.");

            foreach (XmlNode sectionNode in XmlSearchEngine.Of(node).GetAllNodes("Sections/Section"))
            {
                var section = new Section(sectionNode);
                sectionList.Add(section);
                sectionDictionary.Add(section.Type, section);
            }
        }

        internal XmlNode GenerateXmlNode(XmlDocument document)
        {

            return null;
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

            public static string GetSectionTypeString(SectionTypeEnum typeEnum)
            {
                switch (typeEnum)
                {
                    case SectionTypeEnum.INPUT:
                        return "Input";
                    case SectionTypeEnum.OUTPUT:
                        return "Output";
                    case SectionTypeEnum.INOUT:
                        return "InOut";
                    case SectionTypeEnum.STATIC:
                        return "Static";
                    case SectionTypeEnum.TEMP:
                        return "Temp";
                    case SectionTypeEnum.CONSTANT:
                        return "Constant";
                    case SectionTypeEnum.RETURN:
                        return "Return";
                }
            }

            private SectionTypeEnum type;
            public SectionTypeEnum Type { get => type; }


            public readonly List<Member> memberList;
            public List<Member> MemberList { get => memberList; }

            public Section(XmlNode node)
            {
                memberList = new List<Member>();
                this.DoXmlNode(node);
            }

            public Section(SectionTypeEnum type) => this.type = type;

            public Member AddMember(string name, string dataType, string startValue) => new Member(name, dataType, startValue);

            private void DoXmlNode(XmlNode node)
            {
                Validate.NotNull(node);
                Validate.IsTrue(node.Name.Equals("Section"), "Section node name is not valid.");

                bool parseOK = Enum.TryParse(node.Attributes["Name"]?.Value, true, out type);
                if (!parseOK)
                {
                    throw new InvalidOperationException("Some of the values inside a Interface/Sections/Section has not been parsed correctly");
                }

                foreach (XmlNode memberNode in XmlSearchEngine.Of(node).GetAllNodes("Member"))
                {
                    var member = new Member();
                    member.DoXmlNode(memberNode);
                    memberList.Add(member);
                }
            }

            public XmlNode GenerateXmlNode(XmlDocument document)
            {
                var mainNode = document.CreateNode(XmlNodeType.Element, "Section", "");
                mainNode.Attributes.Append(document.CreateAttribute("Name", GetSectionTypeString(type)));
            }

            public class Member
            {
                private string name;
                public String Name { get => name; }

                private string dataType;
                public string Datatype { get => dataType; }

                private string startValue;
                public string StartValue { get => startValue; }

                private readonly List<MemberAttribute> attributeList;
                private readonly Dictionary<CultureInfo, string> commentDictionary;
                private readonly List<Member> subMemberList;

                public Member()
                {
                    attributeList = new List<MemberAttribute>();
                    commentDictionary = new Dictionary<CultureInfo, string>();
                    subMemberList = new List<Member>();
                }
                public Member(string name, string dataType, string startValue) : this()
                {
                    this.name = name;
                    this.dataType = dataType;
                    this.startValue = startValue;
                }

                internal void DoXmlNode(XmlNode node)
                {
                    Validate.NotNull(node);
                    Validate.IsTrue(node.Name.Equals("Member"), "Section/Member node name is not valid.");

                    var parseOK = Util.TryNotNull(node.Attributes["Name"].Value, out name);
                    parseOK &= Util.TryNotNull(node.Attributes["Datatype"].Value, out dataType);
                    if (!parseOK)
                    {
                        throw new InvalidOperationException("Some of the values inside a Section/Member has not been parsed correctly");
                    }

                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "StartValue")
                        {
                            startValue = childNode.InnerText;
                        }
                        else if (childNode.Name == "Member")
                        {
                            var childMember = new Member();
                            childMember.DoXmlNode(childNode);
                            subMemberList.Add(childMember);
                        }
                        else if (childNode.Name == "AttributeList")
                        {
                            foreach (XmlNode attributeNode in childNode)
                            {
                                var memberAttribute = new MemberAttribute();
                                memberAttribute.DoXmlNode(attributeNode);
                                attributeList.Add(memberAttribute);
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
                                    commentDictionary.Add(cultureInfo, languangeTextNode.InnerText);
                                }
                            }
                        }
                    }
                }

                public class MemberAttribute
                {
                    private string name;
                    public string Name { get => name; }

                    private bool systemDefined;
                    public bool SystemDefined { get => systemDefined; }

                    private string value;
                    public string Value { get => value; }

                    public MemberAttribute(string name, bool systemDefined, string value)
                    {
                        this.name = name;
                        this.systemDefined = systemDefined;
                        this.value = value;
                    }

                    internal MemberAttribute()
                    {

                    }

                    internal void DoXmlNode(XmlNode node)
                    {
                        Validate.NotNull(node);
                        Validate.IsTrue(node.Name == "StringAttribute", "Section/Member/Attribute node name is not valid.");

                        var parseOK = Util.TryNotNull(node.Attributes["Name"]?.Value, out string name);
                        parseOK = bool.TryParse(node.Attributes["SystemDefined"]?.Value, out bool systemDefined);
                        parseOK = Util.TryNotNull(node.InnerText, out value);
                        if (!parseOK)
                        {
                            throw new InvalidOperationException("Some of the values inside a Section/Member/AttributeList has not been parsed correctly");
                        }
                    }
                }
            }
        }
    }
}
