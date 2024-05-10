using System.Xml;
using TiaXmlReader.SimaticML.nBlockAttributeList;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.XMLClasses;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;

namespace TiaXmlReader.SimaticML.Blocks
{
    public class BlockGlobalDB : XmlNodeConfiguration, IGlobalObject
    {
        public const string NODE_NAME = "SW.Blocks.GlobalDB";
        public static XmlNodeConfiguration CreateObjectListConfiguration(XmlNode node)
        {
            switch (node.Name)
            {
                case MultilingualText.NODE_NAME:
                    return MultilingualText.CreateMultilingualText(node);
            }

            return null;
        }

        private readonly GlobalObjectData globalObjectData;

        private readonly BlockAttributeList blockAttributeList;
        private readonly XmlNodeListConfiguration<XmlNodeConfiguration> objectList;

        public BlockGlobalDB() : base(BlockGlobalDB.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            globalObjectData = this.AddAttribute(new GlobalObjectData());

            this.blockAttributeList = this.AddNode(new BlockAttributeList());

            this.objectList = this.AddNodeList(Constants.OBJECT_LIST_KEY, BlockGlobalDB.CreateObjectListConfiguration, required: true);
            //==== INIT CONFIGURATION ====
        }

        public GlobalObjectData GetGlobalObjectData()
        {
            return globalObjectData;
        }

        public void Init()
        {
            this.ComputeBlockTitle().SetText(LocalizationVariables.CULTURE, "");
            this.ComputeBlockComment().SetText(LocalizationVariables.CULTURE, "");

            blockAttributeList.ComputeSection(SectionTypeEnum.STATIC);

            blockAttributeList.SetBlockMemoryReserve(100)
                .SetBlockProgrammingLanguage(SimaticProgrammingLanguage.DB)
                .SetBlockMemoryLayout("Optimized")
                .SetHeaderAuthor(Constants.HEADER_AUTHOR)
                .SetHeaderFamily(Constants.HEADER_FAMILY);
        }

        public MultilingualText ComputeBlockTitle() //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.TITLE)
                {
                    return text;
                }
            }

            var title = new MultilingualText(MultilingualTextType.TITLE);
            objectList.GetItems().Add(title);
            return title;
        }

        public MultilingualText ComputeBlockComment() //Add if does not exists.
        {
            foreach (var item in objectList.GetItems())
            {
                if (item is MultilingualText text && text.GetMultilingualTextType() == MultilingualTextType.COMMENT)
                {
                    return text;
                }
            }

            var comment = new MultilingualText(MultilingualTextType.COMMENT);
            objectList.GetItems().Add(comment);
            return comment;
        }

        public BlockAttributeList GetAttributes()
        {
            return blockAttributeList;
        }

        public List<string> GetAllMemberAddress()
        {
            var section = blockAttributeList.ComputeSection(SectionTypeEnum.STATIC);

            var membersAddressList = new List<string>();
            foreach(var member in section.GetItems())
            {
                var memberChildAddressList = this.GetAddressOfChildMembers(member);

                var memberName = SimaticMLUtil.WrapAddressComponentIfRequired(member.GetMemberName());
                membersAddressList.AddRange(memberChildAddressList.Select(s => this.blockAttributeList.GetBlockName() + "." + memberName + "." + s));
            }

            return membersAddressList;
        }

        private List<string> GetAddressOfChildMembers(Member member)
        {
            var childAddressList = new List<string>();

            var items = member.GetItems();
            if(items.Count == 0)
            {
                var memberName = SimaticMLUtil.WrapAddressComponentIfRequired(member.GetMemberName());
                childAddressList.Add(memberName);
            }
            else
            {
                foreach (var childMember in items)
                { //TO-DO Add arrays into list!
                    var childName = SimaticMLUtil.WrapAddressComponentIfRequired(childMember.GetMemberName());

                    var childItems = childMember.GetItems();
                    if(childItems.Count == 0)
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
