using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.nBlockAttributeList;
using SimaticML.XMLClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimaticML.Blocks
{
    public abstract class BlockDB : XmlNodeConfiguration, IGlobalObject
    {
        public static XmlNodeConfiguration? CreateObjectListConfiguration(XmlNode node)
        {
            return node.Name switch
            {
                MultilingualText.NODE_NAME => MultilingualText.CreateMultilingualText(node),
                _ => null,
            };
        }

        public MultilingualText Title { get => this.CompuleMultilingualText(MultilingualTextType.TITLE); }
        public MultilingualText Comment { get => this.CompuleMultilingualText(MultilingualTextType.COMMENT); }
        public BlockAttributeList AttributeList { get => this.blockAttributeList; }

        private readonly GlobalObjectData globalObjectData;
        public readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockDB(string nodeName) : base(nodeName)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(SimaticMLAPI.OBJECT_LIST_KEY, BlockDB.CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        private MultilingualText CompuleMultilingualText(MultilingualTextType textType) //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.TextType == textType)
                {
                    return text;
                }
            }

            var comment = new MultilingualText(textType);
            objectList.GetItems().Add(comment);
            return comment;
        }

        public List<string> GetAllMemberAddress()
        {
            var membersAddressList = new List<string>();
            foreach (var member in blockAttributeList.STATIC.GetItems())
            {
                var memberChildAddressList = this.GetAddressOfChildMembers(member);

                var memberName = SimaticMLUtil.WrapAddressComponentIfRequired(member.MemberName);
                membersAddressList.AddRange(memberChildAddressList.Select(s => this.blockAttributeList.BlockName + "." + memberName + "." + s));
            }

            return membersAddressList;
        }

        private List<string> GetAddressOfChildMembers(Member member)
        {
            var childAddressList = new List<string>();

            var items = member.GetItems();
            if (items.Count == 0)
            {
                var memberName = SimaticMLUtil.WrapAddressComponentIfRequired(member.MemberName);
                childAddressList.Add(memberName);
            }
            else
            {
                foreach (var childMember in items)
                { //TO-DO Add arrays into list!
                    var childName = SimaticMLUtil.WrapAddressComponentIfRequired(childMember.MemberName);

                    var childItems = childMember.GetItems();
                    if (childItems.Count == 0)
                    {
                        childAddressList.Add(childName);
                    }
                    else
                    {
                        var subChildAddressList = this.GetAddressOfChildMembers(childMember);
                        childAddressList.AddRange(subChildAddressList.Select(s => childName + "." + s));
                    }
                }
            }

            return childAddressList;
        }
    }
}
