using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Engineering.SW.Tags;

namespace FCFBConverter
{
    class TagsDatabase
    {

        private readonly Dictionary<string, Tag> tagNameDictionary;
        private readonly Dictionary<string, Tag> tagLogicalAddressDictionary;

        public TagsDatabase()
        {
            tagNameDictionary = new Dictionary<string, Tag>();
            tagLogicalAddressDictionary = new Dictionary<string, Tag>();
        }

        public void LoadTags(PlcTagTable table)
        {
            foreach(PlcTag plcTag in table.Tags)
            {
                string comment = plcTag.Comment.Items.Count != 0 ? plcTag.Comment.Items[0].Text : null;
                var tag = new Tag()
                {
                    Name = plcTag.Name,
                    Comment = comment,
                    DataTypeName = plcTag.DataTypeName,
                    LogicalAddress = plcTag.LogicalAddress,
                };

                if(!tagNameDictionary.ContainsKey(tag.Name))
                {
                    tagNameDictionary.Add(tag.Name, tag);
                }

                if (!tagLogicalAddressDictionary.ContainsKey(tag.LogicalAddress))
                {
                    tagLogicalAddressDictionary.Add(tag.LogicalAddress, tag);
                }
            }
        }
    }

    class Tag
    {
        public string Name {  get; set; }

        public string Comment {  get; set; }

        public string DataTypeName {  get; set; }

        public string LogicalAddress { get; set;  }
    }
}
