using System.Diagnostics;
using System.Runtime.InteropServices;
using TiaUtilities.CustomControls;
using TiaUtilities.CustomControls.tableColorizable;
using TiaUtilities.Styles;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep
{
    public class SettingsSearchPanel
    {
        private const int CONTEXT_LABEL_COLUMN = 1;
        private const int CONTROL_COLUMN = 2;

        private const string CONTEXT_DIVIDER_SYMBOL = "↦";

        private readonly TableLayoutPanelColorizable panel;
        private readonly List<SettingsSequencePanelLine> searchLines;

        private string actualSearchText = "";

        public SettingsSearchPanel(TableLayoutPanelColorizable panel)
        {
            this.panel = panel;
            this.searchLines = [];
        }

        public void InitPanel()
        {
            this.actualSearchText = "";

            this.panel.SuspendLayout();

            this.panel.RowStyles.Clear();
            this.panel.ColumnStyles.Clear();
            this.panel.Controls.Clear();

            this.panel.ColumnCount = 4;
            this.panel.ColumnStyles.Add(new(SizeType.Percent, 100f));
            this.panel.ColumnStyles.Add(new(SizeType.AutoSize)); //Main Label
            this.panel.ColumnStyles.Add(new(SizeType.AutoSize)); //Control
            this.panel.ColumnStyles.Add(new(SizeType.Percent, 100f));

            this.panel.ResumeLayout();
        }

        public void Clear()
        {
            this.actualSearchText = "";

            this.panel.SuspendLayout();

            this.panel.RowStyles.Clear();
            this.panel.ClearCellStyles();
            this.panel.Controls.Clear();

            this.searchLines.Clear();

            this.panel.ResumeLayout();
        }

        public void UpdateSearchText(List<SettingsSequence> sequences, string searchText)
        {
            if (searchText == actualSearchText)
            {
                return;
            }

            actualSearchText = searchText;

            Cursor.Current = Cursors.WaitCursor;

            this.panel.SuspendLayout();
            DllImports.SuspendDrawing(this.panel);

            this.panel.Controls.Clear();
            this.panel.RowStyles.Clear();
            this.panel.ClearCellStyles();

            this.searchLines.Clear(); //Since i don't add anything particular to the ContextLabel, i do not need to dispose of it. It should be done automagically.

            var sequencesLines = SettingsSearchPanel.GetLines(sequences.SelectMany(s => s.Panels).SelectMany(p => p.Lines), searchText).ToList();

            int rowCounter = 0;
            foreach (var sequenceLine in sequencesLines)
            {
                if (sequenceLine.Name == "GroupLabel" || sequenceLine.Name == "Divider")
                {
                    continue;
                }

                var context = String.Join($" {CONTEXT_DIVIDER_SYMBOL} ", sequenceLine.ContextPhrases.Where(str => !string.IsNullOrWhiteSpace(str)));

                var contextLabel = SettingsSearchPanel.CreateContextLabel(context, sequenceLine.Name);
                SettingsSequencePanelLine searchLine = new()
                {
                    Name = sequenceLine.Name,
                    Main = new(sequenceLine.Main.Control) { Column = CONTROL_COLUMN, Row = rowCounter },
                    Label = new(contextLabel) { Column = CONTEXT_LABEL_COLUMN, Row = rowCounter }
                };

                this.searchLines.Add(searchLine);
                rowCounter++;
            }

            this.panel.SetDynamicCellStyle(new()
            {
                Column = CONTEXT_LABEL_COLUMN,
                ColumnSpan = CONTROL_COLUMN - CONTEXT_LABEL_COLUMN + 1,
                StartRow = 0,
                BackColor = Color.AntiqueWhite, // Color.AntiqueWhite,
                BorderColor = Color.Transparent,
                BorderWidth = 0,
                FitToControls = true,
                Padding = Padding.Empty,
                RowChangedCallback = args =>
                {
                    if (sequencesLines.TryGet(args.oldRow, out var oldLine))
                    {
                        oldLine.Main.Control.BackColor = Form.DefaultBackColor;
                    }

                    if (sequencesLines.TryGet(args.newRow, out var newLine))
                    {
                        newLine.Main.Control.BackColor = Color.AntiqueWhite;
                    }
                }
            });

            Enumerable.Range(0, rowCounter)
                .Select(i => new RowStyle(SizeType.AutoSize))
                .ForEach(rs => this.panel.RowStyles.Add(rs));

            this.searchLines.ForEach(l => l.AddToPanel(this.panel));

            this.panel.ResumeLayout();
            DllImports.ResumeDrawing(this.panel);

            Cursor.Current = Cursors.Default;
        }

        public void DisposeControls()
        {
            this.searchLines.ForEach(l => l.DisposeAll());
        }

        private static IEnumerable<SettingsSequencePanelLine> GetLines(
            IEnumerable<SettingsSequencePanelLine> lines,
            string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return [];
            }

            // Dividiamo il testo di ricerca in parole individuali (ignorando spazi multipli)
            var searchWords = searchText
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (searchWords.Length == 0) {
                return [];
            }

            return lines.Where(item =>
            {
                string[] phrases = [.. item.ContextPhrases, item.Name];

                var wordIndex = 0;
                foreach (var phrase in phrases)
                {
                    var word = searchWords[wordIndex];
                    if(phrase.Contains(word, StringComparison.OrdinalIgnoreCase))
                    {
                        wordIndex++;
                    }

                    if(wordIndex >= searchWords.Length)
                    {
                        break;
                    }
                }

                return wordIndex >= searchWords.Length;
            }).OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase);
        }

        private static Label CreateContextLabel(string context, string text)
        {
            context = LabelHtmlStyle.Wrap(context,
                    borderColor: Color.FromArgb(100, Color.Black),
                    backColor: Color.Transparent,
                    textColor: Color.DimGray,
                    borderWidth: 1,
                    borderRadius: 3,
                    padding: new(5, 2, 5, 3)
            );

            return new LabelHtmlStyle()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,

                Font = StyleManager.Fonts.NORMAL,

                FlatStyle = FlatStyle.Flat,

                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None,

                Text = $"{context} {text}",
                TextAlign = ContentAlignment.MiddleCenter,

                Padding = Padding.Empty,
                Margin = new(1),
            };
        }
    }
}
