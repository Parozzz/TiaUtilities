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
        //Nullable
        private string compositionName;

        public GlobalObjectData(uint id, string compositionName)
        {
            this.id = id;
            this.compositionName = compositionName;
        }

        public GlobalObjectData(string compositionName) : this(0, compositionName) { }

        public GlobalObjectData() : this(0, "") { }

        public GlobalObjectData GenerateNextID()
        {
            this.id = GlobalIDGenerator.GetNextID();
            return this;
        }

        public void ParseNode(XmlNode node)
        {
            if (node.Attributes["ID"] == null)
            {
                throw new InvalidOperationException("Missing ID of IGlobalObject Attributes from node " + node.Name);
            }

            id = uint.Parse(node.Attributes["ID"].InnerText, NumberStyles.HexNumber);
            compositionName = node.Attributes["CompositionName"]?.InnerText; //A global object might not have a composition name (Like the main node of a XML)
        }

        public void SetToNode(XmlNode node)
        {
            node.Attributes.Append(node.OwnerDocument.CreateAttribute("ID")).InnerText = id.ToString("X"); //This is an HEX value
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
