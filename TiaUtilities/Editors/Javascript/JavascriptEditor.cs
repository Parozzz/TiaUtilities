using ScintillaNET;
using TiaUtilities.Editors.ErrorReporting;

namespace TiaUtilities.Editors.Javascript
{
    public class JsDoc
    {
        public string Name { get; set; } = string.Empty;
        public string Syntax { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class JavascriptEditor
    {
        public event EventHandler TextChanged
        {
            add => this.jsScintilla.Scintilla.TextChanged += value;
            remove => this.jsScintilla.Scintilla.TextChanged -= value;
        }

        public string Text
        {
            get => this.jsScintilla.Text;
            set => this.jsScintilla.Text = value;
        }

        public IEnumerable<string> Suggestions
        {
            get => this.jsScintilla.Suggestions;
            set => this.jsScintilla.Suggestions = value;
        }

        private readonly JavascriptScintilla jsScintilla;
        private readonly JavascriptErrorReporter jsErrorReporter;

        public JavascriptEditor(Scintilla? scintilla = null)
        {
            this.jsScintilla = scintilla == null ? new() : new(scintilla);
            this.jsErrorReporter = new(() => this.Text, () => !this.jsScintilla.Scintilla.CanFocus);

            //this.visualErrorHandler = new(this.textBox, this.jsErrorReporter);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.jsScintilla.InitControl(borderStyle, backColor, foreColor);

            ErrorReportThread.ISTANCE.AddReporter(this.jsErrorReporter);
            this.jsScintilla.Scintilla.Disposed += (sender, args) =>
            {
                ErrorReportThread.ISTANCE.RemoveReporter(this.jsErrorReporter);
            };

            this.jsErrorReporter.CompleteEvent += (sender, args) =>
            {
                var errors = args.ErrorList;
                if (errors.Count == 0)
                {
                    this.jsScintilla.CurrentError = null;
                    return;
                }

                var error = errors[0];
                this.jsScintilla.CurrentError = new()
                {
                    Line = (int)error.Line,
                    Column = (int)error.Column,
                    Length = 1,
                    Message = error.Description ?? ""
                };
            };
        }

        public void ClearUndo() => this.jsScintilla.Scintilla.EmptyUndoBuffer();

        public void InsertText(string text, int position = -1)
        {
            if (position < 0)
            {
                position = this.jsScintilla.Scintilla.CurrentPosition;
            }

            this.jsScintilla.Scintilla.InsertText(position, text);
        }

        public void FocusControl() => this.jsScintilla.Scintilla.Focus();

        public Scintilla GetControl() => this.jsScintilla.Scintilla;

    }
}
