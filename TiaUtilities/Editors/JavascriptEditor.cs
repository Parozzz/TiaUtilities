using Acornima.Ast;
using FastColoredTextBoxNS;
using Jint;
using Jint.Native;
using ScintillaNET;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using TiaUtilities.Editors.ErrorReporting;
using Style = ScintillaNET.Style;

namespace TiaUtilities.Editors
{
    public class FunctionDoc
    {
        public string Name { get; set; }
        public string Syntax { get; set; }
        public string Description { get; set; }
    }

    public class JavascriptEditor
    {
        private static readonly MarkerStyle HIGHLIGHTED_BRACKET_STYLE = new(new SolidBrush(Color.FromArgb(150, Color.LightGreen)));

        const string paroleJavaScript = "Array Array.from Array.isArray " +
                                        "Console console console.error console.log console.warn " +
                                        "Date Date.now Date.parse " +
                                        "JSON JSON.parse JSON.stringify " +
                                        "Math Math.abs Math.ceil Math.floor Math.max Math.min Math.pow Math.random Math.round Math.sqrt " +
                                        "Number Number.isInteger Number.parseFloat Number.parseInt " +
                                        "Object Object.assign Object.keys Object.values " +
                                        "String String.fromCharCode " +
                                        "async await break case catch class const continue debugger default delete do else export extends " +
                                        "false finally for function if import in instanceof isNaN new null decodeURI encodeURI " +
                                        "parseFloat parseInt return super switch this throw true try typeof undefined var void while with yield";

        private readonly FastColoredTextBox textBox;
        private readonly Scintilla scintilla;

        private readonly JavascriptErrorReporter jsErrorReporter;
        private readonly FCTBErrorVisualizer visualErrorHandler;


        private Dictionary<string, FunctionDoc> mappaDocumentazione = [];

        public JavascriptEditor(FastColoredTextBox? textBox = null)
        {
            this.textBox = textBox ?? new();
            this.scintilla = new();

            this.jsErrorReporter = new(this.ErrorReportingGetScript);
            this.visualErrorHandler = new(this.textBox, this.jsErrorReporter);
        }

        public FastColoredTextBox GetTextBox()
        {
            return textBox;
        }

