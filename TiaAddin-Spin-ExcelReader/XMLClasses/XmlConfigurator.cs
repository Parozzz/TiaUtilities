using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Utility;

namespace TiaXmlReader.XMLClasses
{
    public abstract class XmlConfiguration
    {
        protected readonly string name;
        protected bool required;
        protected XmlNodeConfiguration parentConfiguration { get; set; }

        public XmlConfiguration(string name, bool required = false)
        {
            this.name = name;
            this.required = required;
        }

        public string GetConfigurationName()
        {
            return this.name;
        }

        public bool IsRequired()
        {
            return this.required;
        }

        public void SetRequired()
        {
            this.required = true;
        }

        public XmlNodeConfiguration GetParentConfiguration()
        {
            return parentConfiguration;
        }

        public virtual void SetParentConfiguration(XmlNodeConfiguration parentConfiguration)
        {
            if (this.parentConfiguration != null)
            {
                throw new Exception("Setting a Parent Configuration for a XmlConfiguration that already have it (Double add?) for " + name + ".");
            }
            this.parentConfiguration = parentConfiguration;
        }

        public abstract void Load(XmlNode xmlNode, bool parseUnknown = true);

        public abstract bool IsEmpty();
    }


}
