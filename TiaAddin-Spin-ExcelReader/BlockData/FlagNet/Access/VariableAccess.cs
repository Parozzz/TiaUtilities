using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace SpinXmlReader.Block
{
    public class VariableAccess : Access
    {
        public static VariableAccess Parse(XmlNode node)
        {
            StringBuilder symbolBuilder = new StringBuilder();
            foreach (XmlNode componentNode in XmlSearchEngine.Of(node).AddSearch("Symbol/Component").GetAllNodes())
            {
                symbolBuilder.Append('.')
                    .Append(componentNode.Attributes["Name"].Value)
                    .Append('.');
            }

            if(symbolBuilder.Length == 0) //Empty string = No components = Error
            {
                return null;
            }

            var symbol = symbolBuilder.Remove(0, 1) //Remove the initial dot
                .Remove(symbolBuilder.Length - 1, 1)  //Remove the final dot
                .ToString();

            return new VariableAccess()
            {
                Symbol = symbol
            };
        }

        public string Symbol { get; protected internal set; }


        public VariableAccess()
        {

        }
    }
}
