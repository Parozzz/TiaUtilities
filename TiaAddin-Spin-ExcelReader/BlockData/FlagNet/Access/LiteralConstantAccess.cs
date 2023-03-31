using System.Xml;
using TiaAddin_Spin_ExcelReader.Utility;

namespace SpinXmlReader.Block
{
    public class LiteralConstantAccess : Access
    {

        public static LiteralConstantAccess Parse(XmlNode node)
        {
            var constantType = XmlSearchEngine.Of(node).AddSearch("Constant/ConstantType").GetFirstNode()?.InnerText;
            var constantValue = XmlSearchEngine.Of(node).AddSearch("Constant/ConstantValue").GetFirstNode()?.InnerText;
            if(constantType == null || constantValue == null)
            {
                return null;
            }


            return new LiteralConstantAccess()
            {
                ConstantType = constantType,
                ConstantValue = constantValue
            };
        }

        public string ConstantType { get; internal protected set; }
        public string ConstantValue { get; internal protected set; }

        internal LiteralConstantAccess() {  }
    }
}
