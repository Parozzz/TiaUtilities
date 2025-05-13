using System.Linq.Expressions;
using TiaUtilities.CustomControls;
using TiaUtilities.Generation.Configuration.Utility;

namespace TiaUtilities.Generation.Configuration.Lines
{
    public class ConfigCheckBoxLine : ConfigLine<ConfigCheckBoxLine>
    {
        private readonly IConfigGroup configGroup;
        private readonly RJToggleButton control;

        private Action<bool>? checkedChangedAction;
        private Action? transferToOtherBoolAction;

        public ConfigCheckBoxLine(IConfigGroup configGroup)
        {
            this.configGroup = configGroup;
            control = new RJToggleButton
            {
                FlatStyle = FlatStyle.Flat,
                Text = "",
                AutoSize = true,
                BorderColor = SystemColors.ControlDark,
                OnBackColor = Color.Transparent,
                OffBackColor = Color.Transparent,
                OnToggleColor = Color.DarkGreen,
                OffToggleColor = Color.PaleVioletRed,
            };
            control.CheckedChanged += CheckedChangedEventHandler;
        }

        private void CheckedChangedEventHandler(object? sender, EventArgs args)
        {
            checkedChangedAction?.Invoke(control.Checked);
        }

        public ConfigCheckBoxLine Value(bool value)
        {
            control.Checked = value;
            return this;
        }
        /*
        public ConfigCheckBoxLine CheckedChanged(Action<bool> action)
        {
            checkedChangedAction = action;
            return this;
        }
        */
        public ConfigCheckBoxLine BindChecked(Expression<Func<bool>> propertyExpression)
        {
            var propertyInfo = ConfigLineUtils.ValidateBindExpression(this.configGroup, propertyExpression.Body, out object configuration, out IEnumerable<object> otherConfigurations);

            this.control.Checked = propertyExpression.Compile().Invoke();
            this.checkedChangedAction = boolValue => propertyInfo.SetValue(configuration, boolValue);
            this.transferToOtherBoolAction = () =>
            {
                var boolValue = this.control.Checked;
                foreach (var otherConfig in otherConfigurations)
                {
                    propertyInfo.SetValue(otherConfig, boolValue);
                }
            };
            return this;
        }
        public override void TrasferToAllConfigurations()
        {
            transferToOtherBoolAction?.Invoke();
        }

        public override Control GetControl()
        {
            return control;
        }
    }
}
