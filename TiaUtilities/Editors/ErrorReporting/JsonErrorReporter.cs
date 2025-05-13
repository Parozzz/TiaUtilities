using Acornima;
using System.Text.Json;
using TiaUtilities.Javascript.ErrorReporting;

namespace TiaUtilities.Javascript.ErrorReporters
{
    public class JsonErrorReporter(Func<string> scriptFunc) : ErrorReporter
    {
        public const int RUN_TIME_MS = 333;

        private volatile string? script;
        private volatile ReportedError? error;

        public override bool Busy { get => this._busy; set => this._busy = value; }
        private volatile bool _busy;

        public override void ExecuteSync()
        {
            base.Complete(error == null ? [] : [error]); //Report error from previous run!
            this.script = scriptFunc(); //Get the script sync for the next async execution!
        }

        public override void ExecuteAsync()
        {
            error = null;
            if (script != null)
            {
                try
                {
                    JsonDocument.Parse(script);
                }
                catch (JsonException ex)
                {
                    error = CreateError(ex);
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
