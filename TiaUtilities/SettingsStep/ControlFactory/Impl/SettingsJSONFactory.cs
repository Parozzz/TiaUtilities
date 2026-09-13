using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.Editors.Javascript;
using TiaUtilities.Editors.Json;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsJSONFactory : SettingsControlFactory
    {
        public SettingsJSONFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryOptions? options = null) :
            base(configurationProperty, name, description, options)
        {
        }

        public override (Label?, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.Options, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");

            var editor = SettingsControls.GetJSONEditor();

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            editor.Text = $"{startValue}";

            var setInProgress = false;
            editor.TextChanged += (sender, args) =>
            {
                setInProgress = true;

                var text = editor.Text;
                this.ConfigurationProperty.SetTo(configuration, text);

                setInProgress = false;
            };

            bool propertyChangedPredicate(PropertyChangedEventArgs args)
            {
                if (!this.ConfigurationProperty.IsPropertyChanged(args))
                {
                    return false;
                }

                this.Options?.PropertyChangedCallback?.Invoke(); //This way also handles property changed from the control!
                if (setInProgress)
                {
                    return false;
                }

                var value = this.ConfigurationProperty.GetFrom(configuration);
                editor.Text = $"{value}";
                return true;
            }

            return (null, editor.GetControl(), propertyChangedPredicate);
        }
    }
}
