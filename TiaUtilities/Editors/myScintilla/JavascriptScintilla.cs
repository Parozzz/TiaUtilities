using ScintillaNET;
using System.Reflection;
using System.Text.Json;
using TiaUtilities.Editors.ErrorReporting;
using TiaUtilities.Utility;
using Style = ScintillaNET.Style;

namespace TiaUtilities.Editors.myScintilla
{
    public class JavascriptScintilla
    {
        private const string keywords0 = "async await break case catch class const continue debugger default delete do else export extends false finally for function if import in instanceof let new null return super switch this throw true try typeof var void while yield";
        private const string keywords1 = "Array Date JSON Math Number number Object object String string console parseInt parseFloat undefined NaN";

        private static void SetScintillaLightStyle(Scintilla scintilla, Color? backColor = null, Color? foreColor = null)
        {
            var defaultBackColor = backColor ?? Color.FromArgb(250, 250, 250); // Sfondo quasi bianco
            var defaultForeColor = foreColor ?? Color.FromArgb(40, 40, 40); // Testo principale scuro

            // 1. Resetta lo stile base
            scintilla.StyleResetDefault();

            // Imposta font e colore dello sfondo principale (Bianco Puro)
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 11;
            scintilla.Styles[Style.Default].BackColor = defaultBackColor; // Sfondo Bianco
            scintilla.Styles[Style.Default].ForeColor = defaultForeColor;   // Testo base (Nero/Grigio scuro)

            scintilla.StyleClearAll(); // Applica il font/sfondo a tutti gli stili di default

            scintilla.SetKeywords(0, keywords0); //Style.Cpp.Word
            scintilla.SetKeywords(1, keywords1); //Style.Cpp.Word2

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 40;
            scintilla.Margins[1].Type = MarginType.Symbol;
            scintilla.Margins[1].Width = 15;

            scintilla.CaretLineBackColor = Color.FromArgb(243, 243, 243);
            scintilla.SelectionBackColor = Color.FromArgb(173, 214, 255);

            scintilla.Styles[Style.BraceLight].ForeColor = Color.FromArgb(30, 30, 30);
            scintilla.Styles[Style.BraceLight].Bold = true;
            scintilla.Styles[Style.BraceLight].BackColor = Color.FromArgb(210, 210, 210);

            scintilla.Styles[Style.BraceBad].ForeColor = Color.FromArgb(30, 30, 30);

            scintilla.Styles[Style.LineNumber].BackColor = Color.FromArgb(240, 240, 240); // Grigio chiarissimo
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120); // Grigio medio per i numeri

            // 4. COLORAZIONE SINTASSI (Sfondo Bianco)

            scintilla.Styles[Style.Cpp.Default].BackColor = defaultBackColor;

            // Parole Chiave (if, function, const, let...) -> Blu Scuro
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(0, 0, 255);
            scintilla.Styles[Style.Cpp.Word].Bold = true;

            // Oggetti Nativi / Metodi (Set 1) -> Blu Ottanio/Acqua
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.FromArgb(43, 145, 175);

