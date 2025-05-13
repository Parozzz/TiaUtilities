using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.Configuration
{
    public abstract class ConfigLine<LINE> : IConfigLine where LINE : ConfigLine<LINE>
    {
        private bool labelOnTop = false;
        private ObservableObject<string?> labelText = new(null);
        private Font? labelFont = null;

        private int height = 0; // 0 = means use standard value in ConfigForm
        private bool controlNoAdapt = false;

        public ConfigLine() { }

        public LINE LabelOnTop()
        {
            this.labelOnTop = true;
            return (LINE)this;
        }

        public LINE Label(string labelText)
        {
            this.labelText.Value = labelText;
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

        public LINE ControlNoAdapt()
        {
            this.controlNoAdapt = true;
            return (LINE)this;
        }

        public ObservableObject<string?> GetLabelText()
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

        public bool IsControlNoAdapt()
        {
            return this.controlNoAdapt;
        }

        public abstract Control? GetControl();

        public virtual void TrasferToAllConfigurations() { } //To be implemeted inside all childrens 
    }
}
