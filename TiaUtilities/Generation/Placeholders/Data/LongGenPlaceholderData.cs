using TiaUtilities.Generation.Placeholders.Data;

namespace TiaUtilities.Generation.Placeholders.Data
{
    public class LongGenPlaceholderData : IGenPlaceholderData
    {
        public long Value { get; set; }
        public Func<long, string> Function { get; set; }

        public LongGenPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function.Invoke(Value);
        }
    }

}
