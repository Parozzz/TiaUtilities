using SimaticML.API;
using SimaticML.Attributes;
using SimaticML.Blocks;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;
using System.Globalization;
using System.Xml;

namespace SimaticML.nBlockAttributeList
{
    //Members can have other members inside (In case of structs)
    public class Member : XmlNodeConfiguration, ISimaticVariableDataHolder
    {
        public const string NODE_NAME = "Member";
        public static Member? CreateMember(XmlNode node)
        {
            return node.Name == Member.NODE_NAME ? new Member() : null;
        }

        public string MemberName { get => this.memberName.Value.AsString; set => this.memberName.Value.AsString = (string.IsNullOrEmpty(value) ? SimaticMLAPI.DEFAULT_EMPTY_MEMBER_NAME : value); }
        public string MemberDataType { get => this.dataType.Value.AsString; set => this.dataType.Value.AsString = value; }
        public SimaticDataType SimaticDataType { get => SimaticDataType.FromSimaticMLString(this.MemberDataType); }

        public IEnumerable<Member> Members { get => this.children.Where(c => c.ConfigurationName == Member.NODE_NAME).Cast<Member>(); }

        public Section? SubSection { get => this.sections.GetItems().FirstOrDefault(s => s.SectionType == SectionTypeEnum.NONE); }
        public IEnumerable<SubElement> SubElements { get => this.children.Where(c => c.ConfigurationName == SubElement.NODE_NAME).Cast<SubElement>(); }

        public string StartValue { get => this.startValue.Value.AsString; set => this.startValue.Value.AsString = value; }
        public string Version { get => this.version.Value.AsString; set => this.version.Value.AsString = value; }
        public uint Offset { get => this.GetAttribute<uint>("Offset")?.Value.AsUInt ?? 0; } //If not found, returns 0

        public Comment Comment { get => this.comment; }

        private readonly XmlAttributeConfiguration memberName;
        private readonly XmlAttributeConfiguration dataType;
        private readonly XmlAttributeConfiguration version;
        private readonly XmlAttributeConfiguration remanence;
        private readonly XmlAttributeConfiguration accessibility;
        private readonly XmlAttributeConfiguration informative;

        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> attributeList;

        private readonly XmlNodeListConfiguration<Section> sections;//Not implemented yet. Used to define start value in case of an array of UDT.                     

        private readonly XmlNodeConfiguration startValue;

        private readonly Comment comment;

        public Member() : base(Member.NODE_NAME, namespaceURI: SimaticMLAPI.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            memberName = this.AddAttribute("Name", required: true, value: "DefaultName");
            dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
            version = this.AddAttribute("Version");
            remanence = this.AddAttribute("Remanence");
            accessibility = this.AddAttribute("Accessibility");
            informative = this.AddAttribute("Informative");

            attributeList = this.AddNode(new XmlNodeListConfiguration<XmlNodeConfiguration>(SimaticMLAPI.ATTRIBUTE_LIST_KEY, AttributeUtil.CreateAttribute));

            sections = this.AddNodeList("Sections", xmlNode => new Section());

            startValue = this.AddNode("StartValue");

            comment = this.AddNode(new Comment());
            //==== INIT CONFIGURATION ====
        }

        public string GetRemanence()
        {
            return this.remanence.Value.AsString;
        }

        public Member SetRemanenceRetain()
        {
            this.remanence.Value.AsString = "Retain";
            return this;
        }

        public Member SeRemancenceSetInIDB()
        {
            this.remanence.Value.AsString = "SetInIDB";
            return this;
        }

        public Member AddMember(string name, SimaticDataType dataType)
        {
            var member = new Member()
            {
                MemberName = name,
                MemberDataType = dataType.SimaticMLString,
            };
            member.SetParentConfiguration(this);
            base.children.Add(member);
            return member;
        }

        public string GetCompleteSymbol()
        {
            return this.GetParentSymbol(this);
        }

        public List<string> GetAllMemberAddress(bool includeItself = true)
        {
            var membersAddressList = new List<string>();
            
            foreach (var member in this.Members)
            {
                var memberChildAddressList = BlockDB.GetAddressOfChildMembers(member);

                var memberName = includeItself ? (SimaticMLUtil.WrapAddressComponentIfRequired(member.MemberName) + ".") : "";
                membersAddressList.AddRange(memberChildAddressList.Select(s => memberName + s));
            }

            return membersAddressList;
        }

        private string GetParentSymbol(XmlNodeConfiguration parentConfiguration)
        {
            if (parentConfiguration is Member parentMember)
            {
                if(parentMember.ParentConfiguration == null)
                {
                    return SimaticMLUtil.WrapAddressComponentIfRequired(parentMember.MemberName);
                }
                else
                {
                    //Wrap the member name in double quotes to "join" all the values toghether. If the name contains special chars (Like a dot) it will create problems.
                    return this.GetParentSymbol(parentMember.ParentConfiguration) + "." + SimaticMLUtil.WrapAddressComponentIfRequired(parentMember.MemberName);
                }
            }
            else if (parentConfiguration != null)
            {
                var loopParentConfiguration = parentConfiguration.GetParentConfiguration();
                while (true)
                {
                    if (loopParentConfiguration == null)
                    {
                        return "";
                    }
                    else if (loopParentConfiguration is BlockGlobalDB globalDB)
                    {
                        return SimaticMLUtil.WrapAddressComponentIfRequired(globalDB.AttributeList.BlockName);
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

        //Attribute for the offset in the DB espressed in bits.
        //<IntegerAttribute Name="Offset" Informative="true" SystemDefined="true">240</IntegerAttribute>
        public IAttribute<V>? GetAttribute<V>(string name)
        {
            foreach (var node in attributeList.GetItems())
            {
                if(node is IAttribute<V> attribute && attribute.AttributeName == name)
                {
                    return attribute;
                }
            }

            return null;
        }

        public Dictionary<CultureInfo, string> GetComments()
        {
            var dict = new Dictionary<CultureInfo, string>();
            foreach (var item in comment.GetItems())
            {
                dict.Add(item.Lang, item.Value.AsString);
            }
            return dict;
        }

        public override string ToString() => $@"Member: {memberName.Value.AsString}, Type: {dataType.Value.AsString}";

        public string GetName()
        {
            return this.MemberName;
        }

        public void AddComment(CultureInfo cultureInfo, string commentText)
        {
            this.comment[cultureInfo] = commentText;
        }
    }
}
