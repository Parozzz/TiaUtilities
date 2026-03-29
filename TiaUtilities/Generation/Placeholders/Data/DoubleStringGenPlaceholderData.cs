namespace TiaUtilities.Generation.Placeholders.Data
{
    public class DoubleStringGenPlaceholderData : IGenPlaceholderData
    {
        public required string? FirstValue { get; set; }
        public required string SecondValue { get; set; }
        public Func<string, string>? Function { get; set; }

        public string GetSubstitution()
        {
            var value = string.IsNullOrEmpty(this.FirstValue) ? this.SecondValue : this.FirstValue;
            return this.Function == null ? value : this.Function.Invoke(value);
        }
    }

}
