using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Editors;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Generation.Configuration.Lines;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public class SettingsJavascriptEditor : SettingsEditor
    {
        private readonly JavascriptEditor editor;
        private FastColoredTextBox Control { get => editor.GetTextBox(); }

        public SettingsJavascriptEditor(SettingsValue value) : base(value)
        {
            this.editor = new JavascriptEditor();
            this.editor.InitControl();

            this.Control.Dock = DockStyle.Fill;
            this.Control.MinimumSize = new Size(0, 400);
            this.Control.Font = SettingsConstants.VALUE_CONTROL_FONT;
            this.Control.Text = "" + value.GetConfigurationValue();

            this.Control.TextChanged += TextChangedEventHandler;

            SettingsUtils.AddContextualMenu(this.Control, value);
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = this.Control.Text;
            this.Value.SetConfigurationValue(text);
        }

        public void RegisterErrorThreadWithForm(ErrorReportThread errorThread, Form form)
        {
            this.editor.RegisterErrorReporter(errorThread);
            form.FormClosing += (sender, args) => this.editor.UnregisterErrorReporter(errorThread);
        }

        public override Control GetControl()
        {
            return this.Control;
        }
    }
}
