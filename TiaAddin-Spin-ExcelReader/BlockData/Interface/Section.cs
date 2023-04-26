using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinXmlReader.Block
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

    public class Section : IXMLNodeSerializable
    {
        public const string NODE_NAME = "Section";
        public const string NAME = "Name";

        public static string GetSectionNameFromType(SectionTypeEnum typeEnum)
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
                default:
                    return null;
            }
        }

        private SectionTypeEnum type;
        private readonly List<Member> memberList;

        private Section()
        {
            memberList = new List<Member>();
        }

        public Section(SectionTypeEnum type) : this()
        {
            this.type = type;
        }

        //
        // Riepilogo:
        //     Aggiungi un nuovo membro alla lista dei membri di questa sezione
        //
        // Valori restituiti:
        //     Il nuovo membro creato
        public Member AddMember(string name, string dataType, string startValue)
        {
            var newMember = new Member(name, dataType, startValue);
            memberList.Add(newMember);
            return newMember;
        }

        public void ParseNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals(Section.NODE_NAME), "Section node name is not valid.");

            this.memberList.Clear();

            bool parseOK = Enum.TryParse(node.Attributes[Section.NAME]?.Value, true, out type);
            if (!parseOK)
            {
                throw new InvalidOperationException("Some of the values inside a Interface/Sections/Section has not been parsed correctly");
            }

            foreach (XmlNode memberNode in XmlSearchEngine.Of(node).GetAllNodes("Member"))
            {
                memberList.Add(new Member(memberNode));
            }
        }

        public XmlNode GenerateNode(XmlDocument document)
        {
            var xmlNode = document.CreateElement(Section.NODE_NAME);
            xmlNode.SetAttribute(Section.NAME, GetSectionNameFromType(type));

            foreach (var member in memberList)
            {
                var node = member.GenerateNode(document);
                xmlNode.AppendChild(node);
            }

            return xmlNode;
        }

        public class Member : IXMLNodeSerializable
        {
            public const string NODE_NAME = "Member";
            public const string NAME = "Name";
            public const string DATA_TYPE = "Datatype";
            public const string START_VALUE = "StartValue";

            public const string COMMENT = "Comment";
            public const string COMMENT_LANGUAGE = "Lang";
            public const string COMMENT_MULTI_LANGUAGE_TEXT = "MultiLanguageText";

            private string name;
            public String Name { get => name; }

            private string dataType;
            public string Datatype { get => dataType; }

            private string startValue;
            public string StartValue { get => startValue; }

            private readonly List<StringMemberAttribute> stringAttributeList;
            private readonly Dictionary<CultureInfo, string> commentDictionary;
            private readonly List<Member> subMemberList;

            private Member()
            {
                stringAttributeList = new List<StringMemberAttribute>();
                commentDictionary = new Dictionary<CultureInfo, string>();
                subMemberList = new List<Member>();
            }

            public Member(XmlNode node) : this()
            {
                this.ParseNode(node);
            }

            public Member(string name, string dataType, string startValue) : this()
            {
                this.name = name;
                this.dataType = dataType;
                this.startValue = startValue;
            }

            public void ParseNode(XmlNode node)
            {
                Validate.NotNull(node);
                Validate.IsTrue(node.Name.Equals(Member.NODE_NAME), "Section/Member node name is not valid.");

                stringAttributeList.Clear();
                commentDictionary.Clear();
                subMemberList.Clear();

                var parseOK = Utils.TryNotNull(node.Attributes[Member.NAME].Value, out name);
                parseOK &= Utils.TryNotNull(node.Attributes[Member.DATA_TYPE].Value, out dataType);
                if (!parseOK)
                {
                    throw new InvalidOperationException("Some of the values inside a Section/Member has not been parsed correctly");
                }

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case Member.START_VALUE:
                            startValue = childNode.InnerText;
                            break;
                        case Member.NODE_NAME:
                            subMemberList.Add(new Member(childNode));
                            break;
                        case Constants.ATTRIBUTE_LIST_NAME:
                            foreach (XmlNode attributeNode in childNode)
                            {
                                if (attributeNode.Name == StringMemberAttribute.NODE_NAME)
                                {
                                    stringAttributeList.Add(new StringMemberAttribute(attributeNode));
                                }
                            }
                            break;
                        case Member.COMMENT:
                            foreach (XmlNode languangeTextNode in XmlSearchEngine.Of(childNode).GetAllNodes(Member.COMMENT_MULTI_LANGUAGE_TEXT))
                            {
                                var lang = languangeTextNode.Attributes[Member.COMMENT_LANGUAGE].Value;
                                var cultureInfo = CultureInfo.GetCultureInfo(lang);
                                if (cultureInfo != null)
                                {
                                    commentDictionary.Add(cultureInfo, languangeTextNode.InnerText);
                                }
                            }
                            break;
                    }
                }
            }

            public XmlNode GenerateNode(XmlDocument document)
            {
                var xmlNode = document.CreateElement(Member.NODE_NAME);
                xmlNode.SetAttribute(Member.NAME, this.name);
                xmlNode.SetAttribute(Member.DATA_TYPE, this.dataType);

                if (this.startValue != null)
                {
                    xmlNode.AppendChild(document.CreateElement(Member.START_VALUE)).InnerText = this.startValue;
                }

                if (stringAttributeList.Count > 0)
                {
                    var attributeListNode = xmlNode.AppendChild(document.CreateElement(Constants.ATTRIBUTE_LIST_NAME));
                    foreach (var stringAttribute in stringAttributeList)
                    {
                        var node = stringAttribute.GenerateNode(document);
                        attributeListNode.AppendChild(node);
                    }
                }

                //This part has a custom comment part without global ID
                if (commentDictionary.Count > 0)
                {
                    var commentNode = document.CreateElement(Member.COMMENT);
                    foreach (KeyValuePair<CultureInfo, string> commentPair in commentDictionary)
                    {
                        var textNode = document.CreateElement(Member.COMMENT_MULTI_LANGUAGE_TEXT);
                        textNode.SetAttribute(Member.COMMENT_LANGUAGE, commentPair.Key.IetfLanguageTag);
                        textNode.InnerText = commentPair.Value;
                        commentNode.AppendChild(textNode);
                    }
                    xmlNode.AppendChild(commentNode);
                }

                return xmlNode;
            }

            public class StringMemberAttribute : IXMLNodeSerializable
            {
                public const string NODE_NAME = "StringAttribute";
                public const string NAME = "Name";
                public const string SYSTEM_DEFINED = "SystemDefined";

                private string name;
                public string Name { get => name; }

                private bool systemDefined;
                public bool SystemDefined { get => systemDefined; }

                private string value;
                public string Value { get => value; }

                public StringMemberAttribute(XmlNode node)
                {
                    this.ParseNode(node);
                }

                public StringMemberAttribute(string name, bool systemDefined, string value)
                {
                    this.name = name;
                    this.systemDefined = systemDefined;
                    this.value = value;
                }

                public void ParseNode(XmlNode node)
                {
                    Validate.NotNull(node);
                    Validate.IsTrue(node.Name == StringMemberAttribute.NODE_NAME, "Section/Member/Attribute node name is not valid.");

                    var parseOK = Utils.TryNotNull(node.Attributes[StringMemberAttribute.NAME]?.Value, out string name);
                    parseOK &= bool.TryParse(node.Attributes[StringMemberAttribute.SYSTEM_DEFINED]?.Value, out bool systemDefined);
                    parseOK &= Utils.TryNotNull(node.InnerText, out value);
                    if (!parseOK)
                    {
                        throw new InvalidOperationException("Some of the values inside a Section/Member/AttributeList has not been parsed correctly");
                    }
                }
                public XmlNode GenerateNode(XmlDocument document)
                {
                    var xmlNode = document.CreateElement(StringMemberAttribute.NODE_NAME);
                    xmlNode.SetAttribute(StringMemberAttribute.NAME, this.name);
                    xmlNode.SetAttribute(StringMemberAttribute.SYSTEM_DEFINED, this.SystemDefined.ToString());
                    return xmlNode;
                }

            }
        }
    }
}
