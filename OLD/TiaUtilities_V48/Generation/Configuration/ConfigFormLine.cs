using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public abstract class ConfigFormLine<LINE> : IConfigLine where LINE : ConfigFormLine<LINE>
    {
        private bool labelOnTop = false;
        private string labelText = null;
        private Font labelFont = null;

        private int height = 0; // 0 = means use standard value in ConfigForm

        public ConfigFormLine() { }

        public LINE LabelOnTop()
        {
            this.labelOnTop = true;
            return (LINE) this;
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

        public string GetLabelText()
        {
            return labelText;
        }

        public bool IsLabelOnTop()
        {
            return labelOnTop;
        }

        public Font GetLabelFont() //NULLABLE
        {
            return labelFont;
        }

        public int GetHeight()
        {
            return height;
        }

        public abstract Control GetControl();
    }

    public class ConfigFormLineType<C> where C : ConfigFormLine<C>
    {
        internal ConfigFormLineType() { }

        public override string ToString()
        {
            return typeof(C).Name;
        }
    }

    public static class ConfigFormLineTypes
    {
        public static ConfigFormLineType<ConfigFormLabelLine> LABEL = new ConfigFormLineType<ConfigFormLabelLine>();
        public static ConfigFormLineType<ConfigFormTextBoxLine> TEXT_BOX = new ConfigFormLineType<ConfigFormTextBoxLine>();
        public static ConfigFormLineType<ConfigFormComboBoxLine> COMBO_BOX = new ConfigFormLineType<ConfigFormComboBoxLine>();
        public static ConfigFormLineType<ConfigFormCheckBoxLine> CHECK_BOX = new ConfigFormLineType<ConfigFormCheckBoxLine>();
        public static ConfigFormLineType<ConfigFormButtonPanelLine> BUTTON_PANEL = new ConfigFormLineType<ConfigFormButtonPanelLine>();
        public static ConfigFormLineType<ConfigFormColorPickerLine> COLOR_PICKER = new ConfigFormLineType<ConfigFormColorPickerLine>();
        public static ConfigFormLineType<ConfigFormJavascriptLine> JAVASCRIPT = new ConfigFormLineType<ConfigFormJavascriptLine>();
        public static ConfigFormLineType<ConfigFormJSONLine> JSON = new ConfigFormLineType<ConfigFormJSONLine>();
    }
}
