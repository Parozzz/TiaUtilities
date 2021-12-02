using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader.Utility
{
    public static class XmlUtil
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

            var child = XmlUtil.GetFirstChild(node, childName);
            if (child == null || childNameArray.Length == (index + 1)) //If is the last value of the array
            {
                return child;
            }
            else
            {
                return XmlUtil.GetFirstSubChildInner(child, childNameArray, index + 1);
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
    }

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

        public XmlSearchNode AddSearch(string childNodeName)
        {
            var searchNode = new XmlSearchNode(this, childNodeName);
            searchNodeList.Add(searchNode);
            return searchNode;
        }

        public XmlSearchEngine AddMultipleSearch(string multipleNodeName)
        {
            var childNodeNameArray = multipleNodeName.Split('/');
            foreach (string childNodeName in childNodeNameArray)
            {
                AddSearch(childNodeName);
            }

            return this;
        }

        public XmlNode Search()
        {
            if (searchNodeList.Count == 0 || mainNode == null)
            {
                return mainNode;
            }

            var loopNode = mainNode;
            foreach(var searchNode in searchNodeList)
            {
                loopNode = searchNode.SearchChilds(loopNode);
                if(loopNode == null)
                {
                    return null;
                }
            }

            return loopNode;
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

        public XmlSearchEngine NextMultipleSearch(string multipleNodeName)
        {
            return engine.AddMultipleSearch(multipleNodeName);
        }

        public XmlNode Search()
        {
            return engine.Search();
        }

        internal XmlNode SearchChilds(XmlNode node)
        {
            foreach (XmlNode childNode in XmlUtil.GetAllChild(node, nodeName))
            {
                bool allAttributesOK = true;
                foreach (KeyValuePair<string, string> requiredAttributes in attributeRequiredList)
                {
                    var attribute = childNode.Attributes[requiredAttributes.Key];
                    if (attribute == null || attribute.Value != requiredAttributes.Value)
                    {
                        allAttributesOK = false;
                    }
                }

                if (allAttributesOK)
                {
                    return childNode;
                }
            }

            return null;
        }
    }
}
