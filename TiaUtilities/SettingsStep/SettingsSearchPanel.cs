using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.CustomControls;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;

namespace TiaUtilities.SettingsStep
{
    public class SettingsSearchPanel
    {
        private static readonly char[] WordSeparators = [' ', ',', '.', ';', ':', '-', '_', '!', '?'];

        private readonly List<SettingsStepSequence> sequences;
        private readonly List<SettingsLineControls> lines;
        private readonly TableLayoutPanelColorizable panel;


        public SettingsSearchPanel(List<SettingsStepSequence> sequences)
        {
            this.sequences = sequences;
            this.lines = [];

            this.panel = this.InitControls();
        }

        private TableLayoutPanelColorizable InitControls()
        {
            TableLayoutPanelColorizable panel = new()
            {
                Dock = DockStyle.Top,
                ColumnCount = 5,
                ColumnStyles =
                {
                    new(SizeType.Percent, 100f),
                    new(SizeType.AutoSize), //Context label
                    new(SizeType.AutoSize), //Main Label
                    new(SizeType.AutoSize), //Control
                    new(SizeType.Percent, 100f)
                }
            };

            return panel;
        }

        public void UpdateSearchText(string searchText)
        {
            const int CONTEXT_LABEL_COLUMN = 1;
            const int LABELS_COLUMN = 2;
            const int CONTROL_COLUMN = 3;

            this.panel.Controls.Clear();

            var lines = SettingsSearchPanel.GetLines(this.sequences.SelectMany(s => s.PanelControls).SelectMany(p => p.Lines), searchText, 999);

            List<SettingsLineControls> searchLines = [];

            int rowIndex = 0;
            foreach(var line in lines)
            {
                var context = $"[{String.Join(" - ", line.Keyphrases)}]";

                var contextLabel = CreateContextLabel(context);

                SettingsLineControls searchLine = new()
                {
                    MainControl = new(line.MainControl.Control) { Column = CONTROL_COLUMN , Row = rowIndex }
                };

                searchLine.Labels.Add(new(contextLabel) { Column = CONTEXT_LABEL_COLUMN, Row = rowIndex });
                foreach (var label in line.Labels)
                {
                    searchLine.Labels.Add(new(label.Control) { Column = LABELS_COLUMN, Row = rowIndex } );
                }

                searchLines.Add(searchLine);

                rowIndex++;
            }

            this.panel.Controls.AddRange([.. searchLines.SelectMany(l => l.GetAllControls())]);

            foreach(var searchLine in searchLines)
            {
                searchLine.Labels.ForEach(l => l.SetPositionToPanel(this.panel));
                searchLine.MainControl.SetPositionToPanel(this.panel);
            }

        }

        private static IEnumerable<SettingsLineControls> GetLines(
            IEnumerable<SettingsLineControls> lines,
            string searchText, 
            int maxResults = 0)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return [];
            }

            // 1. Estrae le parole uniche dalla ricerca (in minuscolo)
            HashSet<string> searchWords = searchText
                .Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => w.ToLowerInvariant())
                .ToHashSet();

            if (searchWords.Count == 0){
                return [];
            }

            // 2. Calcola lo score per ogni LineControls e ordina per punteggio decrescente
            var scoredResults = lines
                .Select(lc => new
                {
                    Item = lc,
                    // Per ogni Keyphrase conta quante parole della ricerca sono presenti, poi prende il punteggio massimo
                    MaxScore = lc.Keyphrases.Count == 0 ? 0 : lc.Keyphrases.Max(kp => GetMatchScore(kp, searchWords))
                })
                .Where(x => x.MaxScore > 0)          // Scarta chi non ha nessuna corrispondenza
                .OrderByDescending(x => x.MaxScore)   // Prima chi ha più parole in comune
                .Select(x => x.Item);

            return maxResults > 0 ? scoredResults.Take(maxResults) : scoredResults;
        }

        private static int GetMatchScore(string keyphrase, HashSet<string> searchWords)
        {
            var kpWords = keyphrase
                .Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => w.ToLowerInvariant());

            // Conta quante parole distinte della keyphrase coincidono con quelle della ricerca
            return kpWords.Distinct().Count(searchWords.Contains);
        }

        private static Label CreateContextLabel(string text)
        {
            var emptyText = string.IsNullOrEmpty(text);
            return new LabelColorizable()
            {
                BackColor = Form.DefaultBackColor,
                HoverColor = Form.DefaultBackColor,
                ClickedColor = Form.DefaultBackColor,

                BorderColor = ControlPaint.Dark(Color.DarkGray, 0.2f),
                BorderWidth = emptyText ? 0 : 1,

                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                //Dock = DockStyle.Fill,
                AutoSize = true,
                Font = StyleManager.Fonts.BIG_BOLD,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = emptyText ? Padding.Empty : new(5, 2, 5, 2),
                Margin = emptyText ? Padding.Empty : new(2),
            };
        }
    }
}
