using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaXmlReader.GenerationForms.IO
{
    public class ConfigTableLayoutPanel : TableLayoutPanel
    {

        private readonly string configName;
        public ConfigTableLayoutPanel(string configName) : base()
        {
            this.configName = configName;
        }

        public void Init()
        {
            base.Name = configName;
        }
    }


    public abstract class ConfigTableLine
    {
        public readonly string ConfigName;

        public abstract TableLayoutPanel GeneratePanel();
    }


    public class ConfigTableTextBoxLine : ConfigTableLine
    {
        public readonly string ConfigName;
        public readonly string LabelName;

        public override TableLayoutPanel GeneratePanel()
        {
            throw new NotImplementedException();
        }
    }
}
