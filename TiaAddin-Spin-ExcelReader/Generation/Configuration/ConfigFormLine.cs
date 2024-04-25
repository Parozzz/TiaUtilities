
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public class ConfigFormLine
    {
        private readonly string labelText;
        private readonly int height;

        public ConfigFormLine(string labelText, int height = 0) //Height = 0 means use standard value in ConfigForm
        {
            this.labelText = labelText;
            this.height = height;
        }

        public string GetLabelText()
        {
            return labelText;
        }

        public int GetHeight()
        {
            return height;
        }

        public virtual Control GetControl()
        {
            return null;
        }
    }
}
