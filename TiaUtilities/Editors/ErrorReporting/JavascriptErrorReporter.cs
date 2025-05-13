using Acornima;
using TiaUtilities.Editors.ErrorReporting;

namespace TiaUtilities.Editors.ErrorReporting
{
    public class JavascriptErrorReporter(Func<string> scriptFunc) : ErrorReporter
    {
        public const int RUN_TIME_MS = 333;

        private volatile string? script;
        private volatile ReportedError? error;

        public override bool Busy { get => this._busy; set => this._busy = value; }
        private volatile bool _busy;

        public void SetBusy()
        {
            _busy = true;
        }

        public bool GetBusy()
        {
            return _busy;
        }

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
                    ParseErrorCollector errorCollector = new();
                    var parsingOptions = new ParserOptions()
                    {
                        AllowReturnOutsideFunction = true,
                        Tolerant = false,
                        CheckPrivateFields = true,
                        ErrorHandler = errorCollector,
                    };

                    var parsedScript = new Parser(parsingOptions).ParseScript(script, strict: true);
                    error = CreateError(errorCollector.Errors.FirstOrDefault());
                }
                catch (ParseErrorException parseEx)
                {
                    error = CreateError(parseEx.Error);
                }
            }

            _busy = false;
        }

        private static ReportedError? CreateError(ParseError? parseError)
        {
            if (parseError == null)
            {
                return null;
            }

            var pos = parseError.Position;
            //Lines need to start from zero!
            return new ReportedError(pos.Line - 1, pos.Column, parseError.Description);
        }

    }
}
