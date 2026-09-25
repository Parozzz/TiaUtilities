using static TiaUtilities.Utility.GraphicsUtils;

namespace TiaUtilities.CustomControls.tableColorizable
{
    public class TableDynamicCellStyle
    {
        public required int Column { get; init; }
        public required int StartRow { get; init; }
        
        internal int CurrentRow { get; set; } = 0;

        public int ColumnSpan { get => _columnSpan; set { _columnSpan = value; this.TableLayoutPanel?.Invalidate(); } }
        public int RowSpan { get => _rowSpan; set { _rowSpan = value; this.TableLayoutPanel?.Invalidate(); } }

        public Color BackColor { get => _backColor; set { _backColor = value; this.TableLayoutPanel?.Invalidate(); } }

        public Color BorderColor { get => _borderColor; set { _borderColor = value; this.TableLayoutPanel?.Invalidate(); } }
        public int BorderWidth { get => _borderWidth; set { _borderWidth = value; this.TableLayoutPanel?.Invalidate(); } }
        public BorderRadius BorderRadius { get => _borderRadius; set { _borderRadius = value; this.TableLayoutPanel?.Invalidate(); } }

        public Padding Padding { get => _padding; set { _padding = value; this.TableLayoutPanel?.Invalidate(); } }

        public bool FitToControls { get => _fitToControls; set { _fitToControls = value; this.TableLayoutPanel?.Invalidate(); } }

        public Rectangle Bounds { get; internal set; } = Rectangle.Empty;

        public Action<(TableDynamicCellStyle style, int oldRow, int newRow)>? RowChangedCallback { get; set; }

        private Color _backColor = Color.Transparent;
        private Color _borderColor = Color.Transparent;
        private int _borderWidth = 1;
        private BorderRadius _borderRadius = new(3);
        private int _columnSpan = 1;
        private int _rowSpan = 1;
        private bool _fitToControls = true;
        private Padding _padding = new(0);

        internal TableLayoutPanelColorizable? TableLayoutPanel { get; set; }
    }
}
