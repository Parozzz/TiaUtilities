using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Drawing;
using System.Windows.Forms;
using TiaUtilities.Generation.Configuration.Lines;

namespace TiaXmlReader.Generation.Configuration
{
    public abstract class ConfigLine<LINE> : IConfigLine where LINE : ConfigLine<LINE>
    {
        private bool labelOnTop = false;
        private string? labelText = null;
        private Font? labelFont = null;

        private int height = 0; // 0 = means use standard value in ConfigForm

        public ConfigLine() { }

        public LINE LabelOnTop()
        {
            this.labelOnTop = true;
            return (LINE)this;
        }

        public LINE LabelText(string labelText)
        {
            this.labelText = labelText;
            return (LINE)this;
        }

        public LINE LabelFont(Font labelFont)
        {
            this.labelFont = labelFont;
            return (LINE)this;
        }

        public LINE Height(int height)
        {
            this.height = height;
            return (LINE)this;
        }

        public virtual LINE ControlText(IConvertible? value)
        {
            var control = this.GetControl();
            if (control != null)
            {
                control.Text = (value ?? "").ToString();
            }

            return (LINE)this;
        }

        public string? GetLabelText()
        {
            return labelText;
        }

        public bool IsLabelOnTop()
        {
            return labelOnTop;
        }

        public Font? GetLabelFont() //NULLABLE
        {
            return labelFont;
        }

        public int GetHeight()
        {
            return height;
        }

        public abstract Control? GetControl();
    }
}
