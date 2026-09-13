using System.ComponentModel;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsDividerFactory : SettingsControlFactory
    {
        public SettingsDividerFactory(SettingsFactoryOptions? options)
            : base(null, "", "", options)
        {
            this.Options = options;
        }

        public override (Label, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration)
        {
            var dividerLabel = SettingsControls.GetDividerLabel(Color.Black, this.Options);
            return (dividerLabel, null, null);
        }
    }
}
