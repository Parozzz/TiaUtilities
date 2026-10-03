using ScintillaNET;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Editors.Json;

namespace TiaUtilities.Editors.T_SQL
{
    public class TSQLEditor
    {
        public event EventHandler TextChanged
        {
            add => this.sqlScintilla.Scintilla.TextChanged += value;
            remove => this.sqlScintilla.Scintilla.TextChanged -= value;
        }

        public string Text
        {
            get => this.sqlScintilla.Text;
            set => this.sqlScintilla.Text = value;
        }

        private readonly TSQLScintilla sqlScintilla;

        public TSQLEditor(Scintilla? scintilla = null)
        {
            this.sqlScintilla = new(scintilla);
        }

        public void InitControl(EditorOptions options)
        {
            this.sqlScintilla.InitControl(options);
        }

        public void ClearUndo() => this.sqlScintilla.Scintilla.EmptyUndoBuffer();

        public Scintilla GetControl() => this.sqlScintilla.Scintilla;
    }
}
