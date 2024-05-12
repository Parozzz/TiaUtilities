using FastColoredTextBoxNS;
using System.Text.RegularExpressions;
using TiaXmlReader.Javascript.FCTB;
using TiaXmlReader.Utility;
using Timer = System.Windows.Forms.Timer;

namespace TiaXmlReader.Javascript
{
    public class JavascriptFCTB
    {
        private static readonly Style SAME_WORDS_STYLE = new MarkerStyle(new SolidBrush(Color.FromArgb(65, Color.Green)));
        private static readonly Style LINE_ERROR_WAVY_STYLE = new CustomWavyLineStyle(Color.FromArgb(255, Color.DarkRed), Color.FromArgb(125, Color.MediumVioletRed));
        private static readonly MarkerStyle HIGHLIGHTED_BRACKET_STYLE = new MarkerStyle(new SolidBrush(Color.FromArgb(150, Color.LightGreen)));

        private const int SHOW_ERROR_DELAY_AFTER_TEXT_CHANGED = 1250; //ms

        private readonly FastColoredTextBox fctb;

        private JavascriptErrorReportThread.JSScriptReport? scriptReport;
        private DisplayedError? currentError;

        private bool haltError; //This is required to avoid having the wavy style contantly flash on screen. After changing text, give some time before accetting new text!

        public JavascriptFCTB()
        {
            this.fctb = new FastColoredTextBox();
        }

        public FastColoredTextBox GetFCTB()
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
            fctb.AutoIndentChars = true;
            fctb.TabLength = 4;
            // == LINE NUMBERS ==
            fctb.ShowLineNumbers = true;
            fctb.LineNumberStartValue = 1;
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

            //Highligh same word if something is selected.
            fctb.SelectionChangedDelayed += SelectionChangedDelayed;

            fctb.ToolTipNeeded += (sender, args) =>
            {
                if(currentError != null && currentError.Line == args.Place.iLine && args.Place.iChar >= currentError.Column)
                {
                    args.ToolTipTitle = "Error";
                    args.ToolTipText = this.currentError.Description;
                    args.ToolTipIcon = ToolTipIcon.Error;
                }
                else if(args.HoveredWord == "JSON")
                {
                    args.ToolTipTitle = "JSON";
                    args.ToolTipText = "The JSON namespace object contains static methods for parsing values from and converting values to JavaScript Object Notation (JSON).\nhttps://devdocs.io/javascript/global_objects/json";
                    args.ToolTipIcon = ToolTipIcon.Info;
                }
                else
                {
                    args.ToolTipText = "";
                }
            };

            var resetHaltErrorTimer = new Timer() { Interval = SHOW_ERROR_DELAY_AFTER_TEXT_CHANGED };
            resetHaltErrorTimer.Tick += (sender, args) =>
            {
                resetHaltErrorTimer.Stop();
                haltError = false;
            };

            fctb.TextChanged += (sender, args) =>
            {
                this.ClearError();
                this.haltError = true;

                resetHaltErrorTimer.Stop(); //This reset the timer.
                resetHaltErrorTimer.Start();
            };
        }

        public void RegisterErrorReport(JavascriptErrorReportThread errorReportingThread)
        {
            this.scriptReport = errorReportingThread.RegisterScript(() => fctb.Text, this.ScriptReportDone);
        }

        public void UnregisterErrorReport(JavascriptErrorReportThread errorReportingThread)
        {
            if(scriptReport != null)
            {
                errorReportingThread.RemoveScript(this.scriptReport);
                this.scriptReport = null;
            }
        }

        private void ScriptReportDone()
        {
            if (scriptReport == null || haltError)
            {
                return;
            }

            var jsError = this.scriptReport.JSError;
            if(jsError == null)
            {
                if(currentError != null)
                {
                    this.ClearError();
                }
            }
            else
            {
                var newError = new DisplayedError()
                {
                    Line = jsError.Line - 1, //Lines in FCTB start from 0!
                    Column = jsError.Column,
                    Description = jsError.Description,
                };

                if(this.AreErrorLimitValid(newError) && Utils.AreDifferentObject(this.currentError, newError))
                {
                    this.ClearError();

                    this.currentError = newError;
                    UpdateCurrentError();
                }
            }
        }

        private void UpdateCurrentError()
        {
            if(this.currentError == null || !this.AreErrorLimitValid(currentError))
            {
                return;
            }

            var lineRange = this.fctb.GetLine(this.currentError.Line);

            var start = lineRange.Start;
            var end = lineRange.End;
            if (this.currentError.Column >= 0 && this.currentError.Column < lineRange.Length)
            {
                lineRange.Start = new Place(this.currentError.Column, this.currentError.Line);
                lineRange.End = end;
            }

            lineRange.SetStyle(LINE_ERROR_WAVY_STYLE);
        }

        private void ClearError()
        {
            fctb.Range.ClearStyle(LINE_ERROR_WAVY_STYLE);
            this.currentError = null;
        }

        private bool AreErrorLimitValid(DisplayedError error)
        {
            return error.Line >= 0 && error.Line < fctb.LinesCount && error.Column >= 0 && !string.IsNullOrEmpty(error.Description);
        }

        private void SelectionChangedDelayed(object? sender, EventArgs args)
        {
            try
            {
                this.fctb.VisibleRange.ClearStyle(SAME_WORDS_STYLE);

                //get fragment around caret
                string text = this.fctb.Selection.Text;
                if (text.Length < 2)
                {
                    return;
                }

                //highlight same words
                var ranges = this.fctb.VisibleRange.GetRanges(Regex.Escape(text)).ToArray();
                if (ranges.Length > 1)
                {
                    foreach (var r in ranges)
                    {
                        r.SetStyle(SAME_WORDS_STYLE);
                    }
                }
            } catch(Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }
        }
    }

    class DisplayedError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Description { get; set; }
    }
}