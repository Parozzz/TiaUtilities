using TiaUtilities.Editors.myScintilla;

namespace TiaUtilities.JSScript
{
    public record JsScriptConsoleLogEventArgs(DateTime DateTime, LoggerScintilla.LogLevel LogLevel, string Message);

    public delegate void JsScriptConsoleLogEvent(object? sender, JsScriptConsoleLogEventArgs args);

    public class JsScriptConsole()
    {
        public event JsScriptConsoleLogEvent LogEvent = delegate { };

        public void log(string message) {
            var dateTime = DateTime.Now;
            var level = LoggerScintilla.LogLevel.INFO;

            LogEvent(this, new(dateTime, level, message));
        }

        public void warn(string message)
        {
            var dateTime = DateTime.Now;
            var level = LoggerScintilla.LogLevel.WARN;

            LogEvent(this, new(dateTime, level, message));
        }

        public void error(string message)
        {
            var dateTime = DateTime.Now;
            var level = LoggerScintilla.LogLevel.ERROR;

            LogEvent(this, new(dateTime, level, message));
        }
    }
}
