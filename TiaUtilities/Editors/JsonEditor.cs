using ScintillaNET;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Editors.myScintilla;

namespace TiaUtilities.Editors
{
    public class JsonEditor
    {
        public string Text
        {
            get => this.jsonScintilla.Scintilla.Text;
            set
            {
                var scintilla = this.jsonScintilla.Scintilla;

                var wasReadOnly = scintilla.ReadOnly;

                scintilla.ReadOnly = false;
                this.jsonScintilla.Scintilla.Text = value;
                scintilla.ReadOnly = wasReadOnly;
            }
        }

        private readonly JsonScintilla jsonScintilla;
        private readonly JsonErrorReporter jsonErrorReporter;

        public JsonEditor(Scintilla? scintilla = null)
        {
            this.jsonScintilla = new(scintilla);
            this.jsonErrorReporter = new(() => this.Text, () => !this.jsonScintilla.Scintilla.CanFocus);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.jsonScintilla.InitControl(borderStyle, backColor, foreColor);

            ErrorReportThread.ISTANCE.AddReporter(this.jsonErrorReporter);
            this.jsonScintilla.Scintilla.Disposed += (sender, args) =>
            {
                ErrorReportThread.ISTANCE.RemoveReporter(this.jsonErrorReporter);
            };

            this.jsonErrorReporter.CompleteEvent += (sender, args) =>
            {
                var errors = args.ErrorList;
                if (errors.Count == 0)
                {
                    this.jsonScintilla.CurrentError = null;
                    return;
                }

                var error = errors[0];
                this.jsonScintilla.CurrentError = new()
                {
                    Line = (int)error.Line,
                    Column = (int)error.Column,
                    Length = 1,
                    Message = error.Description ?? ""
                };
            };
        }

        public void ClearUndo() => this.jsonScintilla.Scintilla.EmptyUndoBuffer();

        public Scintilla GetControl() => this.jsonScintilla.Scintilla;
    }
}
