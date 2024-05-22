using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimaticML.Blocks.FlagNet.nCall;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks.FlagNet.nCall
{
    public class CallParameter : XmlNodeConfiguration
    {
        public const string NODE_NAME = "Parameter";

        private readonly XmlAttributeConfiguration uid; //Only for SCL
        private readonly XmlAttributeConfiguration parameterName;
        private readonly XmlAttributeConfiguration section;
        private readonly XmlAttributeConfiguration type;
        private readonly XmlAttributeConfiguration templateReference; //??
        private readonly XmlAttributeConfiguration informative;

        public CallParameter() : base(CallParameter.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId");

            parameterName = this.AddAttribute("Name", required: true);
            section = this.AddAttribute("Section", required: true);
            type = this.AddAttribute("Type", required: true);
            templateReference = this.AddAttribute("TemplateReference");
            informative = this.AddAttribute("Informative");
            //==== INIT CONFIGURATION ====
        }
    }
}