            // Stringhe ("testo", 'testo') -> Rosso Scuro/Marrone
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Template Literals (`)
            scintilla.Styles[Style.Cpp.TripleVerbatim].ForeColor = Color.FromArgb(163, 21, 21);

            // Commenti (// e /* */) -> Verde Foresta
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla.Styles[Style.Cpp.CommentDoc].ForeColor = Color.FromArgb(0, 128, 0);

            // Numeri (123, 45.67) -> Verde Scuro/Smeraldo
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(9, 134, 88);

            // Operatori (+, -, =, ==, &&, ecc.) -> Grigio/Viola Scuro
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Cpp.Operator].Bold = true;

            // Identificatori (Variabili, Nomi Funzioni) -> Grigio Antracite
            scintilla.Styles[Style.Cpp.Identifier].ForeColor = Color.FromArgb(30, 30, 30);

            scintilla.Indicators[ScintillaTooltip.ERROR_INDICATOR].Style = IndicatorStyle.Squiggle;
            scintilla.Indicators[ScintillaTooltip.ERROR_INDICATOR].ForeColor = Color.Red;

            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].Symbol = MarkerSymbol.Arrow;
            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].SetBackColor(Color.Red);
            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].SetForeColor(Color.Transparent);

            scintilla.Indicators[ScintillaHighlighter.INDICATOR].Style = IndicatorStyle.GradientCenter;
            scintilla.Indicators[ScintillaHighlighter.INDICATOR].ForeColor = Color.DarkGray;
            scintilla.Indicators[ScintillaHighlighter.INDICATOR].Alpha = 128;
        }

        public Scintilla Scintilla { get; init; }

        /** Add suggestion for the popup divided by an empty space. */
        public IEnumerable<string> Suggestions { get => this.autoCList.Suggestions; set => this.autoCList.Suggestions = value; }

        public ScintillaTooltip.Error? CurrentError
        {
            get => this.tooltip.CurrentError;
            set => this.tooltip.CurrentError = value;
        }

        private readonly ScintillaHighlighter highlighter;
        private readonly ScintillaTooltip tooltip;
        private readonly ScintillaAutoCList autoCList;
        private readonly ScintillaBrackets brackets;

        public JavascriptScintilla(Scintilla? scintilla = null)
        {
            this.Scintilla = scintilla ?? new();

            this.highlighter = new(this.Scintilla);
            this.tooltip = new(this.Scintilla);
            this.autoCList = new(this.Scintilla);
            this.brackets = new(this.Scintilla);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.Scintilla.LexerName = this.Scintilla.GetLexerIDFromLexer(Lexer.SCLEX_JAVASCRIPT);

            this.Scintilla.IndentationGuides = IndentView.LookBoth;
            this.Scintilla.Dock = DockStyle.Fill;

            this.Scintilla.WrapIndentMode = WrapIndentMode.DeepIndent;

            this.Scintilla.MultipleSelection = true;
            this.Scintilla.MultiPaste = MultiPaste.Each;
            this.Scintilla.AdditionalSelectionTyping = true;
            this.Scintilla.AdditionalCaretsVisible = true;
            this.Scintilla.AdditionalCaretsBlink = true;

            JavascriptScintilla.SetScintillaLightStyle(this.Scintilla, backColor, foreColor);

            this.Scintilla.TabWidth = 4;
            this.Scintilla.UseTabs = false;
            this.Scintilla.MouseDwellTime = 800;

            this.Scintilla.BorderStyle = borderStyle ?? ScintillaNET.BorderStyle.None;

            this.Scintilla.DwellStart += (sender, e) =>
            {
                var errorDone = this.tooltip.EventDwellStart_Error(e.Position);
                if (!errorDone)
                {
                    this.tooltip.EventDwellStart_Docs(e.Position);
                }
            };

            this.Scintilla.DwellEnd += (sender, args) => this.tooltip.EventDwellStop();

            UpdateLineNumbers(0);
            this.Scintilla.Insert += (sender, args) =>
            {
                if (args.LinesAdded != 0)
                {
                    UpdateLineNumbers(Scintilla.LineFromPosition(args.Position));
                }
            };

            this.Scintilla.Delete += (sender, args) =>
            {
                if (args.LinesAdded != 0)
                {
                    UpdateLineNumbers(Scintilla.LineFromPosition(args.Position));
                }
            };

            this.Scintilla.BeforeDelete += (sender, args) =>
            {
                this.brackets.EventBeforeDelete_WrapSelection(args.Source, args.Text);
            };

            this.Scintilla.CharAdded += (sender, args) =>
            {
                var wrapSelectionDone = this.brackets.EventCharAdded_WrapSelection(args.Char, ScintillaUtils.IsJSBrace);
                if(wrapSelectionDone)
                {
                    return;
                }

                this.autoCList.EventCharAdded_Show();
                this.tooltip.EventCharAdded_ShowOnBracket(args.Char);

                var ignoredClosingifExistsDone = this.brackets.EventCharAdded_IgnoreClosingIfExists(args.Char, ScintillaUtils.IsJSBrace);
                if(!ignoredClosingifExistsDone)
                {
                    this.brackets.EventCharAdded_InsertMatchedBracket(args.Char);
                }
            };

            this.Scintilla.UpdateUI += (sender, args) =>
            {
                highlighter.EventUpdateUI_Brackets(args.Change, ScintillaUtils.IsJSBrace);
                highlighter.EventUpdateUI_SameSelectionWords(args.Change);
            };
        }

        private void UpdateLineNumbers(int startingAtLine)
        {
            // Starting at the specified line index, update each
            // subsequent line margin text with a hex line number.
            for (int i = startingAtLine; i < Scintilla.Lines.Count; i++)
            {
                Scintilla.Lines[i].MarginStyle = Style.LineNumber;
                Scintilla.Lines[i].MarginText = i.ToString().PadLeft(4, '0');
            }
        }
    }
}
