using System;
using System.Collections.Generic;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.XMLClasses;
using System.Linq;

namespace TiaXmlReader.SimaticML.nBlockAttributeList
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
        BASE, //???. Maybe inside TechnologyObjects???
        NONE //Used inside member to define start value in case of an array of UDT, or inside an UDT directly.1
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
                case SectionTypeEnum.BASE: return "Base";
                case SectionTypeEnum.NONE: return "None";
                default:
                    return null;
            }
        }
    }

    public class Section :  XmlNodeListConfiguration<Member>
    {
        public const string NODE_NAME = "Section";

        private readonly XmlAttributeConfiguration sectionName;

        public Section() : base(Section.NODE_NAME, Member.CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
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
            var components = SimaticMLUtil.SplitFullAddressIntoComponents(address);
            if(components.Count == 0)
            {
                return null;
            }

            Member lastMember = null;
            IEnumerable<Member> lastMembersList = this.GetItems();
            for(int x = 0; x < components.Count; x++)
            {
                Member existingMember = null;

                var addressComponent = components[x];

                var str = string.IsNullOrEmpty(addressComponent.Name) ? Constants.DEFAULT_EMPTY_STRUCT_NAME : addressComponent.Name;
                existingMember = lastMembersList.Where(m => m.GetMemberName().ToLower() == str.ToLower()).FirstOrDefault();

                var isLastComponent = (x == components.Count - 1);
                if (existingMember == null)
                {
                    if(x == 0) //First item!
                    {
                        existingMember = this.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                    else
                    {
                        Validate.IsTrue(lastMember != null, "Somethign went wrong while adding members from address");
                        existingMember = lastMember.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                }

                lastMember = existingMember;
                lastMembersList = existingMember.GetItems();
            }

            return lastMember;
        }
    }
}
