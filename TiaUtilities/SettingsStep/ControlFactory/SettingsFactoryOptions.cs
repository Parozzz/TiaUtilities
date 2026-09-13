namespace TiaUtilities.SettingsStep.ControlFactory
{
    public class SettingsFactoryOptions
    {
        public enum StringCustomEditor { NONE, JS, JSON }

        public int MinWidth { get; set; } = -1;
        public ContentAlignment? TextAlign { get; set; }
        public IEnumerable<string>? StringSelections { get; set; }
        public StringCustomEditor StringSpecifiedEditor { get; set; } = StringCustomEditor.NONE;

        public Func<string, string>? PlaceholdersCallback { get; set; }
        public Action? PropertyChangedCallback { get; set; }
    }
}
