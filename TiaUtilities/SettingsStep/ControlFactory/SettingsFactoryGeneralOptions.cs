namespace TiaUtilities.SettingsStep.ControlFactory
{
    public class SettingsFactoryGeneralOptions
    {
        public enum StringCustomEditor { NONE, JS, JSON, TSQL }

        public int MinWidth { get; set; } = -1;
        public ContentAlignment? TextAlign { get; set; }
        public IEnumerable<string>? StringSelections { get; set; }
        public StringCustomEditor StringSpecifiedEditor { get; set; } = StringCustomEditor.NONE;

        public bool SupportPlaceholders { get; set; } = false;
        public Action? PropertyChangedCallback { get; set; }
    }
}
