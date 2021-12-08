using System.Collections.Generic;
using System.Xml;

namespace TiaAddin_Spin_ExcelReader
{
    public class Part : UIdObject
    {
        public static Part Parse(XmlNode node)
        {
            var name = node.Attributes["Name"]?.Value;
            if(name == null)
            {
                return null;
            }

            return new Part()
            {
                Name = name
            };
        }

        public string Name { get; internal protected set; }

        private readonly Dictionary<uint, string> connectionDictionary;

        public Part()
        {
            connectionDictionary = new Dictionary<uint, string>();
        }

        public void AddConnection(uint uid, string name)
        {
            connectionDictionary.Add(uid, name);
        }
    }
}
