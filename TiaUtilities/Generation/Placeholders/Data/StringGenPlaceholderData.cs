namespace TiaUtilities.Generation.Placeholders.Data
{
    public class StringGenPlaceholderData : IGenPlaceholderData
    {
        public required string Value { get; set; }
        public Func<string, string>? Function { get; set; }

        public string GetSubstitution()
        {
            return this.Function == null ? this.Value : this.Function.Invoke(Value);
        }
    }

}
