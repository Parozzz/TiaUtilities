using System.Collections.Generic;
using System.Xml;
using TiaXmlReader.Utility;

namespace SpinXmlReader.Block
{
    public class Part : XmlNodeConfiguration, ILocalObject
    {
        public const string NODE_NAME = "Part";
        public static Part CreatePart(XmlNode node)
        {
            return node.Name == Part.NODE_NAME ? new Part() : null;
        }

        public enum Type
        {
            CONTACT,
            COIL,
            NOT_IMPLEMENTED
        }

        private readonly LocalObjectData localObjectData;

        private readonly XmlAttributeConfiguration partName;
        private readonly XmlAttributeConfiguration disabledENO;

        private readonly XmlNodeConfiguration negated; //FOR COIL - CONTACT
        private readonly XmlAttributeConfiguration negatedName;

        private readonly XmlNodeConfiguration automaticTyped;
        private readonly XmlAttributeConfiguration automaticTypedName;

        public Part() : base(Part.NODE_NAME, namespaceURI: Constants.GET_FLAG_NET_NAMESPACE())
        {
            //==== INIT CONFIGURATION ====
            localObjectData = this.AddAttribute(new LocalObjectData());

            partName = this.AddAttribute("Name", required: true);
            disabledENO = this.AddAttribute("DisabledENO");

            negated = this.AddNode("Negated", namespaceURI: Constants.GET_FLAG_NET_NAMESPACE());
            negatedName = negated.AddAttribute("Name");

            automaticTyped = this.AddNode("AutomaticTyped", namespaceURI: Constants.GET_FLAG_NET_NAMESPACE());
            automaticTypedName = automaticTyped.AddAttribute("Name", required: true);

            //==== INIT CONFIGURATION ====
        }
        public LocalObjectData GetLocalObjectData()
        {
            return localObjectData;
        }

        public void SetPartType(Type type)
        {
            switch(type)
            {
                case Type.CONTACT:
                    partName.SetValue("Contact");
                    break;
                case Type.COIL:
                    partName.SetValue("Coil");
                    break;
                default:
                    throw new System.Exception("PartType not implemented yet.");
            }
        }

        public Type GetPartType()
        {
            switch(partName.GetValue())
            {
                case "Contact":
                    return Type.CONTACT;
                case "Coil":
                    return Type.COIL;
                default:
                    return Type.NOT_IMPLEMENTED;
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
                case Type.CONTACT:
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
