using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.SettingsNew.Editors
{
    public abstract class SettingsEditor
    {
        public static SettingsEditor ObtainFromValue(Form form, SettingsValue value)
        {
            /*
             *         STRING,
        INT,
        UINT,
        BOOLEAN,
        JSON,
        JAVASCRIPT,
        COLOR,
        ENUM,
             */

            switch (value.ValueBinding.EditorType)
            {
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
                    return new SettingsComboBoxEditor(value);
            }
        }

        public SettingsValue Value { get; init; }
        public SettingsEditor(SettingsValue value)
        {
            this.Value = value;
        }

        public abstract Control GetControl();
    }
}
