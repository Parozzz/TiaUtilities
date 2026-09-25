using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Editors.myScintilla;

namespace TiaUtilities.Editors.T_SQL
{
    public class TSQLScintilla
    {
        // Set 0: Istruzioni T-SQL principali (DML, DDL, Controllo Flusso)
        private const string keywords0 =
            "add all alter and any as asc authorization backup begin between break browse bulk by cascade case catch " +
            "check checkpoint close clustered coalesce collate column commit compute constraint containstable continue " +
            "create cross current current_date current_time current_timestamp current_user cursor database dbcc " +
            "deallocate declare default delete deny desc disk distinct distributed double drop dump else end errlvl " +
            "escape except exec execute exists exit external fetch file fillfactor for foreign freetext freetexttable " +
            "from full goto grant group having holdlock identity identity_insert identitycol if in index inner insert " +
            "intersect into is join key kill left like lineno load merge national nocheck nonclustered not null " +
            "nullif of off offsets on open opendatasource openquery openrowset openxml option or order outer over " +
            "percent pivot plan precision primary print proc procedure public raiserror read readtext reconfigure " +
            "references replication restore restrict return revert revoke right rollback rollup rowcount rowguidcol " +
            "rule save schema select session_user set sets shutdown some stats table tablesample textsize then " +
            "to top tran transaction trigger truncate try union unique unpivot update updatetext use user values " +
            "varying view waitfor when where while with writetext";

        // Set 1: Tipi di Dato e Funzioni Aggregate / Sistema Native di T-SQL
        private const string keywords1 =
            "bigint binary bit char date datetime datetime2 datetimeoffset decimal float image int money nchar ntext " +
            "numeric nvarchar real smalldatetime smallint smallmoney sql_variant text time timestamp tinyint uniqueidentifier " +
            "varbinary varchar xml " +
            "avg count max min sum checksum_agg count_big grouping grouping_id statscol_number stdev stdevp var varp " +
            "abs acos asin atan atn2 ceiling cos cot degrees exp floor log log10 pi radians rand round sign sin sqrt tan " +
            "cast convert try_cast try_convert coalesce nullif len charindex substring replace lower upper ltrim rtrim " +
            "getdate sysdatetime getutcdate ISNULL ISNUMERIC SCOPE_IDENTITY ROW_NUMBER RANK DENSE_RANK OVER";

        private static void SetScintillaLightStyle(Scintilla scintilla, Color? backColor = null, Color? foreColor = null)
        {
            var defaultBackColor = backColor ?? Color.FromArgb(250, 250, 250);
            var defaultForeColor = foreColor ?? Color.FromArgb(40, 40, 40);

            // 1. Resetta lo stile base
            scintilla.StyleResetDefault();

            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 11;
            scintilla.Styles[Style.Default].BackColor = defaultBackColor;
            scintilla.Styles[Style.Default].ForeColor = defaultForeColor;

            scintilla.StyleClearAll();

            // 2. Impostazione Parole Chiave per il Lexer SQL
            scintilla.SetKeywords(0, keywords0.ToLower());
            scintilla.SetKeywords(1, keywords1.ToLower());

            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 40;
            scintilla.Margins[1].Type = MarginType.Symbol;
            scintilla.Margins[1].Width = 15;

            scintilla.CaretLineBackColor = Color.FromArgb(243, 243, 243);
            scintilla.SelectionBackColor = Color.FromArgb(173, 214, 255);

            scintilla.Styles[Style.BraceLight].ForeColor = Color.FromArgb(30, 30, 30);
            scintilla.Styles[Style.BraceLight].Bold = true;
            scintilla.Styles[Style.BraceLight].BackColor = Color.FromArgb(210, 210, 210);

            scintilla.Styles[Style.BraceBad].ForeColor = Color.FromArgb(200, 0, 0);

            scintilla.Styles[Style.LineNumber].BackColor = Color.FromArgb(240, 240, 240);
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120);

            // 3. COLORAZIONE SINTASSI SQL (ScintillaNET.Style.Sql)

            scintilla.Styles[Style.Sql.Default].BackColor = defaultBackColor;

            // Parole Chiave DML/DDL (SELECT, FROM, WHERE, CREATE...) -> Blu Scuro
            scintilla.Styles[Style.Sql.Word].ForeColor = Color.FromArgb(0, 0, 255);
            scintilla.Styles[Style.Sql.Word].Bold = true;

            // Tipi di dato e Funzioni (INT, VARCHAR, GETDATE()...) -> Magenta/Viola o Ottanio
            scintilla.Styles[Style.Sql.Word2].ForeColor = Color.FromArgb(43, 145, 175);
            scintilla.Styles[Style.Sql.Word2].Bold = true;

            // Stringhe tra apici singoli ('testo') -> Rosso Scuro/Marrone
            scintilla.Styles[Style.Sql.String].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Sql.Character].ForeColor = Color.FromArgb(163, 21, 21);

