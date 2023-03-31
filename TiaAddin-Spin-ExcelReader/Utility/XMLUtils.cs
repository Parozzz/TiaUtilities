using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace SpinXmlReader
{
    public static class XMLUtils
    {
        public static XmlNamespaceManager GetNamespace(XmlDocument document, string id, string namespaceURI)
        {
            var nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("net", namespaceURI); //The section has different workspace.
            return nsmgr;
        }

        public static XmlNode GetFirstChild(XmlNode node, string childName)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == childName)
                {
                    return childNode;
                }
            }

            return null;
        }

        public static XmlNode GetFirstSubChild(XmlNode node, string childAddress)
        {
            var childNodeNames = childAddress.Split('/');
            return GetFirstSubChildInner(node, childNodeNames, 0);
        }

        private static XmlNode GetFirstSubChildInner(XmlNode node, string[] childNameArray, int index)
        {
            if (node == null || childNameArray.Length == 0)
            {
                return null;
            }

            var childName = childNameArray[index];

            var child = XMLUtils.GetFirstChild(node, childName);
            if (child == null || childNameArray.Length == (index + 1)) //If is the last value of the array
            {
                return child;
            }
            else
            {
                return XMLUtils.GetFirstSubChildInner(child, childNameArray, index + 1);
            }
        }

        public static List<XmlNode> GetAllChild(XmlNode node, string name)
        {
            var collection = new List<XmlNode>();
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == name)
                {
                    collection.Add(childNode);
                }
            }
            return collection;
        }

        public static XmlNode AppendElement(XmlDocument document, XmlNode mainNode, string newNodeName, object newNodeInnerText)
        {
            var newChildNode = document.CreateNode(XmlNodeType.Element, newNodeName, "");
            newChildNode.InnerText = newNodeInnerText.ToString();
            mainNode.AppendChild(newChildNode);
            return newChildNode;
        }

        public static void AppendAttribute(XmlDocument document, XmlNode mainNode, string newAttributeName, object newAttributeValue)
        {
            var newAttribute = document.CreateAttribute(newAttributeName);
            newAttribute.Value = newAttributeValue.ToString();
            mainNode.Attributes.Append(newAttribute);
        }
    }

    /*
    public class XmlNodeBuilder
    {
        public static XmlNodeBuilder CreateNew(XmlDocument document, string nodeName)
        {
            var node = document.CreateNode(XmlNodeType.Element, nodeName, "");
            return new XmlNodeBuilder(document, node);
        }

        public static XmlNodeBuilder CreateNewWithNamespace(XmlDocument document, string nodeName, string namespaceURI)
        {
            var node = document.CreateNode(XmlNodeType.Element, nodeName, namespaceURI);
            return new XmlNodeBuilder(document, node);
        }

        private readonly XmlDocument document;
        private readonly XmlNode node;
        public XmlNodeBuilder(XmlDocument document, XmlNode node)
        {
            this.document = document;
            this.node = node;
        }

        public XmlNodeBuilder InnerText(string innerText)
        {
            node.InnerText = innerText;
            return this;
        }

        public XmlNodeBuilder AppendChild(string newNodeName, object newNodeInnerText)
        {
            XMLUtils.AppendElement(document, node, newNodeName, newNodeInnerText);
            return this;
        }

        public XmlNodeBuilder AppendChild(string newNodeName, Action<XmlNodeBuilder> childNodeBuilderAction)
        {
            var childNode = document.CreateNode(XmlNodeType.Element, newNodeName, "");

            var childNodeBuilder = new XmlNodeBuilder(document, childNode);
            childNodeBuilderAction.Invoke(childNodeBuilder);
            return this;
        }

        public XmlNodeBuilder AppendChild(XmlNode childNode)
        {
            node.AppendChild(childNode);
            return this;
        }

        public XmlNodeBuilder AppendSerializableAsChild(IXMLNodeSerializable serializable)
        {
            var childNode = serializable.GenerateXmlNode(document);
            return this.AppendChild(childNode);
        }

        public XmlNodeBuilder AppendSerializableCollectionAsChild<T>(ICollection<T> serializableColl) where T : IXMLNodeSerializable
        {
            foreach (var serializable in serializableColl)
            {
                var childNode = serializable.GenerateXmlNode(document);
                this.AppendChild(childNode);
            }

            return this;
        }

        public XmlNodeBuilder AppendAttribute(string newAttributeName, object newAttributeValue)
        {
            XMLUtils.AppendAttribute(document, node, newAttributeName, newAttributeValue);
            return this;
        }

        /// <summary> Set the XmlNode inside this builder as a child to parentNode </summary>
        public void ChildTo(XmlNode parentNode)
        {
            parentNode.AppendChild(node);
        }

        /// <summary> Set the XmlNode inside this builder as a child to the XmlNode inside parentBuilder </summary>
        public void ChildToBuilder(XmlNodeBuilder parentBuilder)
        {
            parentBuilder.AppendChild(node);
        }

        public XmlNode GetNode()
        {
            return node;
        }
    }
    */
    public class XmlSearchEngine
    {
        public static XmlSearchEngine Of(XmlNode node)
        {
            return new XmlSearchEngine(node);
        }

        private readonly XmlNode mainNode;
        private readonly List<XmlSearchNode> searchNodeList;
        private XmlSearchEngine(XmlNode mainNode)
        {
            this.mainNode = mainNode;
            this.searchNodeList = new List<XmlSearchNode>();
        }

        public XmlSearchNode AddSearch(string multipleNodeName)
        {
            var childNodeNameArray = multipleNodeName.Split('/');
            foreach (string childNodeName in childNodeNameArray)
            {
                searchNodeList.Add(new XmlSearchNode(this, childNodeName));
            }

            return searchNodeList[searchNodeList.Count - 1];
        }

        public XmlNode GetFirstNode(string multipleNodeName)
        {
            return this.AddSearch(multipleNodeName).GetFirstNode();
        }

        public XmlNode GetFirstNode()
        {
            if (searchNodeList.Count == 0 || mainNode == null)
            {
                return mainNode;
            }

            var loopNode = mainNode;
            foreach (var searchNode in searchNodeList)
            {
                loopNode = searchNode.FindFirstValidChild(loopNode);
                if (loopNode == null)
                {
                    return null;
                }
            }

            return loopNode;
        }

        public bool HasAnyNode()
        {
            return GetFirstNode() != null;
        }

        public List<XmlNode> GetAllNodes(string multipleNodeName)
        {
            return this.AddSearch(multipleNodeName).GetAllNodes();
        }

        public List<XmlNode> GetAllNodes()
        {
            XmlNode searchChildNode;
            if (searchNodeList.Count == 0 || mainNode == null)
            {
                return new List<XmlNode>();
            }
            else if (searchNodeList.Count == 1)
            {
                searchChildNode = mainNode;
            }
            else
            {
                var loopNode = mainNode;
                for (int x = 0; x < (searchNodeList.Count - 1); x++)
                {
                    loopNode = searchNodeList[x].FindFirstValidChild(loopNode);
                    if (loopNode == null)
                    {
                        return new List<XmlNode>();
                    }
                }
                searchChildNode = loopNode;
            }

            var lastSearchNode = searchNodeList[searchNodeList.Count - 1];

            var nodeList = new List<XmlNode>();
            foreach (XmlNode child in searchChildNode.ChildNodes)
            {
                if (lastSearchNode.IsValid(child))
                {
                    nodeList.Add(child);
                }
            }
            return nodeList;
        }
    }

    public class XmlSearchNode
    {
        private readonly XmlSearchEngine engine;
        private readonly string nodeName;
        private readonly List<KeyValuePair<string, string>> attributeRequiredList;

        internal XmlSearchNode(XmlSearchEngine engine, string nodeName)
        {
            this.engine = engine;
            this.nodeName = nodeName;
            this.attributeRequiredList = new List<KeyValuePair<string, string>>();
        }

        public XmlSearchNode AttributeRequired(string name, string value)
        {
            attributeRequiredList.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        public XmlSearchNode NextSearch(string childNodeName)
        {
            return engine.AddSearch(childNodeName);
        }

        public XmlNode GetFirstNode()
        {
            return engine.GetFirstNode();
        }

        public bool HasAnyNode()
        {
            return GetFirstNode() != null;
        }

        public List<XmlNode> GetAllNodes()
        {
            return engine.GetAllNodes();
        }

        internal bool IsValid(XmlNode node)
        {
            if (node.Name != nodeName)
            {
                return false;
            }

            bool allAttributesOK = true;
            foreach (KeyValuePair<string, string> requiredAttributes in attributeRequiredList)
            {
                var attribute = node.Attributes[requiredAttributes.Key];
                if (attribute == null || attribute.Value != requiredAttributes.Value)
                {
                    allAttributesOK = false;
                }
            }
            return allAttributesOK;
        }

        internal XmlNode FindFirstValidChild(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (IsValid(childNode))
                {
                    return childNode;
                }
            }

            return null;
        }
    }
}
