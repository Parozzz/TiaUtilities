using System.Collections.ObjectModel;
using System.Xml;

namespace SimaticML.XMLClasses
{
    public class XmlNodeListConfiguration<T> : XmlNodeConfiguration where T : XmlNodeConfiguration
    {
        private readonly Func<XmlNode, T?> creationFunction;
        private readonly ObservableCollection<T> items;

        public XmlNodeListConfiguration(string name, Func<XmlNode, T?> creationFunction, bool required = false, string namespaceURI = "", string defaultInnerText = "") : base(name, required, namespaceURI, defaultInnerText)
        {
            this.creationFunction = creationFunction;

            this.items = [];
            this.items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (XmlNodeConfiguration newItem in e.NewItems)
                {
                    newItem.SetParentConfiguration(this);
                }
            }
        }

        public override void Load(XmlNode xmlNode, bool parseUnknown = true)
        {
            base.Load(xmlNode, parseUnknown: false);

            foreach (XmlNode childNode in base.xmlElement.ChildNodes)
            {
                var configuration = creationFunction.Invoke(childNode);
                if (configuration != null)
                {
                    items.Add(configuration);
                    configuration.Load(childNode);

                    base.parseUnkownChilds.Remove(childNode);
                }
            }

            base.ParseUnkown();
        }

        public override void UpdateID_UId(IDGenerator globalIDGeneration)
        {
            base.UpdateID_UId(globalIDGeneration);
            foreach (var nodeConfig in this.items)
            {
                nodeConfig.UpdateID_UId(globalIDGeneration);
            }
        }

        public override bool IsEmpty()
        {
            return base.IsEmpty() && items.Count == 0;
        }

        public override XmlElement Generate(XmlDocument document)
        {
            var xmlElement = base.Generate(document);

            foreach (var item in items)
            {
                var itemXmlElement = item.Generate(document);
                if (itemXmlElement != null)
                {
                    xmlElement.AppendChild(itemXmlElement);
                }
            }

            return xmlElement;
        }

        public ObservableCollection<T> GetItems()
        {
            return items;
        }

        public override string ToString() => $"{base.ToString()}, Items: {this.items.Count}";
    }
}
