using System.ComponentModel;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public abstract class SettingsEditor
    {
        public static SettingsEditor? ObtainFromValue(SettingsFormValueImpl value)
        {
            SettingsEditor? editor = null;
            switch (value.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.NONE:
                    break;
                default:
                case SettingsEditorTypeEnum.STRING:
                case SettingsEditorTypeEnum.UINT:
                case SettingsEditorTypeEnum.INT:
                    editor = new SettingsTextBoxEditor(value);
                    break;
                case SettingsEditorTypeEnum.BOOLEAN:
                    editor = new SettingsCheckBoxEditor(value);
                    break;
                case SettingsEditorTypeEnum.COLOR:
                    editor = new SettingsColorEditor(value);
                    break;
                case SettingsEditorTypeEnum.JSON:
                    editor = new SettingsJSONEditor(value);
                    break;
                case SettingsEditorTypeEnum.JAVASCRIPT:
                    editor = new SettingsJavascriptEditor(value);
                    break;
                case SettingsEditorTypeEnum.ENUM:
                case SettingsEditorTypeEnum.STRING_LIST:
                case SettingsEditorTypeEnum.UNSIGNED_LIST:
                case SettingsEditorTypeEnum.SIGNED_LIST:
                    editor = new SettingsComboBoxEditor(value);
                    break;
            }

            return editor;
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

        public virtual void AddFormCallbacks(Form form) { }
    }
}
