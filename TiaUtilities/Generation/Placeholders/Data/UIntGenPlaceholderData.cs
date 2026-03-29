namespace TiaUtilities.Generation.Placeholders.Data
{
    public class UIntGenPlaceholderData : IGenPlaceholderData
    {
        public required uint Value { get; set; }
        public Func<uint, string>? Function { get; set; }

        public string GetSubstitution()
        {
            return Function == null ? this.Value.ToString() : Function.Invoke(Value);
        }
    }

}
