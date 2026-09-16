using System.Linq.Expressions;
using System.Reflection;
using TiaUtilities.Configuration;
using TiaUtilities.SettingsStep.ControlFactory;
using TiaUtilities.SettingsStep.ControlFactory.Impl;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep
{
    public class SettingsStepDescriptor(string name, string description = "")
    {
        public class StepGroup
        {
            public required string Name { get; init; }
            public required string Description { get; init; }
            public List<SettingsControlFactory> Factories { get; init; } = [];
        }

        public class Binder<Config> where Config : ObservableConfiguration
        {
            private readonly SettingsStepDescriptor stepContext;
            private StepGroup? _lastStepGroup;

            internal Binder(SettingsStepDescriptor stepContext)
            {
                this.stepContext = stepContext;
            }

            public Binder<Config> StartGroup(string name, string description = "")
            {
                this._lastStepGroup = new StepGroup()
                {
                    Name = name,
                    Description = description
                };
                this.stepContext.groups.Add(this._lastStepGroup);
                return this;
            }

            public Binder<Config> AddDivider(SettingsFactoryGeneralOptions? options = default)
            {
                var group = this.StartEmptyGroupIfNeeded();
                group.Factories.Add(new SettingsDividerFactory(options ?? new()));
                return this;
            }

            public Binder<Config> AddText(string text, string description = "", SettingsFactoryGeneralOptions? options = default)
            {
                var group = this.StartEmptyGroupIfNeeded();
                group.Factories.Add(new SettingsTextFactory(text, description, options ?? new()));
                return this;
            }

            public Binder<Config> Add<Prop>(Expression<Func<Config, Prop>> propertyLambda, string name, string description = "", SettingsFactoryGeneralOptions? options = default)
            {
                options ??= new();

                Expression body = propertyLambda.Body;

                //If the system cast a simple type (int, uint, long ...) to another type, it will recover the original operand.
                if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
                {
                    body = unary.Operand;
                }

                if (body is MemberExpression member && member.Member is PropertyInfo propInfo)
                {
                    SettingsConfigurationProperty configurationProperty = new(propInfo);

                    SettingsControlFactory? factory = null;

                    var type = propInfo.PropertyType;
                    if (type == typeof(string))
                    {
                        if(options.StringSpecifiedEditor == SettingsFactoryGeneralOptions.StringCustomEditor.JS)
                        {
                            factory = new SettingsJavascriptFactory(configurationProperty, name, description, options);
                        }
                        else if(options.StringSpecifiedEditor == SettingsFactoryGeneralOptions.StringCustomEditor.JSON)
                        {
                            factory = new SettingsJSONFactory(configurationProperty, name, description, options);
                        }
                        else if(options.StringSelections != null)
                        {
                            factory = new SettingsStringSelectionFactory(configurationProperty, name, description, options) { Selections = options.StringSelections };
                        }
                        else
                        {
                            factory = new SettingsStringFactory(configurationProperty, name, description, options);
                        }
                    }
                    else if (type == typeof(bool))
                    {
                        factory = new SettingsBoolFactory(configurationProperty, name, description, options);
                    }
                    else if (ReflectionUtils.IsUnsignedNumber(type) || ReflectionUtils.IsSignedNumber(type) || ReflectionUtils.IsFloatingNumber(type))
                    {
                        factory = new SettingsNumberFactory(configurationProperty, name, description, options);
                    }
                    else if (typeof(Enum).IsAssignableFrom(type))
                    {
                        factory = new SettingsEnumFactory(configurationProperty, name, description, options) { EnumType = type };
                    }


                    if (factory != null)
                    {
                        var group = this.StartEmptyGroupIfNeeded();
                        group.Factories.Add(factory);
                    }
                }

                return this;
            }

            private StepGroup StartEmptyGroupIfNeeded()
            {
                if (this._lastStepGroup == null)
                {
                    this.StartGroup("");
                }

                return this._lastStepGroup!;
            }

            public SettingsStepDescriptor End() => this.stepContext;
        }

        public string Name { get; init; } = name;
        public string Description { get; init; } = description;

        private readonly List<StepGroup> groups = [];
        private Type? configurationType;

        public IEnumerable<StepGroup> GetGroups() => this.groups;

        public Binder<Config> CreateBinder<Config>() where Config : ObservableConfiguration
        {
            var configType = typeof(Config);
            if (this.configurationType == null)
            {
                this.configurationType = configType;
            }
            else if (this.configurationType != configType)
            {
                throw new InvalidDataException($"One type of ObservableConfiguration [{configurationType.Name}] in this SettingsStepContext");
            }

            return new(this);
        }
    }
}
