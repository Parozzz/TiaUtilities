using Acornima;

namespace TiaUtilities.Editors.ErrorReporting
{
    public class JavascriptErrorReporter(Func<string> textCallback, Func<bool> pausedCallback) : ErrorReporter(textCallback, pausedCallback)
    {
        public const int RUN_TIME_MS = 333;

        public override bool Busy { get => this._busy; set => this._busy = value; }

        private volatile string? _scriptText;
        private volatile ReportedError? _error;

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
                    Parser parser = new(new()
                    {//ErrorHandler does not work. Only outputs 1 error at the time
                        AllowReturnOutsideFunction = true,
                        Tolerant = false,
                        CheckPrivateFields = true
                    });

                    parser.ParseScript(_scriptText, strict: true);
                }
                catch (ParseErrorException parseEx)
                {
                    _error = CreateError(parseEx.Error);
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
