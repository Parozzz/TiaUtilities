using TiaUtilities.Utility;

namespace TiaUtilities.Configuration
{
    public class ObservableObject<T>(T startValue)
    {
        public event ObservableObjectChangedEventHandler<T> Changed = delegate { };

        public T Value { get => _value; set => UpdateValue(value); }
        private T _value = startValue;
        
        public void UpdateValue(T value)
        {
            var oldValue = _value;

            this._value = value;
            if(Utils.AreDifferentObject(this._value, oldValue))
            {
                Changed(this, new(oldValue, this._value));
            }
        }
    }

    public delegate void ObservableObjectChangedEventHandler<T>(object? sender, ObservableObjectChangedEventArgs<T> args);
    public class ObservableObjectChangedEventArgs<T>(T oldValue, T newValue) : EventArgs
    {
        public T OldValue { get; init; } = oldValue;
        public T NewValue { get; init; } = newValue;
    }
}
