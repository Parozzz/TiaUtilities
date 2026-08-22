using DocumentFormat.OpenXml.Math;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Editors.myScintilla
{
    public class ScintillaHighlighter(Scintilla scintilla)
    {
        public const int INDICATOR = 9;

        private int lastCaretPos = 0;

        public void EventUpdateUI_Brackets(UpdateChange change, Func<int, ScintillaUtils.BraceType> braceTypeGetter)
        {
            if (!change.HasFlag(UpdateChange.Selection))
            {
                return;
            }

            // Has the caret changed position?
            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                var caretPosChar = scintilla.GetCharAt(caretPos);
                var prevCaretPosChar = scintilla.GetCharAt(caretPos - 1);

                // Is there a brace to the left or right?
                if (caretPos > 0 && braceTypeGetter(prevCaretPosChar) != ScintillaUtils.BraceType.None)
                {
                    bracePos1 = (caretPos - 1);
                }
                else if (braceTypeGetter(caretPosChar) != ScintillaUtils.BraceType.None)
                {
                    bracePos1 = caretPos;
                }

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                    {
                        scintilla.BraceBadLight(bracePos1);
                    }
                    else
                    {
                        scintilla.BraceHighlight(bracePos1, bracePos2);
                    }
                }
                else
                {
                    // Turn off brace matching
                    scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                }
            }
        }

        public void EventUpdateUI_SameSelectionWords(UpdateChange change)
        {
            if (!change.HasFlag(UpdateChange.Selection))
            {
                return;
            }

            scintilla.IndicatorCurrent = INDICATOR;
            scintilla.IndicatorClearRange(0, scintilla.TextLength);

            string selectedText = scintilla.SelectedText.Trim();
            if (selectedText.Length < 2 ||
                selectedText.Contains('\n') ||
                selectedText.Contains('\r') ||
                selectedText.Contains(' '))
            {//Only following letters
                return;
            }

            scintilla.SearchFlags = SearchFlags.WholeWord | SearchFlags.MatchCase; //(Parola intera e Case - Sensitive)
            scintilla.TargetStart = 0;
            scintilla.TargetEnd = scintilla.TextLength;
            
            while (scintilla.SearchInTarget(selectedText) != -1)
            {
                // Evidenzia l'occorrenza trovata
                scintilla.IndicatorFillRange(scintilla.TargetStart, scintilla.TargetEnd - scintilla.TargetStart);

                // Avanza con la ricerca
                scintilla.TargetStart = scintilla.TargetEnd; //Update target start from last end. Recursive.
                scintilla.TargetEnd = scintilla.TextLength; 
            }
        }

    }
}
