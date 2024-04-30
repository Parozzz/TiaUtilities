using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TiaXmlReader.Javascript
{
    public class JavascriptFCTB
    {
        private static readonly Style SAME_WORDS_STYLE = new MarkerStyle(new SolidBrush(Color.FromArgb(65, Color.Green)));
    private static readonly Style LINE_ERROR_WAVY_STYLE = new WavyLineStyle(255, Color.DarkRed); //new TextStyle(Brushes.Red, null, FontStyle.Italic);

        private static readonly MarkerStyle HIGHLIGHTED_BRACKET_STYLE = new MarkerStyle(new SolidBrush(Color.FromArgb(150, Color.LightGreen)));

        private readonly FastColoredTextBox fctb;

        private int currentErrorLine;
        private string currentErrorDescription;

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
            // == BRACKETS ==
            fctb.AutoCompleteBrackets = true;
            fctb.AutoCompleteBracketsList = new char[] { '(', ')', '{', '}', '\"', '\"', '\'', '\'' };
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
            
            fctb.AcceptsTab = true;
            fctb.AcceptsReturn = true;
            fctb.ShowFoldingLines = false;

            fctb.Language = Language.JS;
            fctb.HighlightingRangeType = HighlightingRangeType.AllTextRange;

            //Highligh same word if something is selected.
            fctb.SelectionChangedDelayed += SelectionChangedDelayed;

            fctb.ToolTip = new ToolTip();
            fctb.ToolTipNeeded += (sender, args) =>
            {
                if (this.currentErrorLine < 0 || string.IsNullOrEmpty(this.currentErrorDescription) || args.Place.iLine != this.currentErrorLine)
                {
                    args.ToolTipText = "";
                    return;
                }

                args.ToolTipTitle = "Error";
                args.ToolTipText = this.currentErrorDescription;
                args.ToolTipIcon = ToolTipIcon.Error;
            };
            ClearErrors();
        }

        public void ClearErrors()
        {
            this.currentErrorLine = -1;
            this.currentErrorDescription = "";

            fctb.Range.ClearStyle(LINE_ERROR_WAVY_STYLE);
        }

        public void SetShownError(int line, string description)
        {
            if (line < 0 || line >= fctb.Lines.Count || line == this.currentErrorLine || description == null || description.Equals(this.currentErrorDescription))
            {
                return;
            }

            this.currentErrorLine = line;
            this.currentErrorDescription = description;

            var lineRange = this.fctb.GetLine(line);
            lineRange.SetStyle(LINE_ERROR_WAVY_STYLE);
        }

        public bool HasError()
        {
            return this.currentErrorLine >= 0 || !string.IsNullOrEmpty(this.currentErrorDescription);
        }

        private void SelectionChangedDelayed(object sender, EventArgs args)
        {
            this.fctb.VisibleRange.ClearStyle(SAME_WORDS_STYLE);

            //get fragment around caret
            string text = this.fctb.Selection.Text;
            if (text.Length < 2)
            {
                return;
            }

            //highlight same words
            var ranges = this.fctb.VisibleRange.GetRanges(text).ToArray();
            if (ranges.Length > 1)
            {
                foreach (var r in ranges)
                {
                    r.SetStyle(SAME_WORDS_STYLE);
                }
            }
        }
    }
}

/*

var autoCompleteItemList = new List<AutocompleteItem>();

var engine = new Engine();
foreach(var globalPropertyEntry in engine.Global.GetOwnProperties())
{
    var jsValue = globalPropertyEntry.Key;
    var globalPropertyDescriptor = globalPropertyEntry.Value;

    if (globalPropertyDescriptor.Value is Function function)
    {
        foreach(var functionPropertyEntry in function.GetOwnProperties())
        {
            if(functionPropertyEntry.Key.IsString() && functionPropertyEntry.Key.AsString() == "name")
            {
                var functionPropertyDescriptor = functionPropertyEntry.Value;
                if(functionPropertyDescriptor.Value.IsString())
                {
                    var item = new AutocompleteItem(functionPropertyDescriptor.Value.AsString()); //Text in the constructor is needed! Otherwise null exception is thrown.
                    autoCompleteItemList.Add(item);
                }

            }
            var ___ = "";
        }
        var __ = "";
    }

    var _ = "";
}

var item = new AutocompleteItem("wow") //Text in the constructor is needed! Otherwise null exception is thrown.
{
    MenuText = "wow",
    ToolTipTitle = "Desc",
    ToolTipText = "wowowowowow",
};

var m = new AutocompleteMenu(fctb);
m.Items.SetAutocompleteItems(autoCompleteItemList);
m.Show(true);
*/