using System.Collections.ObjectModel;
using System.Drawing.Text;
using TiaUtilities.Utility;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class LabelWithTooltip : Label
    {

        public class LabelItem
        {
            public required string Symbol { get; init; }
            public required string TooltipText { get; set; }
            public required Color Color { get; set; }
            public required Color HoverColor { get; set; }
            public Action<LabelItem>? OnClick { get; init; }
            public Action<LabelItem>? OnHover { get; init; }


            internal Rectangle Bounds { get; set; }
            internal bool IsHovered { get; set; }
            internal bool IsHoveredOld { get; set; }

            public Size Measure(Graphics g, Font font)
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

        public ObservableCollection<LabelItem> Symbols { get; init; } = [];
        public int SymbolSpacing { get; set; } = 3;
        public int DistanceFromText { get; set; } = 2;
        public ItemsPosition SymbolsPosition { get; set; } = ItemsPosition.Right;

        public new Color BackColor { get; set; } = Color.Transparent;

        private readonly ToolTip toolTip = ControlUtils.CreateToolTip(longAutoPop: true, quick: true, fading: true);

        public LabelWithTooltip()
        {
            base.BackColor = Color.Transparent;
            this.DoubleBuffered = true;

            this.Symbols.CollectionChanged += (sender, args) => this.UpdateControlLayout();
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
            if (this.Symbols.Count == 0)
            {
                return baseSize;
            }

            using var g = this.CreateGraphics();
            using var unicodeFont = this.GetSymbolIcon();

            var symbolsWidth = 0;
            var symbolsHeight = 0;
            foreach (var symbol in this.Symbols)
            {
                var size = symbol.Measure(g, unicodeFont);
                symbolsWidth += size.Width + this.SymbolSpacing;
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

            using var symbolFont = this.GetSymbolIcon();
            var textSize = TextRenderer.MeasureText(e.Graphics, this.Text, this.Font, this.Size, this.GetFlags());

            // 1. Calcola la larghezza totale e l'altezza massima di tutti i simboli
            int totalSymbolsWidth = 0;
            int maxSymbolHeight = 0;
            List<Size> symbolSizes = new(this.Symbols.Count);

            foreach (var symbol in this.Symbols)
            {
                var size = symbol.Measure(e.Graphics, symbolFont);
                symbolSizes.Add(size);

                totalSymbolsWidth += size.Width;
                maxSymbolHeight = Math.Max(maxSymbolHeight, size.Height);
            }

            int totalSpacing = symbolSizes.Count * this.SymbolSpacing;
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
                    symbolsStartX = textSize.Width + this.SymbolSpacing;
                }
                else if (this.TextAlign == ContentAlignment.TopCenter ||
                         this.TextAlign == ContentAlignment.MiddleCenter ||
                         this.TextAlign == ContentAlignment.BottomCenter)
                {
                    textX = (this.Width - totalBlockWidth) / 2;
                    symbolsStartX = textX + textSize.Width + this.SymbolSpacing;
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
            for (int i = 0; i < this.Symbols.Count; i++)
            {
                var symbol = this.Symbols[i];
                Size currentSize = symbolSizes[i];

                int symbolY = (this.Height - currentSize.Height) / 2;
                symbol.Bounds = new Rectangle(currentSymbolX, symbolY, currentSize.Width, currentSize.Height);

                var drawColor = symbol.IsHovered ? symbol.HoverColor : symbol.Color;

                using SolidBrush brush = new(drawColor);
                e.Graphics.DrawString(symbol.Symbol, symbolFont, brush, currentSymbolX, symbolY);

                // Avanza la coordinata X per il simbolo successivo
                currentSymbolX += currentSize.Width + this.SymbolSpacing;
            }
        }

        private Font GetSymbolIcon()
        {
            return new Font("Segoe UI Symbol", this.Font.Size * 0.85f, FontStyle.Regular); // StyleManager.Fonts.SMALL_BOLD;
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
            if (this.Symbols.Count == 0)
            {
                return;
            }


            foreach (var symbol in this.Symbols)
            {
                symbol.IsHovered = symbol.Bounds.Contains(e.Location);
                if (symbol.IsHovered != symbol.IsHoveredOld)
                {
                    symbol.IsHoveredOld = symbol.IsHovered;

                    this.Invalidate(); //Redraw text
                    if (symbol.IsHovered)
                    {
                        symbol.OnHover?.Invoke(symbol);

                        var text = symbol.TooltipText;
                        if(!string.IsNullOrEmpty(text))
                        {
                            toolTip.SetToolTip(this, text);
                        }
                    }
                    else
                    {
                        toolTip.SetToolTip(this, null);
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Symbols.Where(s => s.Bounds.Contains(e.Location)).ForEach(s => s.OnClick?.Invoke(s));
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Symbols.Count == 0)
            {
                return;
            }

            foreach (var symbol in this.Symbols)
            {
                if (symbol.IsHovered)
                {
                    symbol.IsHovered = false;
                    symbol.IsHoveredOld = false;
                    this.Invalidate();

                    toolTip.SetToolTip(this, null);
                }
            }
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