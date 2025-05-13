using FastColoredTextBoxNS;
using TiaUtilities.Editors;
using TiaUtilities.Editors.ErrorReporting;

namespace TiaUtilities.Editors
{
    public class JsonEditor
    {
        private static readonly MarkerStyle HIGHLIGHTED_BRACKET_STYLE = new(new SolidBrush(Color.FromArgb(150, Color.LightGreen)));

        private readonly FastColoredTextBox textBox;
        private readonly JsonErrorReporter jsonErrorReporter;
        private readonly FCTBErrorVisualizer visualErrorHandler;

        public JsonEditor(FastColoredTextBox? fctb = null)
        {
            this.textBox = fctb ?? new();
            this.jsonErrorReporter = new(this.ErrorReportingGetScript);
            this.visualErrorHandler = new(this.textBox, this.jsonErrorReporter);
        }

        public FastColoredTextBox GetTextBox()
        {
            return textBox;
        }

        public void InitControl()
        {
            #region FCTB_SETUP
            // == BRACKETS ==
            this.textBox.Language = Language.JSON;
            this.textBox.ReadOnly = false;
            // == INDENTATION ==
            this.textBox.AutoIndent = true;
            this.textBox.AutoIndentExistingLines = true;
            this.textBox.AutoIndentChars = true;
            this.textBox.TabLength = 4;
            // == LINE NUMBERS ==
            this.textBox.ShowLineNumbers = false;
            this.textBox.LineNumberStartValue = 0;
            // == CARET ==
            this.textBox.CaretVisible = true;
            this.textBox.CaretBlinking = true;
            this.textBox.ShowCaretWhenInactive = true;
            this.textBox.WideCaret = false;

            this.textBox.CharHeight = 16; //Default 14
            this.textBox.LineInterval = 4; //Default 0

            this.textBox.AcceptsTab = true;
            this.textBox.AcceptsReturn = true;
            this.textBox.ShowFoldingLines = true;
            #endregion

            this.visualErrorHandler.Init();
        }

        public void RegisterErrorReporter(ErrorReportThread errorThread)
        {
            errorThread.AddReporter(this.jsonErrorReporter);
        }

        public void UnregisterErrorReporter(ErrorReportThread errorThread)
        {
            errorThread.RemoveReporter(this.jsonErrorReporter);
        }

        private string ErrorReportingGetScript()
        {
            return this.textBox.Text;
        }
    }
}
