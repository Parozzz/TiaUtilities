using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Editors.myScintilla
{
    public class ScintillaBrackets(Scintilla scintilla)
    {
        private int lastDeleted_Start = 0;
        private int lastDeleted_End = 0;
        private string lastDeleted_Text = string.Empty;

        public void EventBeforeDelete_WrapSelection(ModificationFlags source, string text)
        {
            var selectionStart = scintilla.SelectionStart;
            var selectionEnd = scintilla.SelectionEnd;
            if (source.HasFlag(ModificationFlags.User) && selectionStart != selectionEnd)
            {
                lastDeleted_Start = selectionStart;
                lastDeleted_End = selectionEnd;
                lastDeleted_Text = text;
            }
            else
            {
                lastDeleted_Start = 0;
                lastDeleted_End = 0;
                lastDeleted_Text = string.Empty;
            }
        }

        public bool EventCharAdded_WrapSelection(int charAdded, Func<int, ScintillaUtils.BraceType> braceTypeGetter)
        {
            if (string.IsNullOrEmpty(lastDeleted_Text))
            {
                return false;
            }

            var charBraceType = braceTypeGetter(charAdded);
            if (charBraceType == ScintillaUtils.BraceType.Opening)
            {
                var openingBrace = (char)charAdded;
                var closingBrace = ScintillaUtils.GetMatchingJSBrace(openingBrace);

                scintilla.InsertText(scintilla.CurrentPosition, $"{this.lastDeleted_Text}{closingBrace}");
            }

            this.lastDeleted_Start = 0;
            this.lastDeleted_End = 0;
            this.lastDeleted_Text = string.Empty;

            return charBraceType == ScintillaUtils.BraceType.Opening;
        }

        public bool EventCharAdded_IgnoreClosingIfExists(int charAdded, Func<int, ScintillaUtils.BraceType> braceTypeGetter)
        {
            var currentPos = scintilla.CurrentPosition;

            if (braceTypeGetter(charAdded) != ScintillaUtils.BraceType.None &&
                currentPos < scintilla.TextLength &&
                charAdded == scintilla.GetCharAt(currentPos))
            { //If the user types a brace that already exists 1 char after, just move the position and not input the char.
                scintilla.DeleteRange(currentPos - 1, 1);
                scintilla.GotoPosition(currentPos);

                return true;
            }

            return false;
        }

        public void EventCharAdded_InsertMatchedBracket(int charAdded)
        {
            var caretPos = scintilla.CurrentPosition;
            var docStart = caretPos == 1;
            var docEnd = caretPos == scintilla.Text.Length;

            var charPrev = docStart ? scintilla.GetCharAt(caretPos) : scintilla.GetCharAt(caretPos - 2);
            var charNext = scintilla.GetCharAt(caretPos);

            var isCharPrevBlank = charPrev == ' ' || charPrev == '\t' || charPrev == '\n' || charPrev == '\r';
            var isCharNextBlank = charNext == ' ' || charNext == '\t' || charNext == '\n' || charNext == '\r' || docEnd;

            var charPrevOpeningBrace = ScintillaUtils.IsJSBrace(charPrev) == ScintillaUtils.BraceType.Opening;
            var charNextClosingBrace = ScintillaUtils.IsJSBrace(charNext) == ScintillaUtils.BraceType.Closing;

            var isEnclosed = charPrevOpeningBrace && charNextClosingBrace;
            var isSpaceEnclosed = (charPrevOpeningBrace && isCharNextBlank) || (isCharPrevBlank && charNextClosingBrace);

            var isCharOrString = (isCharPrevBlank && isCharNextBlank) || isEnclosed || isSpaceEnclosed;

            var charNextIsStringClosing = charNext == '"' || charNext == '\'';

            switch (charAdded)
            {
                case '(':
                    if (charNextIsStringClosing)
                    {
                        return;
                    }

                    scintilla.InsertText(caretPos, ")");
                    break;
                case '{':
                    if (charNextIsStringClosing)
                    {
                        return;
                    }

                    scintilla.InsertText(caretPos, "}");
                    break;
                case '[':
                    if (charNextIsStringClosing)
                    {
                        return;
                    }

                    scintilla.InsertText(caretPos, "]");
                    break;
                case '"':
                    // 0x22 = "
                    if (charPrev == 0x22 && charNext == 0x22)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                    {
                        scintilla.InsertText(caretPos, "\"");
                    }
                    break;
                case '\'':
                    // 0x27 = '
                    if (charPrev == 0x27 && charNext == 0x27)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                    {
                        scintilla.InsertText(caretPos, "'");
                    }
                    break;
            }
        }
    }
}
