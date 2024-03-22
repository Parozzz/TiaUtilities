using SpinXmlReader.Block;
using SpinXmlReader;
using System.Collections.Generic;
using System.Globalization;
using TiaXmlReader.Utility;
using System.Xml;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.SimaticML.Attributes;

namespace TiaXmlReader.SimaticML.nBlockAttributeList
{
    //Members can have other members inside (In case of structs)
    public class Member : XmlNodeListConfiguration<Member>
    {
        public const string NODE_NAME = "Member";
        public static Member CreateMember(XmlNode node)
        {
            return node.Name == Member.NODE_NAME ? new Member() : null;
        }

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

        public Member() : base(Member.NODE_NAME, Member.CreateMember, namespaceURI: Constants.GET_SECTIONS_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            memberName = this.AddAttribute("Name", required: true, value: "DefaultName");
            dataType = this.AddAttribute("Datatype", required: true, value: "Bool");
            version = this.AddAttribute("Version");
            remanence = this.AddAttribute("Remanence");
            accessibility = this.AddAttribute("Accessibility");
            informative = this.AddAttribute("Informative");

            attributeList = this.AddNode(new XmlNodeListConfiguration<XmlNodeConfiguration>(Constants.ATTRIBUTE_LIST_KEY, AttributeUtil.CreateAttribute));

            subElement = this.AddNode("Subelement");
            subElementPath = subElement.AddAttribute("Path", required: true);
            subElementStartValue = subElement.AddNode("StartValue", required: true);

            sections = this.AddNodeList("Sections", xmlNode => new Section());

            startValue = this.AddNode("StartValue");

            comment = this.AddNode(new Comment());
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
            if (parentConfiguration is Member parentMember)
            {
                //Wrap the member name in double quotes to "join" all the values toghether. If the name contains special chars (Like a dot) it will create problems.
                return this.GetParentSymbol(parentMember.parentConfiguration) + "." + SimaticMLUtil.WrapAddressComponentIfRequired(parentMember.GetMemberName());
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

        public Member SetCommentText(CultureInfo culture, string text)
        {
            comment.SetText(culture, text);
            return this;
        }

        public Dictionary<CultureInfo, string> GetComments()
        {
            var dict = new Dictionary<CultureInfo, string>();
            foreach (var item in comment.GetItems())
            {
                dict.Add(item.GetLang(), item.GetInnerText());
            }
            return dict;
        }
    }
}
