using SpinXmlReader;
using SpinXmlReader.Block;
using SpinXmlReader.SimaticML;
using System;
using System.Xml;
using TiaXmlReader.Utility;

namespace TiaXmlReader.SimaticML.BlockFCFB.FlagNet.PartNamespace
{
    public enum PartType
    {
        CONTACT,
        COIL,
        NOT_IMPLEMENTED
    }

    public static class PartTypeExtension
    {
        public static string GetSimaticMLString(this PartType partType)
        {
            switch(partType)
            {
                case PartType.CONTACT: return "Contact";
                case PartType.COIL: return "Coil";
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

        private readonly XmlNodeConfiguration templateValue;
        private readonly XmlAttributeConfiguration templateValueName;
        private readonly XmlAttributeConfiguration templateValueType;

        private readonly XmlNodeConfiguration negated; //FOR COIL - CONTACT
        private readonly XmlAttributeConfiguration negatedName;

        private readonly XmlNodeConfiguration automaticTyped;
        private readonly XmlAttributeConfiguration automaticTypedName;

        public Part(CompileUnit compileUnit) : base(Part.NODE_NAME)
        {
            compileUnit.AddPart(this);

            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData(compileUnit.LocalIDGenerator));

            partName = this.AddAttribute("Name", required: true);
            disabledENO = this.AddAttribute("DisabledENO");

            templateValue = this.AddNode("TemplateValue");
            templateValueName = templateValue.AddAttribute("Name");
            //Cardinality means how many connections that block have. For O for example, it means how many input connections it has. For move, how many outputs.
            templateValueType = templateValue.AddAttribute("Cardinality");

            negated = this.AddNode("Negated");
            negatedName = negated.AddAttribute("Name");

            automaticTyped = this.AddNode("AutomaticTyped");
            automaticTypedName = automaticTyped.AddAttribute("Name", required: true);

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
            switch(partName.GetValue())
            {
                case "Contact":
                    return PartType.CONTACT;
                case "Coil":
                    return PartType.COIL;
                default:
                    return PartType.NOT_IMPLEMENTED;
            }
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

        public bool IsNegated()
        {
            return negated.IsParsed();
        }
        
        public Part SetNegated()
        {
            var partType = this.GetPartType();
            switch(partType)
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
    }
}
