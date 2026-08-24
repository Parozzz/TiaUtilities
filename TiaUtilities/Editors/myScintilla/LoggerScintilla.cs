using ScintillaNET;

namespace TiaUtilities.Editors.myScintilla
{
    public class LoggerScintilla
    {
        public enum LogLevel { TRACE, DEBUG, INFO, WARN, ERROR };

        private const int STYLE_DEFAULT = 0;
        private const int STYLE_INFO = 1;
        private const int STYLE_WARN = 2;
        private const int STYLE_ERROR = 3;
        private const int STYLE_DATE = 4;

        private static void SetScintillaLightStyle(Scintilla scintilla, Color? backColor = null, Color? foreColor = null)
        {
            var defaultBackColor = backColor ?? Color.FromArgb(250, 250, 250); // Sfondo quasi bianco
            var defaultForeColor = foreColor ?? Color.FromArgb(40, 40, 40); // Testo principale scuro

            // 2. Stile di default (Sfondo chiaro e testo base grigio scuro)
            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 8;
            scintilla.Styles[Style.Default].BackColor = defaultBackColor;
            scintilla.Styles[Style.Default].ForeColor = defaultForeColor;

            scintilla.StyleClearAll(); // Applica le basi a tutti gli stili

            

            scintilla.Styles[STYLE_INFO].ForeColor = Color.FromArgb(0, 122, 204);   // Blu
            scintilla.Styles[STYLE_WARN].ForeColor = Color.FromArgb(200, 130, 0);   // Arancione
            scintilla.Styles[STYLE_ERROR].ForeColor = Color.FromArgb(215, 58, 73);   // Rosso
            scintilla.Styles[STYLE_ERROR].Bold = true;
            scintilla.Styles[STYLE_DATE].ForeColor = Color.FromArgb(110, 110, 110); // Grigio

            scintilla.CaretForeColor = Color.Black;
            scintilla.SelectionBackColor = Color.FromArgb(173, 214, 255); // Evidenziazione azzurra per il testo selezionato

            scintilla.Indicators[ScintillaHighlighter.INDICATOR].Style = IndicatorStyle.GradientCenter;
            scintilla.Indicators[ScintillaHighlighter.INDICATOR].ForeColor = Color.DarkGray;
            scintilla.Indicators[ScintillaHighlighter.INDICATOR].Alpha = 128;
        }

        public string Text
        {
            get => this.Scintilla.Text;
            set
            {
                var wasReadOnly = this.Scintilla.ReadOnly;

                this.Scintilla.ReadOnly = false;
                this.Scintilla.Text = value;
                this.Scintilla.ReadOnly = wasReadOnly;
            }
        }

        public Scintilla Scintilla { get; init; }

        private readonly ScintillaHighlighter highlighter;

        public LoggerScintilla(Scintilla? scintilla = null)
        {
            this.Scintilla = scintilla ?? new();

            this.highlighter = new(this.Scintilla);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.Scintilla.LexerName = "";

            this.Scintilla.IndentationGuides = IndentView.LookBoth;
            this.Scintilla.Dock = DockStyle.Fill;

            this.Scintilla.WrapIndentMode = WrapIndentMode.DeepIndent;

            this.Scintilla.MultipleSelection = true;
            this.Scintilla.MultiPaste = MultiPaste.Each;
            this.Scintilla.AdditionalSelectionTyping = true;
            this.Scintilla.AdditionalCaretsVisible = true;
            this.Scintilla.AdditionalCaretsBlink = true;

            LoggerScintilla.SetScintillaLightStyle(this.Scintilla, backColor, foreColor);

            this.Scintilla.TabWidth = 4;
            this.Scintilla.UseTabs = false;
            this.Scintilla.MouseDwellTime = 800;

            this.Scintilla.BorderStyle = borderStyle ?? ScintillaNET.BorderStyle.None;

            this.Scintilla.StyleNeeded += (sender, args) =>
            {
                // 1. Trova il punto preciso da cui Scintilla ha smesso di applicare lo stile
                int startPos = this.Scintilla.GetEndStyled();
                int endPos = args.Position;

                // Se non c'è nulla da formattare, esci
                if (startPos >= endPos)
                {
                    return;
                }

                // Espandiamo l'intervallo all'inizio della riga corrente per non spezzare le Regex a metà
                int lineIndex = this.Scintilla.LineFromPosition(startPos);
                startPos = this.Scintilla.Lines[lineIndex].Position;

                // Prendiamo il testo nell'intervallo richiesto
                int length = endPos - startPos;
                string text = this.Scintilla.GetTextRange(startPos, length);

                // 2. Resetta lo stile di default per QUESTO blocco di testo
                this.Scintilla.StartStyling(startPos);
                this.Scintilla.SetStyling(length, STYLE_DEFAULT);

                // 3. Esegui le Regex (sommando startPos per ottenere le posizioni assolute in Scintilla)
                HighlightPattern(this.Scintilla, text, startPos, @"\d{2}-\d{2}-\d{2,4}\s+\d{2}:\d{2}:\d{2}([\.,]\d{1,3})?", STYLE_DATE);
                HighlightPattern(this.Scintilla, text, startPos, @"\[(INFO|DEBUG|TRACE)\]", STYLE_INFO);
                HighlightPattern(this.Scintilla, text, startPos, @"\[(WARN|WARNING)\]", STYLE_WARN);
                HighlightPattern(this.Scintilla, text, startPos, @"\[(ERROR|FATAL|FAIL|EXCEPTION)\]", STYLE_ERROR);

                this.Scintilla.StartStyling(endPos);
            };

            this.Scintilla.UpdateUI += (sender, args) =>
            {
                highlighter.EventUpdateUI_SameSelectionWords(args.Change);
            };
        }

        public void AppendLine(DateTime dateTime, LogLevel level, string text)
        {
            var wasReadOnly = this.Scintilla.ReadOnly;

            var timeStr = dateTime.ToString("dd-MM-yy HH:mm:ss.fff");

            var line = $"{timeStr} [{level}] > {text}\r\n";
            this.Scintilla.ReadOnly = false;
            this.Scintilla.AppendText(line);
            this.Scintilla.ReadOnly = wasReadOnly;

            this.Scintilla.FirstVisibleLine = this.Scintilla.Lines.Count - 1;
        }

        private static void HighlightPattern(Scintilla scintilla, string text, int baseOffset, string pattern, int style)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(
                text,
                pattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                // Posizione ASSOLUTA all'interno dell'intero documento Scintilla
                int absoluteIndex = baseOffset + match.Index;

                scintilla.StartStyling(absoluteIndex);
                scintilla.SetStyling(match.Length, style);
            }
        }
    }
}
