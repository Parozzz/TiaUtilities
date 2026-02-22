using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public abstract class SettingsEditor
    {
        public static SettingsEditor? ObtainFromValue(Form form, SettingsFormValueImpl value)
        {
            switch (value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.NONE:
                    return null;
                default:
                case SettingsEditorTypeEnum.STRING:
                case SettingsEditorTypeEnum.UINT:
                case SettingsEditorTypeEnum.INT:
                    return new SettingsTextBoxEditor(value);
                case SettingsEditorTypeEnum.BOOLEAN:
                    return new SettingsCheckBoxEditor(value);
                case SettingsEditorTypeEnum.COLOR:
                    return new SettingsColorEditor(value);
                case SettingsEditorTypeEnum.JSON:
                    return new SettingsJSONEditor(value);
                case SettingsEditorTypeEnum.JAVASCRIPT:
                    var jsEditor = new SettingsJavascriptEditor(value);
                    jsEditor.RegisterErrorThreadWithForm(MainForm.JavascriptErrorThread, form);
                    return jsEditor;
                case SettingsEditorTypeEnum.ENUM:
                case SettingsEditorTypeEnum.LIST:
                    return new SettingsComboBoxEditor(value);
            }
        }

        public SettingsFormValueImpl Value { get; init; }

        public SettingsEditor(SettingsFormValueImpl value)
        {
            this.Value = value;
        }

        public abstract Control GetControl();

        public abstract void LoadFromConfiguration();

        public abstract void SaveToConfiguration();

        protected void RegisterPropertyChanged(Control control)
        {
            void propertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (!this.Value.SetInProgress && args.PropertyName == this.Value.PropertyInfo.Name)
                {
                    this.LoadFromConfiguration();
                }
            }

            this.Value.ConfigurationObject.PropertyChanged += propertyChanged;
            control.Disposed += (sender, args) => this.Value.ConfigurationObject.PropertyChanged -= propertyChanged;
        }
    }
}
