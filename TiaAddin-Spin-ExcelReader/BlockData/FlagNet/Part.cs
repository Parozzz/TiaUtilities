using System.Collections.Generic;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class Part : UIdObject
    {
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
