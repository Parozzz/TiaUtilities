using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TiaUtilities.Utility;

namespace TiaUtilities.Configuration
{
    public abstract class ObservableConfiguration() : ICleanable, INotifyPropertyChanged
    {
        private class ConfigurationObject(ObservableConfiguration configuration, string propertyName, object startValue)
        {
            public string PropertyName { get; init; } = propertyName;
            public Type Type { get; init; } = startValue.GetType();

            public object Value { get => _value; set => UpdateValue(value); }
            private object _value = startValue;

            public T GetAs<T>()
            {
                if (Value is not T t || Type != typeof(T))
                {
                    throw new ArgumentException($"Cannot cast ConfigurationObject Value from {Type.FullName} to {typeof(T).FullName}");
                }

                return t;
            }

            public void UpdateValue(object value)
            {
                if (_value is ObservableConfiguration oldObservableConfiguration)
                {
                    configuration.subConfigurationList.Remove(oldObservableConfiguration);
                }

                if (value == null || value.GetType() != Type)
                {
                    return;
                }

                if (value is ObservableConfiguration newObservableConfiguration)
                {
                    configuration.subConfigurationList.Add(newObservableConfiguration);
                }

                var differentObjects = Utils.AreDifferentObject(this._value, value);
                this._value = value;
                if (differentObjects)
                {
                    configuration.ConfigurationObjectChanged(this.PropertyName);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private readonly Dictionary<string, ConfigurationObject> objectDict = [];
        private readonly Dictionary<string, List<Action>> objectChangedDict = [];
        private readonly List<ObservableConfiguration> subConfigurationList = [];

        private bool dirty; //When initializing object, the dirty state might be set!

        public virtual bool IsDirty() => this.dirty || this.subConfigurationList.Any(x => x.IsDirty());
        public virtual void Wash()
        {
            this.dirty = false;
            this.subConfigurationList.ForEach(x => x.Wash());
        }

        public Action Subscribe<T>(Expression<Func<T>> property, Action<T> valueChagedAction)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo ?? throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            var propertyName = propertyInfo.Name;

            void action()
            {
                if (objectDict.TryGetValue(propertyName, out var obj))
                {
                    var value = obj.GetAs<T>();
                    valueChagedAction.Invoke(value);
                }
            }

            if (!objectChangedDict.TryGetValue(propertyName, out var actionList))
            {
                actionList = [];
                objectChangedDict.Add(propertyName, actionList);
            }

            actionList.Add(action);
            return action;
        }

        public void Unsubscribe<T>(Expression<Func<T>> property, Action action)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo ?? throw new ArgumentException("The lambda expression 'property' should point to a valid Property");

            var propertyName = propertyInfo.Name;
            if (objectChangedDict.TryGetValue(propertyName, out var actionList))
            {
                actionList.Remove(action);
            }
        }

        private void ConfigurationObjectChanged(string propertyName)
        {
            this.dirty = true;
            PropertyChanged(this, new(propertyName));
            if (objectChangedDict.TryGetValue(propertyName, out var actionList))
            {
                foreach (var action in actionList)
                {
                    action.Invoke();
                }
            }
        }

        private void InitObject(string key, object startValue)
        {
            if (startValue == null || objectDict.ContainsKey(key))
            {
                return;
            }

            ConfigurationObject obj = new(this, key, startValue);
            obj.UpdateValue(startValue);
            objectDict.Add(key, obj);
        }

        public void Set(object value, [CallerMemberName] string key = "")
        {
            if (objectDict.TryGetValue(key, out ConfigurationObject? obj))
            {
                obj.Value = value;
            }
            else
            {
                this.InitObject(key, value);
            }
        }

        public object? Get([CallerMemberName] string key = "")
        {
            return objectDict.TryGetValue(key, out var obj) ? obj.Value : null;
        }

        public T GetAs<T>([CallerMemberName] string key = "")
        {
            if (!objectDict.TryGetValue(key, out var obj))
            {
                throw new ArgumentException($"{key} does not exists inside {this.GetType().FullName}");
            }

            return obj.GetAs<T>();
        }
    }
}
