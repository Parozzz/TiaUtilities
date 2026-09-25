using System.Diagnostics;
using System.Runtime.InteropServices;
using TiaUtilities.CustomControls.tableColorizable;
using TiaUtilities.SettingsStep.CustomControls;
using TiaUtilities.Styles;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep
{
    public class SettingsSearchPanelControls
    {
        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void SuspendDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);
        }

        public static void ResumeDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
            control.Refresh();
        }

        private const int CONTEXT_LABEL_COLUMN = 1;
        private const int CONTROL_COLUMN = 2;

        private static readonly char[] WordSeparators = [' ', ',', '.', ';', ':', '-', '_', '!', '?'];

        private readonly TableLayoutPanelColorizable panel;
        private readonly List<SettingsStepSequence> sequences;
        private readonly List<SettingsLineControls> searchLines;

        private string actualSearchText = "";

        public SettingsSearchPanelControls(TableLayoutPanelColorizable panel, List<SettingsStepSequence> sequences)
        {
            this.panel = panel;
            this.sequences = sequences;
            this.searchLines = [];
        }

        public void InitPanel()
        {
            this.actualSearchText = "";

            this.panel.SuspendLayout();

            this.Clear();

            this.panel.ColumnCount = 5;
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

            this.panel.Controls.Clear();

            this.panel.RowCount = 0;
            this.panel.RowStyles.Clear();

            this.panel.ColumnCount = 0;
            this.panel.ColumnStyles.Clear();

            this.panel.ClearCellStyles();

            this.panel.ResumeLayout();
        }

        public void UpdateSearchText(string searchText)
        {
            if(searchText == actualSearchText)
            {
                return;
            }

            actualSearchText = searchText;

            Cursor.Current = Cursors.WaitCursor;

            this.panel.SuspendLayout();
            SuspendDrawing(this.panel);

            this.panel.Visible = false;

            this.panel.ClearCellStyles();

            this.searchLines.Clear();

            var lines = SettingsSearchPanelControls.GetLines(this.sequences.SelectMany(s => s.PanelControls).SelectMany(p => p.Lines), searchText).ToList();

            int rowCounter = 0;
            foreach(var line in lines)
            {
                if(line.Name == "GroupLabel" || line.Name == "Divider")
                {
                    continue;
                }

                var context = String.Join(" ↦ ", line.ContextPhrases.Where(str => !string.IsNullOrWhiteSpace(str)));
                context = LabelHtmlStyle.Wrap(context, 
                    borderColor: Color.FromArgb(100, Color.Black),
                    backColor: Color.Transparent, 
                    textColor: Color.DimGray, 
                    borderWidth: 1, 
                    borderRadius: 3,
                    padding: new(5 ,2, 5, 3)
                );

                var lineLabelText = String.Join(" ", line.Label?.Control.Text ?? "");

                var contextLabel = CreateContextLabel($"{context} {lineLabelText}");
                SettingsLineControls searchLine = new()
                {
                    Name = line.Name,
                    MainControl = new(line.MainControl.Control) { Column = CONTROL_COLUMN, Row = rowCounter },
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
                    if(lines.TryGet(args.oldRow, out var oldLine))
                    {
                        oldLine.MainControl.Control.BackColor = Form.DefaultBackColor;
                    }

                    if(lines.TryGet(args.newRow, out var newLine))
                    {
                        newLine.MainControl.Control.BackColor = Color.AntiqueWhite;
                    }
                }
            });

            this.panel.RowStyles.Clear();
            Enumerable.Range(0, rowCounter)
                .Select(i => new RowStyle(SizeType.AutoSize))
                .ForEach(rs => this.panel.RowStyles.Add(rs));

            this.panel.Controls.Clear();
            this.searchLines.ForEach(l => l.AddAllControls(this.panel));
            this.searchLines.ForEach(l => l.SetAllPositionsToPanel(this.panel));

            this.panel.Visible = true;

            this.panel.ResumeLayout();
            ResumeDrawing(this.panel);

            Cursor.Current = Cursors.Default;
        }

        private static IEnumerable<SettingsLineControls> GetLines(
            IEnumerable<SettingsLineControls> lines,
            string searchText)
        {
            if(string.IsNullOrWhiteSpace(searchText))
            {
                return [];
            }
            
            // Dividiamo il testo di ricerca in parole individuali (ignorando spazi multipli)
            var searchWords = searchText
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return lines.Where(item =>
                // "Tutte le parole..."
                searchWords.All(word =>
                    // "...devono trovarsi nel Name O in almeno una ContextPhrase"
                    item.Name.Contains(word, StringComparison.OrdinalIgnoreCase) ||
                    item.ContextPhrases.Any(phrase => phrase.Contains(word, StringComparison.OrdinalIgnoreCase))
                )
            ).OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase);
        }

        private static Label CreateContextLabel(string text)
        {
            return new LabelHtmlStyle()
            {
                BackColor = Color.Transparent,

                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Font = StyleManager.Fonts.NORMAL,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
            };
            /*
            return new LabelColorizable()
            {
                BackColor = Form.DefaultBackColor,
                HoverColor = Form.DefaultBackColor,
                ClickedColor = Form.DefaultBackColor,

                BorderColor = ControlPaint.Dark(Color.DarkGray, 0.2f),
                BorderWidth = 1,

                Anchor = AnchorStyles.Right,
                AutoSize = true,
                Font = StyleManager.Fonts.NORMAL,
                FlatStyle = FlatStyle.Flat,
                BorderStyle = BorderStyle.None,
                Text = text,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
            };*/
        }
    }
}
