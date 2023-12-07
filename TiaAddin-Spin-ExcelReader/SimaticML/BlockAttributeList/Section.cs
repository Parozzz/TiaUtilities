using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TiaXmlReader.Attributes;
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
        RETURN
    }

    public class Section :  XmlNodeListConfiguration<Member>
    {
        public const string NODE_NAME = "Section";
        private static Member CreateMember(XmlNode node)
        {
            return node.Name == Member.NODE_NAME ? new Member() : null;
        }

        public static string GetSectionNameFromType(SectionTypeEnum typeEnum)
        {
            switch (typeEnum)
            {
                case SectionTypeEnum.INPUT: return "Input";
                case SectionTypeEnum.OUTPUT: return "Output";
                case SectionTypeEnum.INOUT: return "InOut";
                case SectionTypeEnum.STATIC: return "Static";
                case SectionTypeEnum.TEMP: return "Temp";
                case SectionTypeEnum.CONSTANT: return "Constant";
                case SectionTypeEnum.RETURN: return "Return";
                default:
                    return null;
            }
        }

        private readonly BlockAttributeList parentAttributeList;

        private readonly XmlAttributeConfiguration name;

        public Section(BlockAttributeList parentAttributeList) : base(Section.NODE_NAME, Section.CreateMember, required: true, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
        {
            this.parentAttributeList = parentAttributeList;

            //==== INIT CONFIGURATION ====
            name = this.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }

        public Section(BlockAttributeList parentAttributeList, SectionTypeEnum type) : this(parentAttributeList)
        {
            name.SetValue(GetSectionNameFromType(type));
        }

        public SectionTypeEnum GetSectionType()
        {
            return (SectionTypeEnum) Enum.Parse(typeof(SectionTypeEnum), name.GetValue(), ignoreCase: true);
        }

        public void GetSectionType(out SectionTypeEnum type)
        {
            Enum.TryParse<SectionTypeEnum>(name.GetValue(), ignoreCase: true, out type);
        }

        public Section SetReturnRetValMember(string name, string dataType)
        {
            this.GetItems().Clear();
            this.AddMember(name, dataType);
            return this;
        }

        public Section SetVoidReturnRetValMember()
        {
            this.SetReturnRetValMember("Ret_Val", "Void");
            return this;
        }

        public Member AddMember(string name, string dataType)
        {
            var member = this.AddNode(new Member(this));
            member.SetMemberName(name);
            member.SetMemberDataType(dataType);
            return member;
        }

        //Members can have other members inside (In case of structs)
        public class Member : XmlNodeListConfiguration<Member>
        {
            public const string NODE_NAME = "Member";

            private readonly XmlAttributeConfiguration name;
            private readonly XmlAttributeConfiguration dataType;
            private readonly XmlNodeConfiguration startValue;
            
            private readonly XmlNodeListConfiguration<XmlNodeConfiguration> attributeList;
            private readonly MultiLanguageTextCollection comment;

            public Member(XmlNodeConfiguration parentConfiguration = null) : base(Member.NODE_NAME, CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
            {
                //==== INIT CONFIGURATION ====
                name = this.AddAttribute("Name",         required: true, value: "DefaultName");
                dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
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
                return name.GetValue();
            }

            public void SetMemberName(string name)
            {
                this.name.SetValue(name);
            }

            public string GetMemberDataType()
            {
                return dataType.GetValue();
            }

            public void SetMemberDataType(string dataType)
            {
                this.dataType.SetValue(dataType);
            }

            public string GetStartValue()
            {
                return startValue.GetInnerText();
            }

            public void SetMemberStartValue(string startValue)
            {
                this.startValue.SetInnerText(startValue);
            }

            public string GetCompleteSymbol()
            {
                return this.GetParentSymbol(base.GetParentConfiguration()) + "." + this.name.GetValue();

            }

            private string GetParentSymbol(XmlNodeConfiguration parentConfiguration)
            {
                if(parentConfiguration is Member parentMember)
                {
                    return parentMember.GetMemberName() + this.GetParentSymbol(parentMember.parentConfiguration);
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
                        else if(loopParentConfiguration is GlobalDB globalDB)
                        {
                            return globalDB.GetAttributes().GetBlockName();
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