        public Scintilla GetScintilla()
        {
            return scintilla;
        }

        
        private void CaricaDocumentazione()
        {
            Assembly.GetExecutingAssembly().GetManifestResourceStream("NomeNSP.NomeFile.json")
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JSScript", "jsDoc.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText("javascript_docs.json");
                var lista = JsonSerializer.Deserialize<List<FunctionDoc>>(json);

                // Indicizza per nome così da fare ricerche istantanee
                this.mappaDocumentazione = lista.ToDictionary(item => item.Name, item => item);
            }
        }

        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<':
                case '>':
                    return true;
            }

            return false;
        }

        private void InsertMatchedChars(CharAddedEventArgs e)
        {
            var caretPos = scintilla.CurrentPosition;
            var docStart = caretPos == 1;
            var docEnd = caretPos == scintilla.Text.Length;

            var charPrev = docStart ? scintilla.GetCharAt(caretPos) : scintilla.GetCharAt(caretPos - 2);
            var charNext = scintilla.GetCharAt(caretPos);

            var isCharPrevBlank = charPrev == ' ' || charPrev == '\t' ||
                                  charPrev == '\n' || charPrev == '\r';

            var isCharNextBlank = charNext == ' ' || charNext == '\t' ||
                                  charNext == '\n' || charNext == '\r' ||
                                  docEnd;

            var isEnclosed = (charPrev == '(' && charNext == ')') ||
                                  (charPrev == '{' && charNext == '}') ||
                                  (charPrev == '[' && charNext == ']');

            var isSpaceEnclosed = (charPrev == '(' && isCharNextBlank) || (isCharPrevBlank && charNext == ')') ||
                                  (charPrev == '{' && isCharNextBlank) || (isCharPrevBlank && charNext == '}') ||
                                  (charPrev == '[' && isCharNextBlank) || (isCharPrevBlank && charNext == ']');

            var isCharOrString = (isCharPrevBlank && isCharNextBlank) || isEnclosed || isSpaceEnclosed;

            var charNextIsCharOrString = charNext == '"' || charNext == '\'';

            switch (e.Char)
            {
                case '(':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, ")");
                    break;
                case '{':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "}");
                    break;
                case '[':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "]");
                    break;
                case '"':
                    // 0x22 = "
                    if (charPrev == 0x22 && charNext == 0x22)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "\"");
                    break;
                case '\'':
                    // 0x27 = '
                    if (charPrev == 0x27 && charNext == 0x27)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "'");
                    break;
            }
        }

        private void UpdateLineNumbers(int startingAtLine)
        {
            // Starting at the specified line index, update each
            // subsequent line margin text with a hex line number.
            for (int i = startingAtLine; i < scintilla.Lines.Count; i++)
            {
                scintilla.Lines[i].MarginStyle = Style.LineNumber;
                scintilla.Lines[i].MarginText = i.ToString().PadLeft(4, '0');
            }
        }

        private void setScintillaStyle(Scintilla scintilla1)
        {
            // 1. Resetta lo stile base
            scintilla1.StyleResetDefault();

            // Imposta font e colore dello sfondo principale (Bianco Puro)
            scintilla1.Styles[Style.Default].Font = "Consolas";
            scintilla1.Styles[Style.Default].Size = 10;
            scintilla1.Styles[Style.Default].BackColor = Color.FromArgb(255, 255, 255); // Sfondo Bianco
            scintilla1.Styles[Style.Default].ForeColor = Color.FromArgb(30, 30, 30);   // Testo base (Nero/Grigio scuro)

            scintilla1.StyleClearAll(); // Applica il font/sfondo a tutti gli stili di default

            // 3. Parole Chiave (Keywords)
            scintilla1.SetKeywords(0, "async await break case catch class const continue debugger default delete do else export extends false finally for function if import in instanceof let new null return super switch this throw true try typeof var void while yield");

            // Globali & Built-in (Set 1)
            scintilla1.SetKeywords(1, "Array Date JSON Math Number Object String console parseInt parseFloat undefined NaN");

            // 4. COLORAZIONE SINTASSI (Sfondo Bianco)

            // Parole Chiave (if, function, const, let...) -> Blu Scuro
            scintilla1.Styles[Style.Cpp.Word].ForeColor = Color.FromArgb(0, 0, 255);
            scintilla1.Styles[Style.Cpp.Word].Bold = true;

            // Oggetti Nativi / Metodi (Set 1) -> Blu Ottanio/Acqua
            scintilla1.Styles[Style.Cpp.Word2].ForeColor = Color.FromArgb(43, 145, 175);

            // Stringhe ("testo", 'testo') -> Rosso Scuro/Marrone
            scintilla1.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla1.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla1.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Template Literals (`)

            // Commenti (// e /* */) -> Verde Foresta
            scintilla1.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla1.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
            scintilla1.Styles[Style.Cpp.CommentDoc].ForeColor = Color.FromArgb(0, 128, 0);

            // Numeri (123, 45.67) -> Verde Scuro/Smeraldo
            scintilla1.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(9, 134, 88);

            // Operatori (+, -, =, ==, &&, ecc.) -> Grigio/Viola Scuro
            scintilla1.Styles[Style.Cpp.Operator].ForeColor = Color.FromArgb(0, 0, 0);
            scintilla1.Styles[Style.Cpp.Operator].Bold = true;

            // Identificatori (Variabili, Nomi Funzioni) -> Grigio Antracite
            scintilla1.Styles[Style.Cpp.Identifier].ForeColor = Color.FromArgb(30, 30, 30);

            // 5. MARGINI E SELEZIONE (Visuale)

            // Evidenziazione della riga corrente (Giallo tenue)
            scintilla1.CaretLineBackColor = Color.FromArgb(243, 243, 243);

            // Colore del testo e dello sfondo quando viene selezionato
            scintilla1.SelectionBackColor = Color.FromArgb(173, 214, 255);  // Blu chiaro di selezione
            // Margine della numerazione delle righe
            scintilla1.Margins[0].Width = 40;
            scintilla1.Styles[Style.LineNumber].BackColor = Color.FromArgb(240, 240, 240); // Grigio chiarissimo
            scintilla1.Styles[Style.LineNumber].ForeColor = Color.FromArgb(120, 120, 120); // Grigio medio per i numeri
        }

        public void InitControl()
        {
            CaricaDocumentazione();

            this.scintilla.IndentationGuides = IndentView.LookBoth;
            this.scintilla.Dock = DockStyle.Fill;

            this.scintilla.LexerName = this.scintilla.GetLexerIDFromLexer(Lexer.SCLEX_JAVASCRIPT);

            setScintillaStyle(this.scintilla);

            this.scintilla.Margins[0].Type = MarginType.RightText;
            this.scintilla.Margins[0].Width = 35;

            this.scintilla.DwellStart += (sender, e) =>
            {            // Trova la parola sotto il cursore del mouse
                int pos = e.Position;
                if (pos < 0) { 
                    return;
                }

                int start = this.scintilla.WordStartPosition(pos, true);
                int end = this.scintilla.WordEndPosition(pos, true);
                string word = this.scintilla.GetTextRange(start, end - start);

                // Cerca nel dizionario importato
                if (mappaDocumentazione.TryGetValue(word, out var doc))
                {
                    // Mostra il CallTip di Scintilla
                    this.scintilla.CallTipShow(pos, $"{doc.Name}\n{doc.Syntax}\n---\n{doc.Description}");
                }
            };

            this.scintilla.DwellEnd += (sender, args) =>
            {
                this.scintilla.CallTipCancel(); // Chiude il tooltip quando il mouse si sposta
            };

            UpdateLineNumbers(0);
            this.scintilla.Insert += (sender, args) =>
            {

                // Only update line numbers if the number of lines changed
                if (args.LinesAdded != 0)
                {
                    UpdateLineNumbers(scintilla.LineFromPosition(args.Position));
                }
            };

            this.scintilla.Delete += (sender, args) =>
            {

                // Only update line numbers if the number of lines changed
                if (args.LinesAdded != 0)
                {
                    UpdateLineNumbers(scintilla.LineFromPosition(args.Position));
                }
            };

            this.scintilla.CharAdded += (sender, args) =>
            {
                // Find the word start
                var currentPos = scintilla.CurrentPosition;
                var wordStartPos = scintilla.WordStartPosition(currentPos, true);

                // Display the autocompletion list
                var lenEntered = currentPos - wordStartPos;
                if (lenEntered > 0)
                {
                    if (!scintilla.AutoCActive)
                        scintilla.AutoCShow(lenEntered, paroleJavaScript);
                }

                InsertMatchedChars(args);

                // Se l'utente digita '('
                if (args.Char == '(')
                {
                    int pos = this.scintilla.CurrentPosition;
                    int wordStart = this.scintilla.WordStartPosition(pos - 1, true);
                    string functionName = this.scintilla.GetTextRange(wordStart, (pos - 1) - wordStart);

                    if (mappaDocumentazione.TryGetValue(functionName, out var doc))
                    {
                        // Mostra la sintassi della funzione subito sotto il cursore
                        this.scintilla.CallTipShow(pos, $"{doc.Syntax}\n{doc.Description}");
                    }
                }
            };

            int lastCaretPos = 0;
            this.scintilla.UpdateUI += (sender, e) =>
            {
                // Has the caret changed position?
                var caretPos = scintilla.CurrentPosition;
                if (lastCaretPos != caretPos)
                {
                    lastCaretPos = caretPos;
                    var bracePos1 = -1;
                    var bracePos2 = -1;

                    // Is there a brace to the left or right?
                    if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                        bracePos1 = (caretPos - 1);
                    else if (IsBrace(scintilla.GetCharAt(caretPos)))
                        bracePos1 = caretPos;

                    if (bracePos1 >= 0)
                    {
                        // Find the matching brace
                        bracePos2 = scintilla.BraceMatch(bracePos1);
                        if (bracePos2 == Scintilla.InvalidPosition)
                            scintilla.BraceBadLight(bracePos1);
                        else
                            scintilla.BraceHighlight(bracePos1, bracePos2);
                    }
                    else
                    {
                        // Turn off brace matching
                        scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    }
                }
            };

            #region FCTB_SETUP
            // == BRACKETS ==
            textBox.Language = Language.JS;

            textBox.AutoCompleteBrackets = true;
            textBox.AutoCompleteBracketsList = ['(', ')', '{', '}', '\"', '\"', '\'', '\''];
            textBox.BracketsHighlightStrategy = BracketsHighlightStrategy.Strategy2;
            textBox.BracketsStyle = HIGHLIGHTED_BRACKET_STYLE;
            textBox.BracketsStyle2 = HIGHLIGHTED_BRACKET_STYLE;
            textBox.LeftBracket = '(';
            textBox.RightBracket = ')';
            textBox.LeftBracket2 = '{';
            textBox.RightBracket2 = '}';
            // == INDENTATION ==
            textBox.AutoIndent = true;
            textBox.AutoIndentExistingLines = true;
            textBox.AutoIndentChars = false;
            textBox.TabLength = 4;
            // == LINE NUMBERS ==
            textBox.ShowLineNumbers = true;
            textBox.LineNumberStartValue = 1;
            textBox.LineNumberColor = Color.DarkGreen;
            // == CARET ==
            textBox.CaretVisible = true;
            textBox.CaretBlinking = true;
            textBox.ShowCaretWhenInactive = true;
            textBox.WideCaret = false;

            textBox.CharHeight = 16; //Default 14
            textBox.LineInterval = 4; //Default 0

            textBox.AcceptsTab = true;
            textBox.AcceptsReturn = true;
            textBox.ShowFoldingLines = false;

            textBox.HighlightingRangeType = HighlightingRangeType.AllTextRange;
            #endregion

            this.visualErrorHandler.Init();

            ContextMenuStrip contextMenu = new();
            ToolStripMenuItem formatCode = new("Format Code");
            formatCode.Click += (sender, args) => this.textBox.DoAutoIndentIfNeed();

            contextMenu.Items.Add(formatCode);

            this.textBox.ContextMenuStrip = contextMenu;

            this.textBox.ToolTipNeeded += (sender, args) =>
            {
                //This is checked since the VisualErrorHandler also handle stuff with tooltips to avoid problems.
                if (!string.IsNullOrEmpty(args.ToolTipText) || !string.IsNullOrEmpty(args.ToolTipText))
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

        private List<string> RetrieveJavascriptFunctions()
        {
            var engine = new Engine();

            var functions = new List<string>();

            // Accesso diretto all'oggetto Global in Jint 3.x
            var globalObj = engine.Global;

            foreach (var key in globalObj.GetOwnProperties())
            {
                string propName = key.Key.AsString();
                JsValue val = globalObj.Get(propName);

                // Controlla se è una funzione globale
                if (val.IsCallable())
                {
                    functions.Add(propName);
                }
                // Se è un oggetto (es. Math, JSON), scansiona i suoi metodi
                else if (val.IsObject() && !val.IsArray())
                {
                    var obj = val.AsObject();
                    foreach (var subKey in obj.GetOwnProperties())
                    {
                        if (subKey.Key.IsString())
                        {
                            string subPropName = subKey.Key.AsString();
                            JsValue subVal = obj.Get(subPropName);

                            if (subVal.IsCallable())
                            {
                                functions.Add($"{propName}.{subPropName}");
                            }
                        }

                    }
                }
            }

            return functions;
        }

        private IEnumerable<AutocompleteItem> CreaItemsPerMenu(List<string> funzioni)
        {
            var list = new List<AutocompleteItem>();

            foreach (var func in funzioni)
            {
                // Genera il testo inserito al click (es. "Math.floor(^)" dove ^ indica la posizione del cursore)
                string snippet = func + "(^)";

                // MethodAutocompleteItem formatta il suggerimento come un metodo
                var item = new MethodAutocompleteItem(snippet)
                {
                    Text = func,
                    MenuText = func,
                    ToolTipTitle = $"Funzione JavaScript/Jint",
                    ToolTipText = $"Inserisce {func}() nel codice."
                };

                list.Add(item);
            }

            return list;
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
            return this.textBox.Text;
        }
    }
}
