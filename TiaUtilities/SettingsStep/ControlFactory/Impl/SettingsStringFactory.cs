using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsStringFactory : SettingsControlFactory
    {
        private const string SAMPLE_TEXT = "AaBbCcDdEeFfGgHhIiLlJjKkMmNnOoPpQqRrSsTtUuVvZz!?";

        public SettingsStringFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryOptions? options = null) :
            base(configurationProperty, name, description, options)
        {
        }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.Options, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");
            
            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            var startText = startValue?.ToString() ?? "";

            var textBox = SettingsControls.GetTextBox(SAMPLE_TEXT, startText, this.Options);

            var setInProgress = false;
            textBox.TextChanged += (sender, args) =>
            {
                setInProgress = true;

                var text = textBox.Text;
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
                textBox.Text = value == null ? "" : value.ToString();
                return true;
            }

            return (nameLabel, textBox, propertyChangedPredicate);
        }
    }
}
