namespace TiaUtilities.Generation.Placeholders.Data
{
    public class LongGenPlaceholderData : IGenPlaceholderData
    {
        public required long Value { get; set; }
        public Func<long, string>? Function { get; set; }

        public string GetSubstitution()
        {
            return this.Function == null ? this.Value.ToString() : this.Function.Invoke(Value);
        }
    }

}
