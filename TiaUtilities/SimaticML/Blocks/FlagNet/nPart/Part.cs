using System;
using System.Linq;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nCall;
using TiaXmlReader.SimaticML.LanguageText;
using TiaXmlReader.XMLClasses;

namespace TiaXmlReader.SimaticML.Blocks.FlagNet.nPart
{
    public enum PartType
    {
        CONTACT,
        COIL,
        SET_COIL,
        RESET_COIL,
        NOT,
        TON,
        TOF,
        UNKNOWN = 0 //Default value.
    }

    public static class PartTypeExtension
    {
        public static string GetSimaticMLString(this PartType partType)
        {
            switch (partType)
            {
                case PartType.CONTACT: return "Contact";
                case PartType.COIL: return "Coil";
                case PartType.SET_COIL: return "SCoil";
                case PartType.RESET_COIL: return "RCoil";
                case PartType.NOT: return "Not";
                case PartType.TON: return "TON";
                case PartType.TOF: return "TOF";
                default:
                    throw new Exception("Part " + partType.ToString() + "  not yet implemented");
            }

        }
    }

    public class Part : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Part";

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

        private readonly Instance instance;

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
            instance = this.AddNode(new Instance());

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
            if(!this.instance.IsEmpty())
            {
                this.instance.UpdateLocalUId(localIDGeneration);
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

        /*
        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }*/

        public Part SetPartType(PartType type)
        {
            partName.SetValue(type.GetSimaticMLString());
            return this;
        }

        public PartType GetPartType()
        {
            return Enum.GetValues(typeof(PartType))
                .Cast<PartType>()
                .SingleOrDefault(partType => partType.GetSimaticMLString() == partName.GetValue());
        }

        public bool IsDisabledENO()
        {
            return disabledENO.IsParsed() && bool.Parse(disabledENO.GetValue());
        }

        public Part SetIsDisableENO(bool isDisable)
        {
            disabledENO.SetValue(isDisable.ToString());
            return this;
        }

        public string GetVersion()
        {
            return version.GetValue();
        }

        public Part SetVersion(string version)
        {
            this.version.SetValue(version);
            return this;
        }

        public bool IsNegated()
        {
            return negated.IsParsed();
        }

        public Part SetNegated()
        {
            var partType = this.GetPartType();
            switch (partType)
            {
                case PartType.CONTACT:
                    negatedName.SetValue("operand");
                    break;
            }
            return this;
        }

        public bool IsAutomaticTyped()
        {
            return automaticTyped.IsParsed();
        }

        public string GetAutomaticTypedName()
        {
            return IsAutomaticTyped() ? automaticTypedName.GetValue() : "";
        }

        public Instance GetPartInstance()
        {
            return instance;
        }

        public string GetTemplateValue()
        {
            return this.templateValue.GetInnerText();
        }

        public Part SetTemplateValue(string value)
        {
            this.templateValue.SetInnerText(value);
            return this;
        }

        public string GetTemplateValueName()
        {
            return templateValueName.GetValue();
        }

        public Part SetTemplateValueName(string templateValueName)
        {
            this.templateValueName.SetValue(templateValueName);
            return this;
        }

        public string GetTemplateValueType()
        {
            return this.templateValueType.GetValue();
        }

        public Part SetTemplateValueType(string templateValueType)
        {
            this.templateValueType.SetValue(templateValueType);
            return this;
        }

    }
}
