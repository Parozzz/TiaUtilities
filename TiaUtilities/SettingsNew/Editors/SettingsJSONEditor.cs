using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Editors;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public class SettingsJSONEditor : SettingsEditor
    {
        private readonly JsonEditor editor;
        private FastColoredTextBox Control { get => editor.GetTextBox(); }

        public SettingsJSONEditor(SettingsFormValueImpl value) : base(value)
        {
            this.editor = new JsonEditor();
            this.editor.InitControl();

            this.Control.Dock = DockStyle.Fill;
            this.Control.MinimumSize = new Size(0, 500);
            this.Control.Font = SettingsConstants.VALUE_CONTROL_FONT;
            this.Control.TextChanged += (sender, args) => this.SaveToConfiguration();

            var _ = SettingsUtils.AddContextualMenu(this.Control, value);

            base.RegisterPropertyChanged(this.Control);
            this.LoadFromConfiguration();
        }

        public override Control GetControl()
        {
            return this.Control;
        }

        public override void LoadFromConfiguration()
        {
            this.Control.Text = "" + this.Value.GetConfigurationValue();
        }

        public override void SaveToConfiguration()
        {
            this.Value.SetConfigurationValue(this.Control.Text);
        }
    }
}
