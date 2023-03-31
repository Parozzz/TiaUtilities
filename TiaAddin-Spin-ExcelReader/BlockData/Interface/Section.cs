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
        public SectionTypeEnum Type { get => type; }


        private readonly List<Member> memberList;
        public List<Member> MemberList { get => memberList; }

        private Section()
        {
            memberList = new List<Member>();
        }

        public Section(XmlNode node) : this()
        {
            this.DoXmlNode(node);
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

        internal void DoXmlNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("Section"), "Section node name is not valid.");

            this.memberList.Clear();

            bool parseOK = Enum.TryParse(node.Attributes["Name"]?.Value, true, out type);
            if (!parseOK)
            {
                throw new InvalidOperationException("Some of the values inside a Interface/Sections/Section has not been parsed correctly");
            }

            foreach (XmlNode memberNode in XmlSearchEngine.Of(node).GetAllNodes("Member"))
            {
                memberList.Add(new Member(memberNode));
            }
        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            return XmlNodeBuilder.CreateNew(document, "Section")
                .AppendAttribute("Name", GetSectionNameFromType(type))
                .AppendSerializableCollectionAsChild(memberList)
                .GetNode();
        }

        public class Member : IXMLNodeSerializable
        {
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
                this.DoXmlNode(node);
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

                stringAttributeList.Clear();
                commentDictionary.Clear();
                subMemberList.Clear();

                var parseOK = Utils.TryNotNull(node.Attributes["Name"].Value, out name);
                parseOK &= Utils.TryNotNull(node.Attributes["Datatype"].Value, out dataType);
                if (!parseOK)
                {
                    throw new InvalidOperationException("Some of the values inside a Section/Member has not been parsed correctly");
                }

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "StartValue":
                            startValue = childNode.InnerText;
                            break;
                        case "Member":
                            subMemberList.Add(new Member(childNode));
                            break;
                        case "AttributeList":
                            foreach (XmlNode attributeNode in childNode)
                            {
                                if (attributeNode.Name == "StringAttribute")
                                {
                                    stringAttributeList.Add(new StringMemberAttribute(attributeNode));
                                }
                            }
                            break;
                        case "Comment":
                            foreach (XmlNode languangeTextNode in XmlSearchEngine.Of(childNode).GetAllNodes("MultiLanguageText"))
                            {
                                var lang = languangeTextNode.Attributes["Lang"]?.Value;
                                if (lang == null)
                                {
                                    continue;
                                }

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

            public XmlNode GenerateXmlNode(XmlDocument document)
            {
                var builder = XmlNodeBuilder.CreateNew(document, "Member")
                    .AppendAttribute("Name", this.name).AppendAttribute("Datatype", this.dataType);

                if (this.startValue != null)
                {
                    builder.AppendChild("StartValue", this.startValue);
                }

                if (stringAttributeList.Count > 0)
                {
                    XmlNodeBuilder.CreateNew(document, "AttributeList")
                        .AppendSerializableCollectionAsChild(stringAttributeList)
                        .ChildToBuilder(builder);
                }

                if (commentDictionary.Count > 0)
                {
                    var commentNode = document.CreateNode(XmlNodeType.Element, "Comment", "");
                    foreach (KeyValuePair<CultureInfo, string> commentPair in commentDictionary)
                    {
                        XmlNodeBuilder.CreateNew(document, "MultiLanguageText")
                            .InnerText(commentPair.Value)
                            .AppendAttribute("Lang", commentPair.Key.IetfLanguageTag)
                            .ChildTo(commentNode);
                    }
                    builder.AppendChild(commentNode);
                }

                return builder.GetNode();
            }

            public class StringMemberAttribute : IXMLNodeSerializable
            {
                private string name;
                public string Name { get => name; }

                private bool systemDefined;
                public bool SystemDefined { get => systemDefined; }

                private string value;
                public string Value { get => value; }

                public StringMemberAttribute(XmlNode node)
                {
                    this.ParseXmlNode(node);
                }

                public StringMemberAttribute(string name, bool systemDefined, string value)
                {
                    this.name = name;
                    this.systemDefined = systemDefined;
                    this.value = value;
                }

                private void ParseXmlNode(XmlNode node)
                {
                    Validate.NotNull(node);
                    Validate.IsTrue(node.Name == "StringAttribute", "Section/Member/Attribute node name is not valid.");

                    var parseOK = Utils.TryNotNull(node.Attributes["Name"]?.Value, out string name);
                    parseOK &= bool.TryParse(node.Attributes["SystemDefined"]?.Value, out bool systemDefined);
                    parseOK &= Utils.TryNotNull(node.InnerText, out value);
                    if (!parseOK)
                    {
                        throw new InvalidOperationException("Some of the values inside a Section/Member/AttributeList has not been parsed correctly");
                    }
                }
                public XmlNode GenerateXmlNode(XmlDocument document)
                {
                    return XmlNodeBuilder.CreateNew(document, "StringAttribute")
                        .InnerText(value)
                        .AppendAttribute("Name", name)
                        .AppendAttribute("SystemDefined", systemDefined)
                        .GetNode();
                }

            }
        }
    }
}
