using FastColoredTextBoxNS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Linq.Expressions;
using System.Text.Json;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Configuration;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigJSONLine : ConfigLine<ConfigJSONLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly FastColoredTextBox control;

        private Action<string>? textChangedAction;
        private Action? transferToOtherTextAction;

        public ConfigJSONLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;
            this.control = new FastColoredTextBox
            {
                Language = Language.JSON,
                ReadOnly = false,
                // == INDENTATION ==
                AutoIndent = true,
                AutoIndentExistingLines = true,
                AutoIndentChars = true,
                TabLength = 4,
                // == LINE NUMBERS ==
                ShowLineNumbers = false,
                LineNumberStartValue = 0,
                // == CARET ==
                CaretVisible = true,
                CaretBlinking = true,
                ShowCaretWhenInactive = true,
                WideCaret = false,

                CharHeight = 16, //Default 14
                LineInterval = 4, //Default 0

                AcceptsTab = true,
                AcceptsReturn = true,
                ShowFoldingLines = true,
            };


            control.TextChanged += TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object? sender, EventArgs args)
        {
            var text = control.Text;
            textChangedAction?.Invoke(text);
        }

        public ConfigJSONLine Readonly()
        {
            control.ReadOnly = true;
            control.BackColor = SystemColors.Control;
            return this;
        }

        public override ConfigJSONLine ControlText(IConvertible? value)
        {
            base.ControlText(value);
            control.ClearUndo(); //Avoid beeing able to undo after the text has been added.
            return this;
        }

        public ConfigJSONLine TextChanged(Action<string> action)
        {
            textChangedAction = action;
            return this;
        }

        public ConfigJSONLine BindText(Expression<Func<string>> propertyExpression, bool nullable = false)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.ControlText(propertyExpression.Compile().Invoke());
            this.textChangedAction = str => propertyInfo.SetValue(configuration, nullable ? str : (str ?? ""));
            this.transferToOtherTextAction = () =>
            {
                var str = this.control.Text;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, str);
                }
            };
            return this;
        }

        public override void TrasferToAllConfigurations()
        {
            transferToOtherTextAction?.Invoke();
        }

        public ConfigJSONLine RegisterLinter(Form form)
        {
            System.Windows.Forms.Timer timer = new() { Interval = 500 };
            timer.Tick += (sender, args) =>
            {
                try
                {
                    var jsonDocument = JsonDocument.Parse(this.control.Text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.GetType().Name}, {ex.Message}");
                }
            };
            timer.Start();
            form.FormClosing += (sender, args) => timer.Stop();
            return this;
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
