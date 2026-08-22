using ScintillaNET;
using System.Reflection;
using System.Text.Json;

namespace TiaUtilities.Editors.myScintilla
{
    public class ScintillaTooltip(Scintilla scintilla)
    {
        public class Error
        {
            public required int Line { get; init; }
            public required int Column { get; init; }
            public required int Length { get; init; }
            public required string Message { get; init; }

            public int Position { get; internal set; } = 0;
        }


        public const int ERROR_INDICATOR = 8;
        public const int ERROR_MARKER = 1;

        public Error? CurrentError
        {
            get => _currentError;
            set
            {
                if (_currentError != value)
                {
                    ErrorUpdated(value);
                }

                _currentError = value;
            }
        }
        private Error? _currentError;

        public bool EventDwellStart_Error(int pos)
        {
            if (pos < 0 || _currentError == null || pos < _currentError.Position || pos > _currentError.Position + _currentError.Length)
            {
                return false;
            }

            scintilla.CallTipShow(_currentError.Position, $"{_currentError.Message}");
            return true;
        }

        public void EventDwellStop()
        { // Chiude il tooltip quando il mouse si sposta
            scintilla.CallTipCancel();
        }

        public void EventDwellStart_Docs(int pos)
        {
            if (pos >= 0)
            {
                int endPos = scintilla.WordEndPosition(pos, true);

                var functionName = ScintillaUtils.GetFullFunctionName(scintilla, endPos);
                if (ScintillaDocs.JS_DOCS.TryGetValue(functionName, out var doc))
                {
                    // Mostra il CallTip di Scintilla
                    scintilla.CallTipShow(pos, $"{doc.Syntax}\n{doc.Description}");
                }
            }
        }

        public void EventCharAdded_ShowOnBracket(int addedChar)
        {
            // Se l'utente digita '('
            if (addedChar == '(')
            {
                var pos = scintilla.CurrentPosition - 1;

                var functionName = ScintillaUtils.GetFullFunctionName(scintilla, pos);
                if (!string.IsNullOrEmpty(functionName) && ScintillaDocs.JS_DOCS.TryGetValue(functionName, out var doc))
                {
                    // Mostra la sintassi della funzione subito sotto il cursore
                    scintilla.CallTipShow(pos, $"{doc.Syntax}\n{doc.Description}");
                }
            }
        }

        private void ErrorUpdated(Error? error)
        {
            if (error == null)
            {//Clear Error
                scintilla.MarkerDeleteAll(ERROR_MARKER);

                scintilla.IndicatorCurrent = ERROR_INDICATOR;
                scintilla.IndicatorClearRange(0, scintilla.TextLength);
            }
            else
            {//Mark Error
                scintilla.MarkerDeleteAll(ERROR_MARKER);

                error.Position = scintilla.Lines[error.Line].Position + error.Column;
                scintilla.Lines[error.Line].MarkerAdd(ERROR_MARKER);

                scintilla.IndicatorCurrent = ERROR_INDICATOR;
                scintilla.IndicatorClearRange(0, scintilla.TextLength); //Clear it first in case the error changes. To avoid phantom lines.

                scintilla.IndicatorFillRange(error.Position, error.Length);
            }
        }


    }
}
