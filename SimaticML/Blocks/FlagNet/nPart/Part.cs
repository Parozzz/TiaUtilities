using SimaticML.Blocks.FlagNet.nCall;
using SimaticML.Enums;
using SimaticML.LanguageText;
using SimaticML.XMLClasses;

namespace SimaticML.Blocks.FlagNet.nPart
{
    public enum PartType
    {
        [SimaticEnum("Contact")] CONTACT,
        [SimaticEnum("Coil")] COIL,
        [SimaticEnum("SCoil")] SET_COIL,
        [SimaticEnum("RCoil")] RESET_COIL,
        [SimaticEnum("Not")] NOT,
        [SimaticEnum("TON")] TON,
        [SimaticEnum("TOF")] TOF,
        [SimaticEnum("Move")] MOVE,
        [SimaticEnum("O")] OR,
        UNKNOWN = 0 //Default value. Do not implement SimaticEnum so an exception is thrown!
    }

    public class Part : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Part";

        public PartType PartType { get => this.partName.Value.AsEnum<PartType>(); set => this.partName.Value.AsEnum(value); }
        public string Negated { get => this.negatedName.Value.AsString; set => this.negatedName.Value.AsString = value; }
        public bool DisabledENO { get => this.disabledENO.Value.AsBool; set => this.disabledENO.Value.AsBool = value; }

        public bool AutomaticTyped { get => this.automaticTyped.Value.AsBool; set => this.automaticTyped.Value.AsBool = value; }
        public string AutomaticTypedName { get => this.automaticTypedName.Value.AsString; set => this.automaticTypedName.Value.AsString = value; }

        public string TemplateValue { get => this.templateValue.Value.AsString; set => this.templateValue.Value.AsString = value; }
        public string TemplateValueName { get => this.templateValueName.Value.AsString; set => this.templateValueName.Value.AsString = value; }
        public string TemplateValueType { get => this.templateValueType.Value.AsString; set => this.templateValueType.Value.AsString = value; }

        public string Version { get => this.version.Value.AsString; set => this.version.Value.AsString = value; }

        public CallInstance Instance { get => this.instance; }
        public Comment Comment { get => this.comment; }


        private readonly XmlAttributeConfiguration uid;
        private readonly XmlAttributeConfiguration partName;
        private readonly XmlAttributeConfiguration disabledENO;
        private readonly XmlAttributeConfiguration version; //This is not required. If omitted, it should use the project version.

        private readonly XmlNodeConfiguration templateValue;
        private readonly XmlAttributeConfiguration templateValueName;
        private readonly XmlAttributeConfiguration templateValueType;

        private readonly XmlNodeConfiguration negated; //FOR COIL - CONTACT
        private readonly XmlAttributeConfiguration negatedName;

        private readonly XmlNodeConfiguration automaticTyped;
        private readonly XmlAttributeConfiguration automaticTypedName;

        private readonly CallInstance instance;

        private readonly Comment comment;

        public Part() : base(Part.NODE_NAME)
        {
            //==== INIT CONFIGURATION ====
            uid = this.AddAttribute("UId", required: true);
            partName = this.AddAttribute("Name", required: true);
            disabledENO = this.AddAttribute("DisabledENO");
            version = this.AddAttribute("Version");

            negated = this.AddNode("Negated");
            negatedName = negated.AddAttribute("Name");

            automaticTyped = this.AddNode("AutomaticTyped");
            automaticTypedName = automaticTyped.AddAttribute("Name", required: true);

            //L'ORDINE TRA INSTANCE E TEMPLATE VALUE è IMPORTANTE! NON MUOVERE.
            instance = this.AddNode(new CallInstance());

            comment = this.AddNode(new Comment()); //A part can have a comment in the little resizable square.

            templateValue = this.AddNode("TemplateValue");
            templateValueName = templateValue.AddAttribute("Name");
            templateValueType = templateValue.AddAttribute("Type");
            //Type=Cardinality means how many connections that block have. For O (OR), it means how many input connections it has. For move, how many outputs.

            //==== INIT CONFIGURATION ====
        }

        public void UpdateLocalUId(IDGenerator localIDGeneration)
        {
            this.SetUId(localIDGeneration.GetNext());
            if (!this.instance.IsEmpty())
            {
                this.instance.UpdateLocalUId(localIDGeneration);
            }
        }

        public void SetUId(uint uid)
        {
            this.uid.Value.AsUInt = uid;
        }

        public uint GetUId()
        {
            return this.uid.Value.AsUInt;
        }

        /*
        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }*/

        public override string ToString() => $"{base.ToString()}, Type: {this.PartType}";
    }
}
