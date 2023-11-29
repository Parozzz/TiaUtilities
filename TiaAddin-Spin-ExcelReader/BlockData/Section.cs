﻿using System;
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

        private readonly XmlAttributeConfiguration name;

        public Section() : base(Section.NODE_NAME, Section.CreateMember, required: true, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            name = this.AddAttribute("Name", required: true);
            //==== INIT CONFIGURATION ====
        }

        public Section(SectionTypeEnum type) : this()
        {
            name.SetValue(GetSectionNameFromType(type));
        }

        public SectionTypeEnum GetSectionType()
        {
            return (SectionTypeEnum) Enum.Parse(typeof(SectionTypeEnum), name.GetValue(), ignoreCase: true);
        }

        public void GetType(out SectionTypeEnum type)
        {
            Enum.TryParse<SectionTypeEnum>(name.GetValue(), ignoreCase: true, out type);
        }

        public void AddReturnRetValMember()
        {
            var member = this.AddMember();
            member.SetMemberName("Ret_Val");
            member.SetMemberDataType("Void");
        }

        public Member AddMember()
        {
            var member = new Member();
            this.AddNode(member);
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

            public Member() : base(Member.NODE_NAME, CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
            {
                //==== INIT CONFIGURATION ====
                name = this.AddAttribute("Name",         required: true, value: "DefaultName");
                dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
                startValue = this.AddNode("StartValue");

                attributeList = this.AddNode(new XmlNodeListConfiguration<XmlNodeConfiguration>(Constants.ATTRIBUTE_LIST_KEY, AttributeUtil.CreateAttribute));

                comment = this.AddNode(new MultiLanguageTextCollection("Comment"));
                //==== INIT CONFIGURATION ====
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

            //These are be all IAttribute<V>
            public ICollection<XmlNodeConfiguration> GetAttributeList()
            {
                return attributeList.GetItems();
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