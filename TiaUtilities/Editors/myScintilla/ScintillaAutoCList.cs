using ScintillaNET;

namespace TiaUtilities.Editors.myScintilla
{
    public class ScintillaAutoCList(Scintilla scintilla)
    {
        public IEnumerable<string> Suggestions { get; set; } = [];

        public void EventCharAdded_Show()
        {
            // Find the word start
            var currentPos = scintilla.CurrentPosition;
            var wordStartPos = scintilla.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                if (!scintilla.AutoCActive)
                {
                    var autoCompleteList = ScintillaUtils.JS_SUGGESTIONS.Concat(Suggestions).OrderBy(x => x, StringComparer.Ordinal);

                    var autoCompleteStr = String.Join(" ", autoCompleteList);
                    scintilla.AutoCShow(lenEntered, autoCompleteStr);
                }
            }
        }
    }
}
