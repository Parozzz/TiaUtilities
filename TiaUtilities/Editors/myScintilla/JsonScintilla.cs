using ScintillaNET;

namespace TiaUtilities.Editors.myScintilla
{
    public class JsonScintilla
    {

        private static void SetScintillaLightStyle(Scintilla scintilla, Color? backColor = null, Color? foreColor = null)
        {
            var defaultBackColor = backColor ?? Color.FromArgb(250, 250, 250); // Sfondo quasi bianco
            var defaultForeColor = foreColor ?? Color.FromArgb(40, 40, 40); // Testo principale scuro

            // 2. Stile di default (Sfondo chiaro e testo base grigio scuro)
            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 11;
            scintilla.Styles[Style.Default].BackColor = defaultBackColor;
            scintilla.Styles[Style.Default].ForeColor = defaultForeColor;

            scintilla.StyleClearAll(); // Applica le basi a tutti gli stili

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 40;
            scintilla.Margins[1].Type = MarginType.Symbol;
            scintilla.Margins[1].Width = 15;

            scintilla.CaretForeColor = Color.Black;
            scintilla.SelectionBackColor = Color.FromArgb(173, 214, 255); // Evidenziazione azzurra per il testo selezionato

            scintilla.Styles[Style.LineNumber].BackColor = Color.FromArgb(240, 240, 240); // Grigio chiarissimo
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120); // Grigio medio per i numeri

            scintilla.Styles[Style.BraceLight].ForeColor = Color.FromArgb(30, 30, 30);
            scintilla.Styles[Style.BraceLight].Bold = true;
            scintilla.Styles[Style.BraceLight].BackColor = Color.FromArgb(210, 210, 210);

            scintilla.Styles[Style.BraceBad].ForeColor = Color.FromArgb(30, 30, 30);

            // 3. Colori specifici per il JSON

            // Nomi delle proprietà / Chiavi (Blu scuro/indaco - es. "name":)

            scintilla.Styles[Style.Json.Default].BackColor = defaultBackColor;

            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(4, 81, 165);
            scintilla.Styles[Style.Json.PropertyName].Bold = true;

            // Valori Stringa (Rosso mattone / Bordeaux - es. "John")
            scintilla.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21);

            // Valori Numerici (Verde oliva scuro - es. 123, 45.67)
            scintilla.Styles[Style.Json.Number].ForeColor = Color.FromArgb(9, 134, 88);

            // Booleani e Null (Blu / Viola acceso - es. true, false, null)
            scintilla.Styles[Style.Json.Keyword].ForeColor = Color.FromArgb(0, 0, 255);
            scintilla.Styles[Style.Json.Keyword].Bold = true;

            // Simboli e Marcatori: Parentesi { }, Quadre [ ], Due punti :, Virgole ,
            scintilla.Styles[Style.Json.Operator].ForeColor = Color.FromArgb(50, 50, 50);

            // Caratteri di Escape dentro le stringhe (es. \n, \t, \")
            scintilla.Styles[Style.Json.EscapeSequence].ForeColor = Color.FromArgb(238, 0, 0);

            // Commenti (se presenti in JSON con commenti / JSONC)
            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0);

            // Errore di Sintassi (Sfondo rosa tenue per evidenziare stringhe o elementi non validi)
            scintilla.Styles[Style.Json.Error].ForeColor = Color.DarkRed;
            
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

        public ScintillaTooltip.Error? CurrentError
        {
            get => this.tooltip.CurrentError;
            set => this.tooltip.CurrentError = value;
        }

        private readonly ScintillaHighlighter highlighter;
        private readonly ScintillaTooltip tooltip;
        private readonly ScintillaBrackets brackets;

        public JsonScintilla(Scintilla? scintilla = null)
        {
            this.Scintilla = scintilla ?? new();

            this.highlighter = new(this.Scintilla);
            this.tooltip = new(this.Scintilla);
            this.brackets = new(this.Scintilla);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.Scintilla.LexerName = this.Scintilla.GetLexerIDFromLexer(Lexer.SCLEX_JSON);

            this.Scintilla.IndentationGuides = IndentView.LookBoth;
            this.Scintilla.Dock = DockStyle.Fill;

            this.Scintilla.WrapIndentMode = WrapIndentMode.DeepIndent;

            this.Scintilla.MultipleSelection = true;
            this.Scintilla.MultiPaste = MultiPaste.Each;
            this.Scintilla.AdditionalSelectionTyping = true;
            this.Scintilla.AdditionalCaretsVisible = true;
            this.Scintilla.AdditionalCaretsBlink = true;

            JsonScintilla.SetScintillaLightStyle(this.Scintilla, backColor, foreColor);

            this.Scintilla.TabWidth = 4;
            this.Scintilla.UseTabs = false;
            this.Scintilla.MouseDwellTime = 800;

            this.Scintilla.BorderStyle = borderStyle ?? ScintillaNET.BorderStyle.None;

            this.Scintilla.DwellStart += (sender, e) => this.tooltip.EventDwellStart_Error(e.Position);
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

            /*
            this.Scintilla.BeforeDelete += (sender, args) =>
            {
                this.brackets.EventBeforeDelete_WrapSelection(args.Source, args.Text);
            };
            */
            this.Scintilla.CharAdded += (sender, args) =>
            {
                var wrapSelectionDone = this.brackets.EventCharAdded_WrapSelection(args.Char, ScintillaUtils.IsJSBrace);
                if (wrapSelectionDone)
                {
                    return;
                }
                
                //this.tooltip.EventCharAdded_ShowOnBracket(args.Char);
                
                var ignoredClosingifExistsDone = this.brackets.EventCharAdded_IgnoreClosingIfExists(args.Char, ScintillaUtils.IsJSBrace);
                if (!ignoredClosingifExistsDone)
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
