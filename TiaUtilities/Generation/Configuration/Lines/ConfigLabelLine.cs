using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigLabelLine : ConfigLine<ConfigLabelLine>
    {
        public ConfigLabelLine()
        {
        }

        public override Control GetControl()
        {
            return null;
        }
    }
}
