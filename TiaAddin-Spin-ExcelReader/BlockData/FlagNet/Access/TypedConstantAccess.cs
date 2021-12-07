using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace TiaAddin_Spin_ExcelReader.BlockData
{
    public class TypedConstantAccess : Access
    {

        public static TypedConstantAccess Parse(XmlNode node)
        {
            var constantValue = XmlSearchEngine.Of(node).AddSearch("Constant/ConstantValue").GetLastNode()?.InnerText;
            if(constantValue == null)
            {
                return null;
            }

            return new TypedConstantAccess
            {
                ConstantValue = constantValue
            };
        }

        public string ConstantValue { get; protected internal set; }

        public TypedConstantAccess()
        {

        }
    }
}
