namespace TiaXmlReader.Generation.Placeholders.Data
{
    public class UIntGenPlaceholderData : IGenPlaceholderData
    {
        public uint Value { get; set; }
        public Func<uint, string> Function { get; set; }

        public UIntGenPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function.Invoke(Value);
        }
    }

}
