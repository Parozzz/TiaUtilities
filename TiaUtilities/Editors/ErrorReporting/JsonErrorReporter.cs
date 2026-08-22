using System.Text.Json;

namespace TiaUtilities.Editors.ErrorReporting
{
    public class JsonErrorReporter(Func<string> textCallback, Func<bool> pausedCallback) : ErrorReporter(textCallback, pausedCallback)
    {
        public const int RUN_TIME_MS = 333;

        public override bool Busy { get => this._busy; set => this._busy = value; }

        private volatile string? _scriptText;
        private volatile ReportedError? _error;

        private volatile bool _busy;

        public override void ExecuteSync()
        {
            base.Complete(_error == null ? [] : [_error]); //Report error from previous run!
            this._scriptText = base.Text; //Get the script sync for the next async execution!
        }

        public override void ExecuteAsync()
        {
            _error = null;
            if (_scriptText != null)
            {
                try
                {
                    JsonDocument.Parse(_scriptText);
                }
                catch (JsonException ex)
                {
                    _error = CreateError(ex);
                }
            }

            _busy = false;
        }

        private static ReportedError? CreateError(JsonException jsonException)
        {
            if (jsonException == null)
            {
                return null;
            }

            return new ReportedError(jsonException.LineNumber ?? 0, jsonException.BytePositionInLine ?? 0, jsonException.Message);
        }
    }
}
