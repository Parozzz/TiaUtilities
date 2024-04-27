using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using TiaXmlReader.Utility;

namespace TiaXmlReader.Utility
{
    public static class XMLUtils
    {
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
    }
}
