using System.ComponentModel;
using TiaUtilities.Configuration;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.ControlFactory.Impl
{
    public class SettingsNumberFactory : SettingsControlFactory
    {
        private const string SAMPLE_TEXT = "123456789+-,.";

        public enum NumberType { UNSIGNED, SIGNED, FLOAT }

        private readonly NumberType numberType;

        public SettingsNumberFactory(SettingsConfigurationProperty configurationProperty, string name, string description, SettingsFactoryGeneralOptions options)
            : base(configurationProperty, name, description, options)
        {
            Validate.NotNull(this.ConfigurationProperty);

            if (ReflectionUtils.IsUnsignedNumber(this.ConfigurationProperty.PropertyType))
            {
                this.numberType = NumberType.UNSIGNED;
            }
            else if (ReflectionUtils.IsSignedNumber(this.ConfigurationProperty.PropertyType))
            {
                this.numberType = NumberType.SIGNED;
            }
            else if (ReflectionUtils.IsFloatingNumber(this.ConfigurationProperty.PropertyType))
            {
                this.numberType = NumberType.FLOAT;
            }
            else
            {
                throw new InvalidDataException($"Configuration property is not a number: {configurationProperty}");
            }
        }

        public override (Label, Control, Predicate<PropertyChangedEventArgs>) Create(ObservableConfiguration configuration, SettingsFactoryCreateOptions createOptions)
        {
            Validate.NotNull(this.ConfigurationProperty);

            var nameLabel = SettingsControls.GetNameLabel(this.Name, this.Description, this.GeneralOptions, createOptions);

            var startValue = this.ConfigurationProperty.GetFrom(configuration);
            var startText = startValue?.ToString() ?? "";

            var textBox = SettingsControls.GetTextBox(SAMPLE_TEXT, startText, this.GeneralOptions, createOptions);

            switch (this.numberType)
            {
                case NumberType.UNSIGNED:
                    textBox.KeyPress += ControlUtils.UnsignedNumberKeyPressEventHandler;
                    break;
                case NumberType.SIGNED:
                    textBox.KeyPress += ControlUtils.SignedNumberKeyPressEventHandler;
                    break;
                case NumberType.FLOAT:
                    textBox.KeyPress += ControlUtils.FloatingNumberKeyPressEventHandler;
                    break;
            }

            var setInProgress = false;
            textBox.TextChanged += (sender, args) =>
            {
                bool tryParseOK = false;

                var text = textBox.Text;
                switch (this.numberType)
                {
                    case NumberType.UNSIGNED:
                        tryParseOK = this.ConfigurationProperty.TryParseUnsigned(text, out ulong ulongValue);
                        if (tryParseOK)
                        {
                            setInProgress = true;
                            this.ConfigurationProperty.SetTo(configuration, ulongValue);
                            setInProgress = false;
                        }
                        break;
                    case NumberType.SIGNED:
                        tryParseOK = this.ConfigurationProperty.TryParseSigned(text, out long longValue);
                        if (tryParseOK)
                        {
                            setInProgress = true;
                            this.ConfigurationProperty.SetTo(configuration, longValue);
                            setInProgress = false;
                        }
                        break;
                    case NumberType.FLOAT:
                        tryParseOK = this.ConfigurationProperty.TryParseFloating(text, out double doubleValue);
                        if (tryParseOK)
                        {
                            setInProgress = true;
                            this.ConfigurationProperty.SetTo(configuration, doubleValue);
                            setInProgress = false;
                        }
                        break;
                }

                textBox.ForeColor = tryParseOK ? Form.DefaultForeColor : Color.DarkRed;
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
