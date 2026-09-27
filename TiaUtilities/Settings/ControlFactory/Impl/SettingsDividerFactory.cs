using System.ComponentModel;
using TiaUtilities.Configuration;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsDividerFactory(SettingsFactoryGeneralOptions options)
        : SettingsControlFactory(null, "", "", options)
    {
        public override (Label, Control?, Predicate<PropertyChangedEventArgs>?) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            var dividerLabel = SettingsControls.GetDividerLabel(Color.Black);
            return (dividerLabel, null, null);
        }
    }
}
