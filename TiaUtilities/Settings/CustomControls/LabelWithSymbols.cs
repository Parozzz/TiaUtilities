using System.Collections.ObjectModel;
using System.Drawing.Text;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class LabelWithSymbols : Label
    {
        private static readonly ToolTip ToolTip = ControlUtils.CreateToolTip(longAutoPop: true, quick: true, fading: true);

        public class SymbolItem
        {
            public required string Symbol { get; init; }
            public required Func<string> TooltipTextCallback { get; set; }

            public Color Color { get => this._color; set { this._color = value; this.Owner?.Invalidate(); } }
            public Color HoverColor { get => this._hoverColor; set { this._hoverColor = value; this.Owner?.Invalidate(); } }
            public Color ClickedColor { get => this._clickedColor; set { this._clickedColor = value; this.Owner?.Invalidate(); } }
            public int ClickedColorDuration { get; set; } = 0; //ms

            public Action<SymbolItem>? OnClick { get; init; }
            public Action<SymbolItem>? OnHover { get; init; }

            internal Rectangle Bounds { get; set; } = Rectangle.Empty;
            internal bool IsHovered { get => this._isHovered; set { this._isHovered = value; this.Owner?.Invalidate(); } }

            internal bool UseClickedColor { get => this._useClickedColor; set { this._useClickedColor = value; this.Owner?.Invalidate(); } }

            internal Control? Owner { get; set; }

            private Color _color = Form.DefaultForeColor;
            private Color _hoverColor = Form.DefaultForeColor;
            private Color _clickedColor = Form.DefaultForeColor;

            private bool _isHovered = false;
            private bool _useClickedColor = false;

            public void ShowTooltip()
            {
                if(this.Owner == null)
                {
                    return;
                }

                if (this.IsHovered)
                {
                    var text = this.TooltipTextCallback();
                    LabelWithSymbols.ToolTip.SetToolTip(this.Owner, text);
                }
                else
                {
                    LabelWithSymbols.ToolTip.SetToolTip(this.Owner, null);
                }
            }

            public Size Measure(Font font)
            {
                SizeF size = TextRenderer.MeasureText(this.Symbol, font, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.NoPadding);
                return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
            }

            public Size MeasureWithGraphics(Graphics g, Font font)
            {
                SizeF size = g.MeasureString(this.Symbol, font);
                return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
            }
        }

        public enum ItemsPosition
        {
            Left,
            Right
        }

        public ObservableCollection<SymbolItem> Items { get; init; } = [];

        public ItemsPosition SymbolsPosition { get; set; } = ItemsPosition.Right;
        public int SymbolsGap { get; set; } = 3;
        public int SymbolsGapFromText { get; set; } = 2;
        public float SymbolSizePercent { get; set; } = 0.85f;

        public new Color BackColor { get; set; } = Color.Transparent;

        public LabelWithSymbols()
        {
            base.BackColor = Color.Transparent;
            this.DoubleBuffered = true;

            this.Items.CollectionChanged += (sender, args) =>
            {
                args.OldItems?.Cast<SymbolItem>().ForEach(i => i.Owner = null);
                args.NewItems?.Cast<SymbolItem>().ForEach(i => i.Owner = this);

                this.UpdateControlLayout();
            };
            this.TextChanged += (sender, args) => this.UpdateControlLayout();
        }

        private void UpdateControlLayout()
        {
            if (!this.AutoSize)
            {
                // Forzi manualmente la dimensione ideale calcolata da GetPreferredSize
                this.Size = this.GetPreferredSize(Size.Empty);
            }

            this.Invalidate();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size baseSize = base.GetPreferredSize(proposedSize);
            if (this.Items.Count == 0)
            {
                return baseSize;
            }

            using var unicodeFont = this.GetSymbolIconFont();

            var symbolsWidth = 0;
            var symbolsHeight = 0;
            foreach (var symbol in this.Items)
            {
                var size = symbol.Measure(unicodeFont);
                symbolsWidth += size.Width + this.SymbolsGap;
                symbolsHeight = Math.Max(symbolsHeight, size.Height);
            }

            return new Size(baseSize.Width + symbolsWidth, Math.Max(baseSize.Height, symbolsHeight));
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Attiva l'antialiasing per rendere i simboli Unicode definiti e puliti
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var symbolFont = this.GetSymbolIconFont();
            var textSize = TextRenderer.MeasureText(e.Graphics, this.Text, this.Font, this.Size, this.GetFlags());

            // 1. Calcola la larghezza totale e l'altezza massima di tutti i simboli
            int totalSymbolsWidth = 0;
            int maxSymbolHeight = 0;
            List<Size> symbolSizes = new(this.Items.Count);

            foreach (var symbol in this.Items)
            {
                var size = symbol.MeasureWithGraphics(e.Graphics, symbolFont);
                symbolSizes.Add(size);

                totalSymbolsWidth += size.Width;
                maxSymbolHeight = Math.Max(maxSymbolHeight, size.Height);
            }

            int totalSpacing = symbolSizes.Count * this.SymbolsGap;
            int totalBlockWidth = textSize.Width + totalSymbolsWidth + totalSpacing;

            int textX = 0;
            int symbolsStartX = 0;

            // 2. Calcola le coordinate X iniziali per Testo e Blocco Simboli in base alla posizione
            if (this.SymbolsPosition == ItemsPosition.Left)
            {
                if (this.TextAlign == ContentAlignment.TopLeft ||
                    this.TextAlign == ContentAlignment.MiddleLeft ||
                    this.TextAlign == ContentAlignment.BottomLeft)
                {
                    symbolsStartX = 0;
                    textX = totalSymbolsWidth + totalSpacing;
                }
                else if (this.TextAlign == ContentAlignment.TopCenter ||
                         this.TextAlign == ContentAlignment.MiddleCenter ||
                         this.TextAlign == ContentAlignment.BottomCenter)
                {
                    symbolsStartX = (this.Width - totalBlockWidth) / 2;
                    textX = symbolsStartX + totalSymbolsWidth + totalSpacing;
                }
                else // Right
                {
                    symbolsStartX = this.Width - totalBlockWidth;
                    textX = symbolsStartX + totalSymbolsWidth + totalSpacing;
                }
            }
            else // ItemsPosition.Right
            {
                if (this.TextAlign == ContentAlignment.TopLeft ||
                    this.TextAlign == ContentAlignment.MiddleLeft ||
                    this.TextAlign == ContentAlignment.BottomLeft)
                {
                    textX = 0;
                    symbolsStartX = textSize.Width + this.SymbolsGap;
                }
                else if (this.TextAlign == ContentAlignment.TopCenter ||
                         this.TextAlign == ContentAlignment.MiddleCenter ||
                         this.TextAlign == ContentAlignment.BottomCenter)
                {
                    textX = (this.Width - totalBlockWidth) / 2;
                    symbolsStartX = textX + textSize.Width + this.SymbolsGap;
                }
                else // Right
                {
                    textX = this.Width - totalBlockWidth;
                    symbolsStartX = this.Width - (totalSymbolsWidth + totalSpacing);
                }
            }

            // 3. Disegna il testo principale
            Rectangle textRect = new(textX, 0, textSize.Width, this.Height);
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textRect, this.ForeColor, this.BackColor, this.GetFlags());

            int currentSymbolX = symbolsStartX;
            for (int i = 0; i < this.Items.Count; i++)
            {
                var symbol = this.Items[i];
                Size currentSize = symbolSizes[i];

                int symbolY = (this.Height - currentSize.Height) / 2;
                symbol.Bounds = new Rectangle(currentSymbolX, symbolY, currentSize.Width, currentSize.Height);

                var drawColor = symbol.Color;
                if (symbol.UseClickedColor)
                {
                    drawColor = symbol.ClickedColor;
                }
                else if (symbol.IsHovered)
                {
                    drawColor = symbol.HoverColor;
                }

                using SolidBrush brush = new(drawColor);
                e.Graphics.DrawString(symbol.Symbol, symbolFont, brush, currentSymbolX, symbolY);

                // Avanza la coordinata X per il simbolo successivo
                currentSymbolX += currentSize.Width + this.SymbolsGap;
            }
        }

        private Font GetSymbolIconFont()
        {
            return new Font("Segoe UI Symbol", this.Font.Size * this.SymbolSizePercent, FontStyle.Regular);
        }

        private TextFormatFlags GetFlags()
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding;

            // Allineamento Verticale
            if (this.TextAlign == ContentAlignment.MiddleLeft ||
                this.TextAlign == ContentAlignment.MiddleCenter ||
                this.TextAlign == ContentAlignment.MiddleRight)
            {
                flags |= TextFormatFlags.VerticalCenter;
            }
            else if (this.TextAlign == ContentAlignment.BottomLeft ||
                     this.TextAlign == ContentAlignment.BottomCenter ||
                     this.TextAlign == ContentAlignment.BottomRight)
            {
                flags |= TextFormatFlags.Bottom;
            }
            else
            {
                flags |= TextFormatFlags.Top;
            }

            return flags;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.Items.Count == 0)
            {
                return;
            }

            foreach (var item in this.Items)
            {
                var contains = item.Bounds.Contains(e.Location);

                if (contains != item.IsHovered)
                {
                    item.IsHovered = contains;
                    if (item.IsHovered)
                    {
                        item.OnHover?.Invoke(item);
                    }

                    item.ShowTooltip();
                }

                if(contains)
                { //If one is contained, others are not necessary to be checked.
                    break;
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Items
                .Where(i => i.Bounds.Contains(e.Location))
                .ForEach(async i =>
                {
                    i.OnClick?.Invoke(i);

                    if (i.ClickedColorDuration > 0 && !i.UseClickedColor)
                    {
                        i.UseClickedColor = true;

                        await Task.Delay(i.ClickedColorDuration);

                        i.UseClickedColor = false;
                        i.ShowTooltip();
                    }
                });
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.Items.Where(s => s.IsHovered).ForEach(s => s.IsHovered = false);
            LabelWithSymbols.ToolTip.SetToolTip(this, null);
        }
    }
}

