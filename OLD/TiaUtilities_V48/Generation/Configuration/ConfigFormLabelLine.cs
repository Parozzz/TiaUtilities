using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormLabelLine : ConfigFormLine<ConfigFormLabelLine>
    {
        public ConfigFormLabelLine()
        {
        }

        public override Control GetControl()
        {
            return null;
        }
    }
}
