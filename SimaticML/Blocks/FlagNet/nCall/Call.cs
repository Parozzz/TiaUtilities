using SimaticML.LanguageText;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks.FlagNet.nCall
{

    public class Call : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Call";

        public CallInfo CallInfo { get => this.callInfo; }
        public Comment Comment { get => this.comment; }

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

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());

            var instance = callInfo.Instance;
            if (!instance.IsEmpty())
            {
                instance.UpdateLocalUId(localIDGeneration);
            }
        }

        public void SetUId(uint uid)
        {
            this.uid.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.AsUInt;
        }
    }
}
