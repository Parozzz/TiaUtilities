using System.Xml;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.Utility;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nCall
{

    public class Call : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Call";

        private readonly XmlAttributeConfiguration uid;
        private readonly CallInfo callInfo;
        private readonly Comment comment;

        public Call() : base(Call.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId", required: true);

            callInfo = this.AddNode(new CallInfo());
            comment = this.AddNode(new Comment());
            //==== INIT CONFIGURATION ====
        }

        public CallInfo GetCallInfo()
        {
            return callInfo;
        }

        public Comment GetComment()
        {
            return comment;
        }

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());

            var instance = callInfo.GetInstance();
            if (!instance.IsEmpty())
            {
                instance.UpdateLocalUId(localIDGeneration);
            }
        }

        public void SetUId(uint uid)
        {
            this.uid.SetValue("" + uid);
        }

        public uint GetUId()
        {
            return uint.TryParse(this.uid.GetValue(), out uint uid) ? uid : 0;
        }
    }
}
