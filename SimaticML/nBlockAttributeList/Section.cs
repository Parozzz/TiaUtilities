using SimaticML.API;
using SimaticML.Blocks;
using SimaticML.Blocks.FlagNet;
using SimaticML.Enums;
using SimaticML.Enums.Utility;
using SimaticML.XMLClasses;
using System.Collections.ObjectModel;

namespace SimaticML.nBlockAttributeList
{
    public enum SectionTypeEnum
    {
        [SimaticEnum("Input")] INPUT,
        [SimaticEnum("Output")] OUTPUT,
        [SimaticEnum("InOut")] INOUT,
        [SimaticEnum("Static")] STATIC,
        [SimaticEnum("Temp")] TEMP,
        [SimaticEnum("Constant")] CONSTANT,
        [SimaticEnum("Return")] RETURN,
        [SimaticEnum("Base")] BASE, //???. Maybe inside TechnologyObjects???
        [SimaticEnum("None")] NONE //Used inside member to define start value in case of an array of UDT, or inside an UDT directly.1
    }

    public class Section : XmlNodeListConfiguration<Member>, ISimaticVariableCollection
    {
        public const string NODE_NAME = "Section";

        public SectionTypeEnum SectionType { get => SimaticEnumUtils.FindByString<SectionTypeEnum>(this.sectionName.AsString); }
        public ObservableCollection<Member> Members { get => this.GetItems(); }

        private readonly XmlAttributeConfiguration sectionName;

        public Section() : base(Section.NODE_NAME, Member.CreateMember, namespaceURI: SimaticMLAPI.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            this.sectionName = this.AddAttribute("Name", required: true, value: "Namehere");
            //==== INIT CONFIGURATION ====
        }

        public Section(SectionTypeEnum type) : this()
        {
            this.sectionName.AsString = type.GetSimaticMLString();
        }

        //Override base IsEmpty. A section is empty if there is no items inside. Don't care about other stuff.
        public override bool IsEmpty()
        {
            return this.GetItems().Count == 0;
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

        public SimaticVariable AddVariable(string name, SimaticDataType dataType)
        {
            var member = this.AddMember(name, dataType);
            SimaticVariable variable = SectionType switch
            {
                SectionTypeEnum.INPUT or SectionTypeEnum.OUTPUT or SectionTypeEnum.INOUT or SectionTypeEnum.STATIC or SectionTypeEnum.TEMP or SectionTypeEnum.RETURN => new SimaticLocalVariable(this, member),
                SectionTypeEnum.CONSTANT => new SimaticLocalConstant(this, member),
                _ => throw new InvalidOperationException($"Cannot create a Variable because SectionType {SectionType} is not valid!"),
            };

            return variable;
        }

        public Member AddMember(string name, SimaticDataType dataType)
        {
            var member = new Member { MemberName = name, MemberDataType = dataType.SimaticMLString, };
            this.GetItems().Add(member);
            return member;
        }

        public Member? AddMembersFromAddress(string address, SimaticDataType dataType)
        {
            if (this.ParentConfiguration != null)
            {
                string? dbName = null;

                var blockDB = this.ParentConfiguration.FindParent<BlockDB>();
                if (blockDB != null)
                {
                    dbName = blockDB.AttributeList.BlockName;
                }

                if (dbName != null)
                {
                    var wrappedDBName = SimaticMLUtil.WrapAddressComponentIfRequired(dbName);
                    if (address.StartsWith(wrappedDBName + '.'))
                    {
                        address = address.Replace(wrappedDBName + '.', "");
                    }
                }
            }

            var components = SimaticMLUtil.SplitFullAddressIntoComponents(address);
            if (components.Count == 0)
            {
                return null;
            }

            Member? lastMember = null;
            IEnumerable<Member> lastMembersList = this.GetItems();
            for (int x = 0; x < components.Count; x++)
            {
                Member? existingMember = null;

                var addressComponent = components[x];

                var str = string.IsNullOrEmpty(addressComponent.Name) ? SimaticMLAPI.DEFAULT_EMPTY_MEMBER_NAME : addressComponent.Name;
                existingMember = lastMembersList.Where(m => m.MemberName.ToLower() == str.ToLower()).FirstOrDefault();

                var isLastComponent = (x == components.Count - 1);
                if (existingMember == null)
                {
                    if (x == 0) //First item!
                    {
                        existingMember = this.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                    else
                    {
                        if (lastMember == null)
                        {
                            throw new ArgumentException("Somethign went wrong while adding members from address");
                        }

                        existingMember = lastMember.AddMember(str, isLastComponent ? dataType : SimaticDataType.STRUCTURE);
                    }
                }

                lastMember = existingMember;
                lastMembersList = existingMember.GetItems();
            }

            return lastMember;
        }

        public SimaticDataType? FetchDataTypeOf(string variableName)
        {
            foreach (var member in this.GetItems())
            {
                if (member.MemberName.Equals(variableName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return SimaticDataType.FromSimaticMLString(member.MemberDataType);
                }
            }

            return null;
        }
    }
}
