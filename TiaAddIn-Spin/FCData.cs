using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Siemens.Engineering.SW.Blocks;
using static SpinAddIn.FCInterface.Section;

namespace SpinAddIn
{

    public class FCData
    {
        private readonly FC fc;
        private readonly FCInterface fcInterface;

        public MultilingualText Comment { get; private set; }
        public MultilingualText Title { get; private set; }

        private readonly List<CompileUnit> compileUnitList;

        public FCData(FC fc)
        {
            this.fc = fc;
            this.fcInterface = new FCInterface();

            this.compileUnitList = new List<CompileUnit>();
        }

        public FC GetFC()
        {
            return fc;
        }

        public void ParseXmlNode(XmlNode mainNode)
        {
            foreach (XmlNode mainChildNode in mainNode.ChildNodes)
            {
                if (mainChildNode.Name == "AttributeList")
                {
                    var attributeListNode = mainChildNode;
                    foreach (XmlNode attributeNode in attributeListNode.ChildNodes)
                    {
                        if (attributeNode.Name == "Interface")
                        {
                            fcInterface.ParseXmlNode(mainChildNode);
                            break;
                        }
                    }

                }
                else if (mainChildNode.Name == "ObjectList")
                {
                    var objectListNode = mainChildNode;
                    foreach (XmlNode objectNode in objectListNode.ChildNodes)
                    {
                        if (objectNode.Name == "SW.Blocks.CompileUnit")
                        {
                            compileUnitList.Add(new CompileUnit().ParseXmlNode(objectNode));
                        }
                        else if (objectNode.Name == "MultilingualText")
                        {
                            var multilingualText = new MultilingualText().ParseXMLNode(objectNode);
                            switch (objectNode.Attributes["CompositionName"].Value)
                            {
                                case "Comment":
                                    Comment = multilingualText;
                                    break;
                                case "Title":
                                    Title = multilingualText;
                                    break;
                            }
                        }
                    }
                }
            }
        }

    }

    public class CompileUnit //A.K.A Segment!
    {
        public MultilingualText Title { get; private set; }
        public MultilingualText Comment { get; private set; }
        public ProgrammingLanguage ProgrammingLanguage { get; private set; }
        public FlagNet Net { get; private set; }

        internal CompileUnit()
        {

        }

        internal CompileUnit ParseXmlNode(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "AttributeList":
                        var attributeListNode = childNode;
                        foreach (XmlNode attributeNode in attributeListNode.ChildNodes)
                        {
                            switch (attributeNode.Name)
                            {
                                case "NetworkSource":
                                    var networkSourceNode = attributeNode;
                                    foreach (XmlNode networkSourceChildNode in networkSourceNode.ChildNodes)
                                    {
                                        switch (networkSourceChildNode.Name)
                                        {
                                            case "StructuredText":
                                                break;
                                            case "FlgNet":
                                                Net = new FlagNet();
                                                break;
                                        }
                                    }

                                    break;
                                case "ProgrammingLanguage":
                                    if (Enum.TryParse(attributeNode.Value, true, out ProgrammingLanguage language))
                                    {
                                        ProgrammingLanguage = language;
                                    }
                                    break;
                            }
                        }


                        break;
                    case "ObjectList":
                        var objectListNode = childNode;
                        foreach (XmlNode objectNode in objectListNode.ChildNodes)
                        {
                            if (objectNode.Name == "MultilingualText")
                            {
                                var multilingualText = new MultilingualText().ParseXMLNode(objectNode);
                                switch (multilingualText.CompositionName)
                                {
                                    case "Comment":
                                        Comment = multilingualText;
                                        break;
                                    case "Title":
                                        Title = multilingualText;
                                        break;
                                }
                            }
                        }

                        break;
                }
            }

            return this;
        }
    }

    public class FCInterface
    {
        private readonly Dictionary<SectionTypeEnum, Section> sectionDictionary;
        private readonly List<Section> sectionList;

        internal FCInterface()
        {
            sectionList = new List<Section>();
        }
        public Section GetByType(SectionTypeEnum type)
        {
            return sectionDictionary[type];
        }

        internal void ParseXmlNode(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == "Sections")
                {
                    foreach (XmlNode sectionNode in childNode.ChildNodes)
                    {
                        var section = new Section().ParseXmlNode(sectionNode);
                        sectionList.Add(section);
                        sectionDictionary.Add(section.Type, section);
                    }

                    break;
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
                return ImmutableList.ToImmutableList(MemberList);
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

    public class FlagNet
    {
        internal FlagNet()
        {

        }

        public class Access //A.K.A Variables
        {
            public enum ScopeEnum
            {
                LOCALVARIABLE,
                TYPEDCONSTANT,
            }

            public uint UId { get; private set; }
            public ScopeEnum Scope { get; private set; }

            internal Access ParseXMLNode(XmlNode node)
            {

            }
        }
    }


}
