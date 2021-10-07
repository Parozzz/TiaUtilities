using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Engineering.SW.Tags;

namespace SpinAddin
{
    public class TagsDatabase
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
                var tag = new Tag();
                tag.ParseTag(plcTag);

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

    public enum TagType
    {
        MERKER,
        OUTPUT,
        INPUT
    }

    public class TagAddress
    {
        public ushort Number { get; private set; }
        public byte Bit { get; private set; }

        public TagAddress(ushort number, byte bit)
        {
            this.Number = number;
            this.Bit = bit;
        }
    }

    public class Tag
    {
        public string Name {  get; set; }

        public string Comment {  get; set; }

        public string DataTypeName {  get; set; }

        public string LogicalAddress { get; set;  }

        public TagType Type { get; private set; }

        public TagAddress Address { get; private set; }

        internal void ParseTag(PlcTag plcTag)
        {
            this.Name = plcTag.Name;
            this.Comment = plcTag.Comment.Items.Count != 0 ? plcTag.Comment.Items[0].Text : null; ;
            this.DataTypeName = plcTag.DataTypeName;
            this.LogicalAddress = plcTag.LogicalAddress;

            if (LogicalAddress.StartsWith("%"))
            {
                switch(LogicalAddress.ToUpper()[1])
                {
                    case 'Q':
                        this.Type = TagType.OUTPUT;
                        break;
                    case 'I':
                        this.Type = TagType.INPUT;
                        break;
                    case 'M':
                        this.Type = TagType.MERKER;
                        break;
                }

                ushort addressNumber;
                byte addressBit = 0;
                if (DataTypeName.ToUpper().Equals("BOOL"))
                {
                    var pointIndex = LogicalAddress.IndexOf('.');
                    ushort.TryParse("" + LogicalAddress.Substring(2, pointIndex), out addressNumber);
                    byte.TryParse("" + LogicalAddress[pointIndex + 1], out addressBit);
                }
                else
                {
                    ushort.TryParse("" + LogicalAddress.Substring(2), out addressNumber);
                }

                Address = new TagAddress(addressNumber, addressBit);
            }
        }

        string CreateExcelString()
        {
            return "";
        }
    }
}
