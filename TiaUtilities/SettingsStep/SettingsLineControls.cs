namespace TiaUtilities.SettingsStep
{
    public class SettingsLineControls
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


            public override string ToString() => $"C-{Column}, CS-{ColumnSpan}, R-{Row}, RS-{RowSpan}";
        }

        public required string Name { get; init; }

        public required PanelControl<Control> MainControl { get; init; }
        public PanelControl<Label>? Label { get; init; }
        public List<PanelControl<Button>> Buttons { get; init; } = [];
        public List<string> ContextPhrases { get; init; } = [];

        public void AddAllControls(TableLayoutPanel panel)
        {
            if (this.Label != null)
            {
                panel.Controls.Add(this.Label.Control);
            }
            panel.Controls.Add(this.MainControl.Control);
            this.Buttons.ForEach(b => panel.Controls.Add(b.Control));
        }

        public void SetAllPositionsToPanel(TableLayoutPanel panel)
        {
            this.MainControl.SetPositionToPanel(panel);
            if(this.Label != null)
            {
                this.Label.SetPositionToPanel(panel);
            }
            this.Buttons.ForEach(b => b.SetPositionToPanel(panel));
        } 
    }
}
