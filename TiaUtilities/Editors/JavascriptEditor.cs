using FastColoredTextBoxNS;
using TiaUtilities.Javascript.ErrorReporters;
using TiaUtilities.Javascript.ErrorReporting;

namespace TiaUtilities.Javascript
{
    public class JavascriptEditor
    {
        private static readonly MarkerStyle HIGHLIGHTED_BRACKET_STYLE = new(new SolidBrush(Color.FromArgb(150, Color.LightGreen)));

        private readonly FastColoredTextBox fctb;
        private readonly JavascriptErrorReporter jsErrorReporter;
        private readonly FCTBErrorVisualizer visualErrorHandler;

        public JavascriptEditor(FastColoredTextBox? fctb = null)
        {
            this.fctb = fctb ?? new();
            this.jsErrorReporter = new(this.ErrorReportingGetScript);
            this.visualErrorHandler = new(this.fctb, this.jsErrorReporter);
        }

        public FastColoredTextBox GetTextBox()
        {
            return fctb;
        }

        public void InitControl()
        {
            #region FCTB_SETUP
            // == BRACKETS ==
            fctb.Language = Language.JS;

            fctb.AutoCompleteBrackets = true;
            fctb.AutoCompleteBracketsList = ['(', ')', '{', '}', '\"', '\"', '\'', '\''];
            fctb.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            fctb.BracketsStyle = HIGHLIGHTED_BRACKET_STYLE;
            fctb.BracketsStyle2 = HIGHLIGHTED_BRACKET_STYLE;
            fctb.LeftBracket = '(';
            fctb.RightBracket = ')';
            fctb.LeftBracket2 = '{';
            fctb.RightBracket2 = '}';
            // == INDENTATION ==
            fctb.AutoIndent = true;
            fctb.AutoIndentExistingLines = true;
            fctb.AutoIndentChars = false;
            fctb.TabLength = 4;
            // == LINE NUMBERS ==
            fctb.ShowLineNumbers = true;
            fctb.LineNumberStartValue = 1;
            fctb.LineNumberColor = Color.DarkGreen;
            // == CARET ==
            fctb.CaretVisible = true;
            fctb.CaretBlinking = true;
            fctb.ShowCaretWhenInactive = true;
            fctb.WideCaret = false;

            fctb.CharHeight = 16; //Default 14
            fctb.LineInterval = 4; //Default 0

            fctb.AcceptsTab = true;
            fctb.AcceptsReturn = true;
            fctb.ShowFoldingLines = false;

            fctb.HighlightingRangeType = HighlightingRangeType.AllTextRange;
            #endregion

            this.visualErrorHandler.Init();

            fctb.ToolTipNeeded += (sender, args) =>
            {
                //This is checked since the VisualErrorHandler also handle stuff with tooltips to avoid problems.
                if(!string.IsNullOrEmpty(args.ToolTipText) ||  !string.IsNullOrEmpty(args.ToolTipText))
                {
                    return;
                }

                if (args.HoveredWord == "JSON")
                {
                    args.ToolTipTitle = "JSON";
                    args.ToolTipText = "The JSON namespace object contains static methods for parsing values from and converting values to JavaScript Object Notation (JSON).\nhttps://devdocs.io/javascript/global_objects/json";
                    args.ToolTipIcon = ToolTipIcon.Info;
                }
            };
        }

        public void RegisterErrorReporter(ErrorReportThread errorThread)
        {
            errorThread.AddReporter(this.jsErrorReporter);
        }

        public void UnregisterErrorReporter(ErrorReportThread errorThread)
        {
            errorThread.RemoveReporter(this.jsErrorReporter);
        }

        private string ErrorReportingGetScript()
        {
            return this.fctb.Text;
        }
    }
}