using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsNew;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsStringFactory(SettingsConfigurationProperty? configurationProperty, string name, string description, SettingsFactoryGeneralOptions options) 
        : SettingsControlFactory(configurationProperty, name, description, options)
    {
        private const string SAMPLE_TEXT = "AaBbCcDdEeFfGgHhIiLlJjKkMmNnOoPpQqRrSsTtUuVvZz!?";

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions, () => $"{this.ConfigurationProperty?.GetFrom(configuration)}");
            
            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            var startText = startValue?.ToString() ?? "";

            var textBox = SettingsControls.GetTextBox(SAMPLE_TEXT, startText, this.GeneralOptions, createOptions);

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

                this.GeneralOptions?.PropertyChangedCallback?.Invoke(); //This way also handles property changed from the control!
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
