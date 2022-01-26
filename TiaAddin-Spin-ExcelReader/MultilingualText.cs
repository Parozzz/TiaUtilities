using SpinAddIn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.BlockData;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader
{
    public class MultilingualText : XmlNodeSerializable, GlobalIDObject
    {
        public static MultilingualText FindInParent(XmlNode parent, string compositionName)
        {
            var node = XmlSearchEngine.Of(parent).AddSearch("MultilingualText")
                .AttributeRequired("CompositionName", compositionName)
                .GetFirstNode();
            if (node == null)
            {
                return null;
            }

            return new MultilingualText(node);
        }

        private uint id;
        public uint ID { get => id; }

        private string compositionName;
        public string CompositionName { get => compositionName; }

        private readonly Dictionary<CultureInfo, Item> itemDictionary;

        public MultilingualText(XmlNode xmlNode)
        {
            itemDictionary = new Dictionary<CultureInfo, Item>();
            this.DoXMLNode(xmlNode);
        }

        public uint GetID()
        {
            return id;
        }

        public Item FirstItem()
        {
            return itemDictionary.Count != 0 ? itemDictionary.First().Value : null;
        }

        public Item GetByLocale(CultureInfo culture)
        {
            return itemDictionary[culture];
        }

        private MultilingualText DoXMLNode(XmlNode node)
        {
            Validate.NotNull(node);
            Validate.IsTrue(node.Name.Equals("MultilingualText"), "MultilingualText node name is not valid.");

            var parseOK = true;
            parseOK &= uint.TryParse(node.Attributes["ID"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out id);
            parseOK &= Util.TryNotNull(node.Attributes["CompositionName"]?.Value, out compositionName);
            if (parseOK)
            {
                throw new InvalidOperationException("Some of the values inside a MultilingualText has not been parsed correctly");
            }

            var multilingualTextItemNodeList = node.SelectNodes("./ObjectList/MultilingualTextItem");
            if (multilingualTextItemNodeList != null)
            {
                foreach (XmlNode itemNode in multilingualTextItemNodeList)
                {
                    var item = new Item().ParseXMLNode(itemNode);
                    itemDictionary.Add(item.Locale, item);
                }
            }

            return this;
        }

        public XmlNode GenerateXmlNode(XmlDocument document)
        {
            throw new NotImplementedException();
        }



        public class Item : XmlNodeSerializable, GlobalIDObject
        {
            private uint id;
            public uint ID { get => id; }

            private string compositionName;
            public String CompositionName { get => compositionName; }

            private string text;
            public string Text { get => text; }

            private CultureInfo culture;
            public CultureInfo Culture { get => culture; }

            internal Item(XmlNode xmlNode)
            {
                DoXMLNode(xmlNode);
            }

            public Item(GlobalIDGenerator idGenerator, CultureInfo cultureInfo, string text)
            {
                this.id = idGenerator.GetNextID();
                this.compositionName = "Items";
                this.text = text;
                this.culture = cultureInfo;
            }

            public uint GetID()
            {
                return id;
            }

            private Item DoXMLNode(XmlNode node)
            {
                Validate.NotNull(node);
                Validate.IsTrue(node.Name.Equals("MultilingualTextItem"), "MultilingualText node name is not valid.");

                var parseOK = true;
                parseOK &= uint.TryParse(node.Attributes["ID"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out id);
                parseOK &= Util.TryNotNull(node.Attributes["CompositionName"]?.Value, out compositionName);
                parseOK &= (compositionName == "Items");
                if (parseOK)
                {
                    throw new InvalidOperationException("Some of the values inside a MultilingualTextItem has not been parsed correctly");
                }

                var cultureNode = XmlSearchEngine.Of(node).GetFirstNode("AttributeList/Culture");
                var textNode = XmlSearchEngine.Of(node).GetFirstNode("AttributeList/Text");
                if (cultureNode == null || textNode == null)
                {
                    throw new InvalidOperationException("Missing nodes inside a MultilingualTextItem/AttributeList with ID=" + id);
                }

                try
                {
                    culture = CultureInfo.GetCultureInfo(cultureNode.InnerText);
                }
                catch (CultureNotFoundException)
                {
                    throw new InvalidOperationException("Invalid CultureInfo inside a MultilingualTextItem/AttributeList/Culture with ID=" + id);
                }

                text = textNode.InnerText;

                return this;
            }

            public XmlNode GenerateXmlNode(XmlDocument document)
            {

                var builder = XmlNodeBuilder.CreateNew(document, "MultilingualTextItem")
                    .AppendAttribute("ID", this.id)
                    .AppendAttribute("CompositionName", this.compositionName)
                    .AppendChild("AttributeList", (childBuilder) => childBuilder.AppendChild("Text", this.text)
                                                                                .AppendChild("Culture", culture.IetfLanguageTag));
                return builder.GetNode();
            }


        }
    }
}