            // Commenti a riga singola (-- commento) -> Verde Foresta
            scintilla.Styles[Style.Sql.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla.Styles[Style.Sql.CommentLineDoc].ForeColor = Color.FromArgb(0, 128, 0);

            // Commenti blocco (/* commento */) -> Verde Foresta
            scintilla.Styles[Style.Sql.Comment].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla.Styles[Style.Sql.CommentDoc].ForeColor = Color.FromArgb(0, 128, 0);

            // Numeri -> Verde Smeraldo
            scintilla.Styles[Style.Sql.Number].ForeColor = Color.FromArgb(9, 134, 88);

            // Operatori (+, -, =, <>, LIKE, IS...) -> Viola
            scintilla.Styles[Style.Sql.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Sql.Operator].Bold = true;

            // Identificatori (Nomi Tabelle, Colonne, Variabili) -> Grigio Antracite
            scintilla.Styles[Style.Sql.Identifier].ForeColor = Color.FromArgb(30, 30, 30);

            /*
            // Identificatori T-SQL delimitati [Nome Tabella] o "Nome Tabella" -> Marrone/Bordeaux
            scintilla.Styles[Style.Sql.Quoted].ForeColor = Color.FromArgb(128, 0, 0);
            
            // Variabili T-SQL (@myVar, @@ROWCOUNT) -> Grigio Scuro Bold
            scintilla.Styles[Style.Sql.Variable].ForeColor = Color.FromArgb(80, 80, 80);
            scintilla.Styles[Style.Sql.Variable].Bold = true;
            */
            // Indicatori ed Errori
            scintilla.Indicators[ScintillaTooltip.ERROR_INDICATOR].Style = IndicatorStyle.Squiggle;
            scintilla.Indicators[ScintillaTooltip.ERROR_INDICATOR].ForeColor = Color.Red;

            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].Symbol = MarkerSymbol.Arrow;
            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].SetBackColor(Color.Red);
            scintilla.Markers[ScintillaTooltip.ERROR_MARKER].SetForeColor(Color.Transparent);

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

        public IEnumerable<string> Suggestions
        {
            get => this.autoCList.Suggestions;
            set => this.autoCList.Suggestions = value;
        }

        public ScintillaTooltip.Error? CurrentError
        {
            get => this.tooltip.CurrentError;
            set => this.tooltip.CurrentError = value;
        }

        private readonly ScintillaHighlighter highlighter;
        private readonly ScintillaTooltip tooltip;
        private readonly ScintillaAutoCList autoCList;
        private readonly ScintillaBrackets brackets;

        public TSQLScintilla(Scintilla? scintilla = null)
        {
            this.Scintilla = scintilla ?? new();

            this.highlighter = new(this.Scintilla);
            this.tooltip = new(this.Scintilla);
            this.autoCList = new(this.Scintilla);
            this.brackets = new(this.Scintilla);
        }

        public void InitControl(ScintillaNET.BorderStyle? borderStyle = null, Color? backColor = null, Color? foreColor = null)
        {
            this.Scintilla.LexerName = this.Scintilla.GetLexerIDFromLexer(Lexer.SCLEX_SQL);

            // In T-SQL l'escape dell'apice si fa con '' (due apici singoli)
            // Questa proprietà dice al lexer di non interrompere la stringa quando incontra un apice raddoppiato
            this.Scintilla.SetProperty("sql.backslash.escapes", "false");

            // Permette alle stringhe tra apici di estendersi su più righe senza rompere lo stile
            this.Scintilla.SetProperty("lexer.sql.backquotes.allowed", "1");

            this.Scintilla.IndentationGuides = IndentView.LookBoth;
            this.Scintilla.Dock = DockStyle.Fill;

            this.Scintilla.WrapIndentMode = WrapIndentMode.DeepIndent;

            this.Scintilla.MultipleSelection = true;
            this.Scintilla.MultiPaste = MultiPaste.Each;
            this.Scintilla.AdditionalSelectionTyping = true;
            this.Scintilla.AdditionalCaretsVisible = true;
            this.Scintilla.AdditionalCaretsBlink = true;

            TSQLScintilla.SetScintillaLightStyle(this.Scintilla, backColor, foreColor);

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
                // Assicurati che ScintillaUtils.IsSqlBrace verifichi '(', ')', '[', ']', non le parentesi graffe
                var wrapSelectionDone = this.brackets.EventCharAdded_WrapSelection(args.Char, ScintillaUtils.IsTSQLBrace);
                if (wrapSelectionDone)
                {
                    return;
                }

                this.autoCList.EventCharAdded_Show();
                this.tooltip.EventCharAdded_ShowOnBracket(args.Char);

                var ignoredClosingIfExistsDone = this.brackets.EventCharAdded_IgnoreClosingIfExists(args.Char, ScintillaUtils.IsTSQLBrace);
                if (!ignoredClosingIfExistsDone)
                {
                    this.brackets.EventCharAdded_InsertMatchedBracket(args.Char);
                }
            };

            this.Scintilla.UpdateUI += (sender, args) =>
            {
                highlighter.EventUpdateUI_Brackets(args.Change, ScintillaUtils.IsTSQLBrace);
                highlighter.EventUpdateUI_SameSelectionWords(args.Change);
            };
        }

        private void UpdateLineNumbers(int startingAtLine)
        {
            for (int i = startingAtLine; i < Scintilla.Lines.Count; i++)
            {
                Scintilla.Lines[i].MarginStyle = Style.LineNumber;
                Scintilla.Lines[i].MarginText = i.ToString().PadLeft(4, '0');
            }
        }
    }
}
