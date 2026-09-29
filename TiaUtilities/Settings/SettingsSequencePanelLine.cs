namespace TiaUtilities.SettingsStep
{
    public class SettingsSequencePanelLine
    {

        public class PanelControl<T>(T control) where T : Control
        {
            public T Control { get; init; } = control;

            public required int Column { get; init; }
            public required int Row { get; init; }
            public int ColumnSpan { get; set; } = 0;
            public int RowSpan { get; set; } = 0;

            public void SetPositionToPanel(TableLayoutPanel panel)
            {
                panel.SetCellPosition(this.Control, new(this.Column, this.Row));
                if (this.ColumnSpan > 0)
                {
                    panel.SetColumnSpan(this.Control, this.ColumnSpan);
                }

                if (this.RowSpan > 0)
                {
                    panel.SetRowSpan(this.Control, this.RowSpan);
                }
            }

            public void Dispose()
            {
                if(!this.Control.IsDisposed)
                {
                    this.Control.Dispose();
                }
            }


            public override string ToString() => $"C-{Column}, CS-{ColumnSpan}, R-{Row}, RS-{RowSpan}";
        }

        public required string Name { get; init; }

        public required PanelControl<Control> Main { get; init; }
        public PanelControl<Label>? Label { get; init; }
        public List<string> ContextPhrases { get; init; } = [];

        public void DisposeAll()
        {
            this.Main.Dispose();
            this.Label?.Dispose();
        }

        public void AddToPanel(TableLayoutPanel panel, bool setPositionOnly = false)
        {
            if (!setPositionOnly)
            {
                panel.Controls.Add(this.Main.Control);
                if (this.Label != null)
                {
                    panel.Controls.Add(this.Label.Control);
                }
            }

            this.Main.SetPositionToPanel(panel);
            this.Label?.SetPositionToPanel(panel);
        }
    }
}
