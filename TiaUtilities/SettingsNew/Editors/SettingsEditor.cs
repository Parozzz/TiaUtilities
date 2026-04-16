using System.ComponentModel;
using TiaUtilities.SettingsNew.FormHelpers;

namespace TiaUtilities.SettingsNew.Editors
{
    public abstract class SettingsEditor
    {
        public static SettingsEditor? ObtainFromValue(SettingsFormValueImpl formValue, bool useContextMenu)
        {
            SettingsEditor editor;
            switch (formValue.Binding.EditorType)
            {
                case SettingsEditorTypeEnum.NONE:
                    return null;
                default:
                case SettingsEditorTypeEnum.STRING:
                case SettingsEditorTypeEnum.UINT:
                case SettingsEditorTypeEnum.INT:
                    editor = new SettingsTextBoxEditor(formValue, useContextMenu);
                    break;
                case SettingsEditorTypeEnum.BOOLEAN:
                    editor = new SettingsCheckBoxEditor(formValue, useContextMenu);
                    break;
                case SettingsEditorTypeEnum.COLOR:
                    editor = new SettingsColorEditor(formValue, useContextMenu);
                    break;
                case SettingsEditorTypeEnum.JSON:
                    editor = new SettingsJSONEditor(formValue, useContextMenu);
                    break;
                case SettingsEditorTypeEnum.JAVASCRIPT:
                    editor = new SettingsJavascriptEditor(formValue, useContextMenu);
                    break;
                case SettingsEditorTypeEnum.ENUM:
                case SettingsEditorTypeEnum.STRING_LIST:
                case SettingsEditorTypeEnum.UNSIGNED_LIST:
                case SettingsEditorTypeEnum.SIGNED_LIST:
                    editor = new SettingsComboBoxEditor(formValue, useContextMenu);
                    break;
            }

            editor.LoadFromConfiguration();
            editor.RegisterPropertyChangedToCurrentConfiguration();
            return editor;
        }

        public SettingsFormValueImpl Value { get; init; }

        private readonly PropertyChangedEventHandler propertyChangedEventHandler;
        private readonly EventHandler disposedEventHandler;

        public SettingsEditor(SettingsFormValueImpl value, bool useContextMenu)
        {
            this.Value = value;
            this.propertyChangedEventHandler = this.PropertyChanged;
            this.disposedEventHandler = this.ControlDisposed;
        }

        public abstract Control GetControl();

        protected abstract Control GetControlForEvents();

        public abstract void LoadFromConfiguration();

        public abstract void SaveToConfiguration();

        public virtual void AddFormCallbacks(Form form) { }

        public void UnRegisterPropertyChangedToCurrentConfiguration()
        {
            this.Value.ConfigurationObject.PropertyChanged -= this.propertyChangedEventHandler;
            this.GetControlForEvents().Disposed -= this.disposedEventHandler;
        }

        public void RegisterPropertyChangedToCurrentConfiguration()
        {
            this.Value.ConfigurationObject.PropertyChanged += this.propertyChangedEventHandler;
            this.GetControlForEvents().Disposed += this.disposedEventHandler;
        }

        //These two are just for event handlers. Defined here because i prefer to defining them in constructor.
        private void PropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (!this.Value.SetInProgress && args.PropertyName == this.Value.PropertyInfo.Name)
            {
                this.LoadFromConfiguration();
            }
        }

        private void ControlDisposed(object? sender, EventArgs args)
        {
            this.Value.ConfigurationObject.PropertyChanged -= this.propertyChangedEventHandler;
        }
    }
}
