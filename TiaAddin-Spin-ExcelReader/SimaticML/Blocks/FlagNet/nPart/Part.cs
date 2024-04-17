using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Linq;
using System.Xml;
using TiaXmlReader.Utility;
using TiaXmlReader.SimaticML;
using TiaXmlReader.SimaticML.Blocks;
using TiaXmlReader.SimaticML.Blocks.FlagNet.nCall;

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
        public static Part CreatePart(CompileUnit compileUnit, XmlNode node)
        {
            return node.Name == Part.NODE_NAME ? new Part(compileUnit) : null;
        }

        private readonly LocalObjectData localObjectData;

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

        public Part(CompileUnit compileUnit) : base(Part.NODE_NAME)
        {
            compileUnit.AddPart(this);

            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData(compileUnit.LocalIDGenerator));

            partName = this.AddAttribute("Name", required: true);
            disabledENO = this.AddAttribute("DisabledENO");
            version = this.AddAttribute("Version");

            negated = this.AddNode("Negated");
            negatedName = negated.AddAttribute("Name");

            automaticTyped = this.AddNode("AutomaticTyped");
            automaticTypedName = automaticTyped.AddAttribute("Name", required: true);

            //L'ORDINE TRA INSTANCE E TEMPLATE VALUE è IMPORTANTE! NON MUOVERE.
            instance = this.AddNode(new Instance(compileUnit));

            templateValue = this.AddNode("TemplateValue");
            templateValueName = templateValue.AddAttribute("Name");
            //Cardinality means how many connections that block have. For O for example, it means how many input connections it has. For move, how many outputs.
            templateValueType = templateValue.AddAttribute("Type");
            //==== INIT CONFIGURATION ====
        }
        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }

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
