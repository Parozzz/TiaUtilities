using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SpinXmlReader
{
    //ONLY SAFE INSIDE THE XML GENERATION THREAD
    public static class GlobalIDGenerator
    {
        private static uint counter = 1;

        public static void ResetID()
        {
            counter = 1;
        }

        public static uint GetNextID()
        {
            var ret = counter;
            counter++;
            return ret;
        }
    }

    public class GlobalObjectData
    {
        private uint id;
        private string compositionName;

        public GlobalObjectData(uint id, string compositionName)
        {
            this.id = id;
            this.compositionName = compositionName;
        }

        public GlobalObjectData(string compositionName)
        {
            this.id = GlobalIDGenerator.GetNextID();
            this.compositionName = compositionName;
        }

        public GlobalObjectData()
        {
            this.id = 0;
            this.compositionName = "";
        }

        public void ParseXMLNode(XmlNode node)
        {
            if (node.Attributes["ID"] == null || node.Attributes["CompositionName"] == null)
            {
                throw new InvalidOperationException("Invalid GlobalObject Attributes from node " + node.Name);
            }

            id = uint.Parse(node.Attributes["ID"]?.InnerText, NumberStyles.HexNumber);
            compositionName = node.Attributes["CompositionName"].InnerText;
        }

        public void SetToXMLNode(XmlNode node)
        {
            //This is an HEX value
            node.Attributes.Append(node.OwnerDocument.CreateAttribute("ID")).InnerText = id.ToString("X");
            //Ogni oggetto - ad eccezione dell'oggetto di avvio - contiene anche un attributo XML "CompositionName".
            //Quindi potrebbe essere che questo valore sia vuoto.
            if (compositionName != null && compositionName.Length > 0)
            {
                node.Attributes.Append(node.OwnerDocument.CreateAttribute("CompositionName")).InnerText = compositionName;
            }
        }

        public string GetHexId()
        {
            return id.ToString("X");
        }

        public uint GetId()
        {
            return id;
        }

        public string GetCompositionName()
        {
            return compositionName;
        }
    }

    public interface IGlobalObject
    {
        GlobalObjectData GetGlobalObjectIdata();
    }
}
