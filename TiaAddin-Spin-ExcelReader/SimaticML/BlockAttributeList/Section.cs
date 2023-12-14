using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TiaXmlReader.Attributes;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;
using static SpinXmlReader.Block.Section;

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
        RETURN,
        NONE //Used inside member to define start value in case of an array of UDT.
    }

    public static class SectionTypeEnumExtension
    {
        public static string GetSimaticMLString(this SectionTypeEnum sectionType)
        {
            switch (sectionType)
            {
                case SectionTypeEnum.INPUT: return "Input";
                case SectionTypeEnum.OUTPUT: return "Output";
                case SectionTypeEnum.INOUT: return "InOut";
                case SectionTypeEnum.STATIC: return "Static";
                case SectionTypeEnum.TEMP: return "Temp";
                case SectionTypeEnum.CONSTANT: return "Constant";
                case SectionTypeEnum.RETURN: return "Return";
                case SectionTypeEnum.NONE: return "None";
                default:
                    return null;
            }
        }
    }

    public class Section :  XmlNodeListConfiguration<Member>
    {
        public const string NODE_NAME = "Section";
        private static Member CreateMember(XmlNode node)
        {
            return node.Name == Member.NODE_NAME ? new Member() : null;
        }

        private readonly XmlAttributeConfiguration sectionName;

        public Section() : base(Section.NODE_NAME, Section.CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            sectionName = this.AddAttribute("Name", required: true, value: "Namehere");
            //==== INIT CONFIGURATION ====
        }

        public Section(SectionTypeEnum type) : this()
        {
            sectionName.SetValue(type.GetSimaticMLString());
        }

        //Override base IsEmpty. A section is empty if there is no items inside. Don't care about other stuff.
        public override bool IsEmpty()
        {
            return this.GetItems().Count == 0;
        }

        public SectionTypeEnum GetSectionType()
        {
            return (SectionTypeEnum) Enum.Parse(typeof(SectionTypeEnum), sectionName.GetValue(), ignoreCase: true);
        }

        public void GetSectionType(out SectionTypeEnum type)
        {
            Enum.TryParse<SectionTypeEnum>(sectionName.GetValue(), ignoreCase: true, out type);
        }

        public Section SetReturnRetValMember(string name, SimaticDataType dataType)
        {
            this.GetItems().Clear();
            this.AddMember(name, dataType);
            return this;
        }

        public Section SetVoidReturnRetValMember()
        {
            this.SetReturnRetValMember("Ret_Val", SimaticDataType.VOID);
            return this;
        }

        public Member AddMember(string name, SimaticDataType dataType)
        {
            var member = new Member()
                .SetMemberName(string.IsNullOrEmpty(name) ? Constants.DEFAULT_EMPTY_STRUCT_NAME : name)
                .SetMemberDataType(dataType.GetSimaticMLString());
            this.GetItems().Add(member);
            return member;
        }

        public Member AddMembersFromAddress(string address, SimaticDataType dataType)
        {
            var components = SimaticMLUtil.SplitAddressIntoComponents(address);
            if(components.Count == 0)
            {
                return null;
            }

            Member lastMember = null;
            IEnumerable<Member> lastMembersList = this.GetItems();
            for(int x = 0; x < components.Count; x++)
            {
                Member foundMember = null;

                var str = components[x];
                str = string.IsNullOrEmpty(str) ? Constants.DEFAULT_EMPTY_STRUCT_NAME : str;
                foreach (var member in lastMembersList)
                {
                    if (member.GetMemberName().ToLower() == str.ToLower())
                    {
                        foundMember = member;
                        break;
                    }
                }

                var isLastComponent = (x == components.Count - 1);
                if (foundMember == null)
                {
                    if(x == 0)
                    {
                        foundMember = this.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                    else
                    {
                        Validate.IsTrue(lastMember != null, "Somethign went wrong while adding members from address");
                        foundMember = lastMember.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                }

                lastMember = foundMember;
                lastMembersList = foundMember.GetItems();
            }

            return lastMember;
        }

        //Members can have other members inside (In case of structs)
        public class Member : XmlNodeListConfiguration<Member>
        {
            public const string NODE_NAME = "Member";

            private readonly XmlAttributeConfiguration memberName;
            private readonly XmlAttributeConfiguration dataType;
            private readonly XmlAttributeConfiguration version;
            private readonly XmlAttributeConfiguration remanence;

            private readonly XmlNodeConfiguration subElement;           //Not implemented yet
            private readonly XmlAttributeConfiguration subElementPath;  //Not implemented yet
            private readonly XmlNodeConfiguration subElementStartValue; //Not implemented yet

            private readonly Section section;                           //Not implemented yet. Used to define start value in case of an array of UDT.

            private readonly XmlNodeConfiguration startValue;
            
            private readonly XmlNodeListConfiguration<XmlNodeConfiguration> attributeList;
            private readonly MultiLanguageTextCollection comment;

            public Member() : base(Member.NODE_NAME, CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
            {
                //==== INIT CONFIGURATION ====
                memberName = this.AddAttribute("Name",   required: true, value: "DefaultName");
                dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
                version = this.AddAttribute("Version");
                remanence = this.AddAttribute("Remanence");

                subElement = this.AddNode("Subelement");
                subElementPath = subElement.AddAttribute("Path", required: true);
                subElementStartValue = subElement.AddNode("StartValue", required: true);

                section = this.AddNode(new Section(SectionTypeEnum.NONE));

                startValue = this.AddNode("StartValue");

                attributeList = this.AddNode(new XmlNodeListConfiguration<XmlNodeConfiguration>(Constants.ATTRIBUTE_LIST_KEY, AttributeUtil.CreateAttribute));

                comment = this.AddNode(new MultiLanguageTextCollection("Comment"));
                //==== INIT CONFIGURATION ====

                GetItems().CollectionChanged += Member_CollectionChanged;
            }

            private void Member_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    foreach (Member newItem in e.NewItems)
                    {
                        newItem.parentConfiguration = this;
                    }
                }
            }

            public string GetMemberName()
            {
                return memberName.GetValue();
            }

            public Member SetMemberName(string name)
            {
                this.memberName.SetValue(name);
                return this;
            }

            public string GetMemberDataType()
            {
                return dataType.GetValue();
            }

            public Member SetMemberDataType(string dataType)
            {
                this.dataType.SetValue(dataType);
                return this;
            }

            public string GetStartValue()
            {
                return startValue.GetInnerText();
            }

            public Member SetMemberStartValue(string startValue)
            {
                this.startValue.SetInnerText(startValue);
                return this;
            }

            public string GetVersion()
            {
                return this.version.GetValue();
            }

            public Member SetVersion(string version)
            {
                this.version.SetValue(version);
                return this;
            }

            public string GetRemanence()
            {
                return this.remanence.GetValue();
            }

            public Member SetRemanenceRetain()
            {
                this.remanence.SetValue("Retain");
                return this;
            }

            public Member SeRemancenceSetInIDB()
            {
                this.remanence.SetValue("SetInIDB");
                return this;
            }

            public Member AddMember(string name, SimaticDataType dataType)
            {
                var member = this.AddNode(new Member());
                member.SetMemberName(name);
                member.SetMemberDataType(dataType.GetSimaticMLString());
                return member;
            }

            public string GetCompleteSymbol()
            {
                return this.GetParentSymbol(this);
            }

            private string GetParentSymbol(XmlNodeConfiguration parentConfiguration)
            {
                if(parentConfiguration is Member parentMember)
                {
                    //Wrap the member name in double quotes to "join" all the values toghether. If the name contains special chars (Like a dot) it will create problems.
                    return this.GetParentSymbol(parentMember.parentConfiguration) + "." + SimaticMLUtil.WrapAddressComponentIfRequired(parentMember.GetMemberName());
                }
                else if(parentConfiguration != null)
                {
                    var loopParentConfiguration = parentConfiguration.GetParentConfiguration();
                    while(true)
                    {
                        if(loopParentConfiguration == null)
                        {
                            return "";
                        }
                        else if(loopParentConfiguration is BlockGlobalDB globalDB)
                        {
                            return SimaticMLUtil.WrapAddressComponentIfRequired(globalDB.GetAttributes().GetBlockName());
                        }

                        loopParentConfiguration = loopParentConfiguration.GetParentConfiguration();
                    }
                }

                return "";
            }

            //These are be all IAttribute<V>
            public ICollection<XmlNodeConfiguration> GetAttributeList()
            {
                return attributeList.GetItems();
            }

            public Member SetComment(CultureInfo culture, string text)
            {
                foreach (var item in comment.GetItems())
                {
                    if(item.GetLang() == culture)
                    {
                        item.SetLangText(culture, text);
                        return this;
                    }
                }

                var multiLanguageText = new MultiLanguageText();
                multiLanguageText.SetLangText(culture, text);
                comment.GetItems().Add(multiLanguageText);

                return this;
            }

            public Dictionary<CultureInfo, string> GetComments()
            {
                var dict = new Dictionary<CultureInfo, string>();
                foreach(var item in comment.GetItems())
                {
                    dict.Add(item.GetLang(), item.GetInnerText());
                }
                return dict;
            }
        }
    }
}
