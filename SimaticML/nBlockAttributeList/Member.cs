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
    public class Member : XmlNodeListConfiguration<Member>
    {
        public const string NODE_NAME = "Member";
        public static Member? CreateMember(XmlNode node)
        {
            return node.Name == Member.NODE_NAME ? new Member() : null;
        }

        public string MemberName { get => this.memberName.AsString; set => this.memberName.AsString = (string.IsNullOrEmpty(value) ? SimaticMLAPI.DEFAULT_EMPTY_MEMBER_NAME : value); }
        public string MemberDataType { get => this.dataType.AsString; set => this.dataType.AsString = value; }
        public string StartValue { get => this.startValue.AsString; set => this.startValue.AsString = value; }
        public string Version { get => this.version.AsString; set => this.version.AsString = value; }

        public Comment Comment { get => this.comment; }

        private readonly XmlAttributeConfiguration memberName;
        private readonly XmlAttributeConfiguration dataType;
        private readonly XmlAttributeConfiguration version;
        private readonly XmlAttributeConfiguration remanence;
        private readonly XmlAttributeConfiguration accessibility;
        private readonly XmlAttributeConfiguration informative;

        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> attributeList;

        private readonly XmlNodeConfiguration subElement;           //Not implemented yet
        private readonly XmlAttributeConfiguration subElementPath;  //Not implemented yet
        private readonly XmlNodeConfiguration subElementStartValue; //Not implemented yet

        private readonly XmlNodeListConfiguration<Section> sections;//Not implemented yet. Used to define start value in case of an array of UDT.                     

        private readonly XmlNodeConfiguration startValue;

        private readonly Comment comment;

        public Member() : base(Member.NODE_NAME, Member.CreateMember, namespaceURI: SimaticMLAPI.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            memberName = this.AddAttribute("Name", required: true, value: "DefaultName");
            dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
            version = this.AddAttribute("Version");
            remanence = this.AddAttribute("Remanence");
            accessibility = this.AddAttribute("Accessibility");
            informative = this.AddAttribute("Informative");

            attributeList = this.AddNode(new XmlNodeListConfiguration<XmlNodeConfiguration>(SimaticMLAPI.ATTRIBUTE_LIST_KEY, AttributeUtil.CreateAttribute));

            subElement = this.AddNode("Subelement");
            subElementPath = subElement.AddAttribute("Path", required: true);
            subElementStartValue = subElement.AddNode("StartValue", required: true);

            sections = this.AddNodeList("Sections", xmlNode => new Section());

            startValue = this.AddNode("StartValue");

            comment = this.AddNode(new Comment());
            //==== INIT CONFIGURATION ====

            GetItems().CollectionChanged += Member_CollectionChanged;
        }

        private void Member_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Member newItem in e.NewItems)
                {
                    newItem.ParentConfiguration = this;
                }
            }
        }

        public string GetRemanence()
        {
            return this.remanence.AsString;
        }

        public Member SetRemanenceRetain()
        {
            this.remanence.AsString = "Retain";
            return this;
        }

        public Member SeRemancenceSetInIDB()
        {
            this.remanence.AsString = "SetInIDB";
            return this;
        }

        public Member AddMember(string name, SimaticDataType dataType)
        {
            var member = new Member()
            {
                MemberName = name,
                MemberDataType = dataType.SimaticMLString,
            };
            this.GetItems().Add(member);
            return member;
        }

        public string GetCompleteSymbol()
        {
            return this.GetParentSymbol(this);
        }

        private string GetParentSymbol(XmlNodeConfiguration parentConfiguration)
        {
            if (parentConfiguration is Member parentMember)
            {
                //Wrap the member name in double quotes to "join" all the values toghether. If the name contains special chars (Like a dot) it will create problems.
                return this.GetParentSymbol(parentMember.ParentConfiguration) + "." + SimaticMLUtil.WrapAddressComponentIfRequired(parentMember.MemberName);
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

        public Dictionary<CultureInfo, string> GetComments()
        {
            var dict = new Dictionary<CultureInfo, string>();
            foreach (var item in comment.GetItems())
            {
                dict.Add(item.Lang, item.AsString);
            }
            return dict;
        }

        public override string ToString() => $@"Member: {memberName.AsString}, Type: {dataType.AsString}";
    }
}