/*
protected override void OnPaint(PaintEventArgs e)
{
    // Attiva l'antialiasing per rendere i simboli Unicode definiti e puliti
    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

    using Font symbolIcon = this.GetSymbolIcon();
    Size textSize = TextRenderer.MeasureText(e.Graphics, this.Text, this.Font, this.Size, this.GetFlags());
    Size symbolSize = this.MeasureSymbol(e.Graphics, symbolIcon);

    int textX = 0;
    int symbolX = 0;
    int totalWidth = textSize.Width + symbolSize.Width + this.SymbolSpacing;

    // Calcolo X basato sulla posizione richiesta (Left / Right)
    if (this.SymbolsPosition == ItemsPosition.Left)
    {
        if (this.TextAlign == ContentAlignment.TopLeft ||
            this.TextAlign == ContentAlignment.MiddleLeft ||
            this.TextAlign == ContentAlignment.BottomLeft)
        {
            symbolX = 0;
            textX = symbolSize.Width + this.SymbolSpacing;
        }
        else if (this.TextAlign == ContentAlignment.TopCenter ||
                 this.TextAlign == ContentAlignment.MiddleCenter ||
                 this.TextAlign == ContentAlignment.BottomCenter)
        {
            symbolX = (this.Width - totalWidth) / 2;
            textX = symbolX + symbolSize.Width + this.SymbolSpacing;
        }
        else // Right
        {
            symbolX = this.Width - totalWidth;
            textX = symbolX + symbolSize.Width + this.SymbolSpacing;
        }
    }
    else // TooltipSymbolPosition.Right
    {
        if (this.TextAlign == ContentAlignment.TopLeft ||
            this.TextAlign == ContentAlignment.MiddleLeft ||
            this.TextAlign == ContentAlignment.BottomLeft)
        {
            textX = 0;
            symbolX = textSize.Width + this.SymbolSpacing;
        }
        else if (this.TextAlign == ContentAlignment.TopCenter ||
                 this.TextAlign == ContentAlignment.MiddleCenter ||
                 this.TextAlign == ContentAlignment.BottomCenter)
        {
            textX = (this.Width - totalWidth) / 2;
            symbolX = textX + textSize.Width + this.SymbolSpacing;
        }
        else // Right
        {
            textX = this.Width - totalWidth;
            symbolX = this.Width - symbolSize.Width;
        }
    }

    Rectangle textRect = new(textX, 0, textSize.Width, this.Height);
    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textRect, this.ForeColor, this.BackColor, this.GetFlags());

    if (!string.IsNullOrEmpty(this.TooltipText))
    {
        this.questionRect = new(symbolX, 0, symbolSize.Width, symbolSize.Height);

        using Brush brush = new SolidBrush(_isHoveringQuestionOld ? this.SymbolHoverColor : this.SymbolColor);
        e.Graphics.DrawString(this.TooltipSymbol, symbolIcon, brush, symbolX, 0);
    }
}
*/