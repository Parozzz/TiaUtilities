using TiaUtilities.Generation.Placeholders.Data;

namespace TiaUtilities.Generation.Placeholders.Data
{
    public class StringGenPlaceholderData : IGenPlaceholderData
    {
        public string Value { get; set; }
        public Func<string, string> Function { get; set; }

        public StringGenPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function != null ? Function.Invoke(Value) : Value;
        }
    }

}
