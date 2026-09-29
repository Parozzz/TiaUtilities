using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TiaUtilities.Utility;

namespace TiaUtilities.SettingsStep.CustomControls
{
    public class LabelHtmlStyle : Label
    {
        #region Constants & Fast Regex (Compiled)
        private const string AttributeColor = "c";
        private const string AttributeBackground = "bg";
        private const string AttributeForeground = "fg";
        private const string AttributeWidth = "w";
        private const string AttributeRadius = "r";
        private const string AttributePaddingLeft = "pl";
        private const string AttributePaddingRight = "pr";
        private const string AttributePaddingTop = "pt";
        private const string AttributePaddingBottom = "pb";
        private const string AttributePaddingHorizontal = "p";
        private const string AttributePaddingVertical = "pv";

        private const int DEFAULT_BORDER_WIDTH = 1;
        private const int DEFAULT_BORDER_RADIUS = 3;
        private const int DEFAULT_VERTICAL_PADDING = 2;
        private const int LINE_SPACING = 2;

        // Regex compilate una sola volta per la massima velocità
        private static readonly Regex PatternTagRegex = new(@"<s\s*([^>]*)>(.*?)</s>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex PatternRegex = new(@"<[^>]+>", RegexOptions.Compiled);
        private static readonly Regex AttributeMatchRegex = new(@"(\w+)=([#\w\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static string StripHtmlTags(string? htmlText) => string.IsNullOrEmpty(htmlText) ? "" : PatternRegex.Replace(htmlText, string.Empty);
        #endregion

        public static string Wrap(
            string text,
            Color? borderColor = null,
            Color? backColor = null,
            Color? textColor = null,
            int borderWidth = DEFAULT_BORDER_WIDTH,
            int borderRadius = DEFAULT_BORDER_RADIUS,
            Padding? padding = null)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            Padding pad = padding ?? Padding.Empty;

            string borderAttr = borderColor.HasValue && borderColor.Value.A > 0 ? $"{AttributeColor}={ColorToHex(borderColor.Value)}" : string.Empty;
            string bgAttr = backColor.HasValue && backColor.Value.A > 0 ? $" {AttributeBackground}={ColorToHex(backColor.Value)}" : string.Empty;
            string fgAttr = textColor.HasValue && textColor.Value.A > 0 ? $" {AttributeForeground}={ColorToHex(textColor.Value)}" : string.Empty;
            string widthAttr = borderWidth > 0 ? $" {AttributeWidth}={borderWidth}" : string.Empty;
            string radiusAttr = borderRadius > 0 ? $" {AttributeRadius}={borderRadius}" : string.Empty;

            string plAttr = pad.Left > 0 ? $" {AttributePaddingLeft}={pad.Left}" : string.Empty;
            string ptAttr = pad.Top > 0 ? $" {AttributePaddingTop}={pad.Top}" : string.Empty;
            string prAttr = pad.Right > 0 ? $" {AttributePaddingRight}={pad.Right}" : string.Empty;
            string pbAttr = pad.Bottom > 0 ? $" {AttributePaddingBottom}={pad.Bottom}" : string.Empty;

            return $"<s {borderAttr}{bgAttr}{fgAttr}{widthAttr}{radiusAttr}{plAttr}{ptAttr}{prAttr}{pbAttr}>{text}</s>";
        }

        private static string ColorToHex(Color color)
        {
            return color.A < 255
                ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private class RenderSegment
        {
            public string Text { get; set; } = string.Empty;
            public Color ForeColor { get; set; }
            public Color BackColor { get; set; }
            public Color BorderColor { get; set; }
            public int BorderWidth { get; set; }
            public int BorderRadius { get; set; }
            public int PaddingLeft { get; set; }
            public int PaddingTop { get; set; }

            // Layout calcolato una volta sola
            public Size TextSize { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }
        }

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (value == null)
                {
                    this._htmlText = "";
                    base.Text = "";

                    this._parsedLines.Clear();
                    this._cachedPreferredSize = Size.Empty;

                    return;
                }
                else if (_htmlText == value)
                {
                    return;
                }

                this._htmlText = value;
                base.Text = LabelHtmlStyle.StripHtmlTags(value);

                this.ParseAndLayoutHtmlText();
                Invalidate();
            }
        }

        private readonly List<List<RenderSegment>> _parsedLines = [];
        private Size _cachedPreferredSize = Size.Empty;
        private string _htmlText = string.Empty;

        public LabelHtmlStyle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            base.BackColor = Color.Transparent;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            this.ParseAndLayoutHtmlText();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.ParseAndLayoutHtmlText();
            Invalidate();
        }

        private void ParseAndLayoutHtmlText()
        {
            _parsedLines.Clear();

            if (string.IsNullOrEmpty(this._htmlText))
            {
                _cachedPreferredSize = Size.Empty;
                return;
            }

            var currentLine = new List<RenderSegment>();
            int lastIndex = 0;

            foreach (Match match in PatternTagRegex.Matches(this._htmlText))
            {
                if (match.Index > lastIndex)
                {
                    string rawText = this._htmlText[lastIndex..match.Index];
                    AddSegments(currentLine, rawText, ForeColor, Color.Transparent, Color.Transparent, 0, 0, 0, 0, 0, 0);
                }

                string attributes = match.Groups[1].Value;
                string innerText = match.Groups[2].Value;

                // Parsing veloce attributi
                var attrDict = FastParseAttributes(attributes);

                Color borderColor = GetColorAttr(attrDict, AttributeColor, Color.Transparent);
                Color backColor = GetColorAttr(attrDict, AttributeBackground, Color.Transparent);
                Color foreColor = GetColorAttr(attrDict, AttributeForeground, ForeColor);

                int borderWidth = GetIntAttr(attrDict, AttributeWidth, borderColor != Color.Transparent ? DEFAULT_BORDER_WIDTH : 0);
                int borderRadius = GetIntAttr(attrDict, AttributeRadius, DEFAULT_BORDER_RADIUS);

                bool hasDecoration = backColor.A > 0 || (borderColor.A > 0 && borderWidth > 0);
                int defaultVPad = hasDecoration ? DEFAULT_VERTICAL_PADDING : 0;

                int defaultHPad = GetIntAttr(attrDict, AttributePaddingHorizontal, 0);
                int paddingLeft = GetIntAttr(attrDict, AttributePaddingLeft, defaultHPad);
                int paddingRight = GetIntAttr(attrDict, AttributePaddingRight, defaultHPad);

                int defaultVPadAttr = GetIntAttr(attrDict, AttributePaddingVertical, defaultVPad);
                int paddingTop = GetIntAttr(attrDict, AttributePaddingTop, defaultVPadAttr);
                int paddingBottom = GetIntAttr(attrDict, AttributePaddingBottom, defaultVPadAttr);

                this.AddSegments(currentLine, innerText, foreColor, backColor, borderColor, borderWidth, borderRadius, paddingLeft, paddingRight, paddingTop, paddingBottom);

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < this._htmlText.Length)
            {
                string rawText = this._htmlText.Substring(lastIndex);
                this.AddSegments(currentLine, rawText, ForeColor, Color.Transparent, Color.Transparent, 0, 0, 0, 0, 0, 0);
            }

            if (currentLine.Count > 0)
            {
                _parsedLines.Add(currentLine);
            }

            // Pre-calcolo della dimensione e del layout per evitare di farlo in OnPaint o GetPreferredSize
            this.CalculateLayout();
        }

        private void AddSegments(
            List<RenderSegment> segments, string text, Color fore, Color back, Color border,
            int bWidth, int bRadius, int pLeft, int pRight, int pTop, int pBottom)
        {
            string[] subLines = text.Split(["\r\n", "\n"], StringSplitOptions.None);

            for (int i = 0; i < subLines.Length; i++)
            {
                if (i > 0)
                {
                    _parsedLines.Add([.. segments]);
                    segments.Clear();
                }

                if (!string.IsNullOrEmpty(subLines[i]))
                {
                    // Misura la stringa subito durante il parsing
                    Size tSize = TextRenderer.MeasureText(subLines[i], this.Font, Size.Empty, TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding);

                    segments.Add(new RenderSegment
                    {
                        Text = subLines[i],
                        ForeColor = fore,
                        BackColor = back,
                        BorderColor = border,
                        BorderWidth = bWidth,
                        BorderRadius = bRadius,
                        PaddingLeft = pLeft,
                        PaddingTop = pTop,
                        TextSize = tSize,
                        Width = tSize.Width + pLeft + pRight,
                        Height = tSize.Height + pTop + pBottom
                    });
                }
            }
        }

        private void CalculateLayout()
        {
            int totalWidth = 0;
            int totalHeight = Padding.Top + Padding.Bottom;

            foreach (var line in _parsedLines)
            {
                int lineWidth = Padding.Left + Padding.Right + 4; // +4px margine di sicurezza
                int maxLineHeight = Font.Height;

                foreach (var segment in line)
                {
                    lineWidth += segment.Width;
                    if (segment.Height > maxLineHeight)
                    {
                        maxLineHeight = segment.Height;
                    }
                }

                totalWidth = Math.Max(totalWidth, lineWidth);
                totalHeight += maxLineHeight + LINE_SPACING;
            }

            this._cachedPreferredSize = new Size(totalWidth, totalHeight);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (_cachedPreferredSize == Size.Empty)
            {
                return base.GetPreferredSize(proposedSize);
            }

            int width = _cachedPreferredSize.Width;
            int height = _cachedPreferredSize.Height;
            if (proposedSize.Width > 0 && proposedSize.Width > width)
            {//If a proposed with is bigger, i keep it.
                width = proposedSize.Width;
            }

            if (proposedSize.Height > 0 && proposedSize.Height > height)
            {
                height = proposedSize.Height;
            }

            return new Size(width, height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_parsedLines.Count == 0 || base.Width <= 0 || base.Height <= 0)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int totalContentHeight = 0;

            List<int> lineHeights = new(this._parsedLines.Count);
            foreach (var line in this._parsedLines)
            {
                int maxLineHeight = Font.Height;
                foreach (var segment in line)
                {
                    if (segment.Height > maxLineHeight)
                    {
                        maxLineHeight = segment.Height;
                    }
                }
                lineHeights.Add(maxLineHeight);
                totalContentHeight += maxLineHeight;
            }

            if (this._parsedLines.Count > 1)
            {
                totalContentHeight += LINE_SPACING * (this._parsedLines.Count - 1);
            }

            // 2. Calcolo dell'Offset Y iniziale in base all'Allineamento Verticale
            int startY = Padding.Top;
            int availableHeight = Height - Padding.Top - Padding.Bottom;
            if (availableHeight > totalContentHeight)
            {
                switch (base.TextAlign)
                {
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleRight:
                        startY = Padding.Top + (availableHeight - totalContentHeight) / 2;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomRight:
                        startY = Height - Padding.Bottom - totalContentHeight;
                        break;
                }
            }

            int currentY = startY;
            foreach (var line in this._parsedLines)
            {
                int currentX = Padding.Left + 2; // Offset +2px per evitare tagli a sinistra
                int maxLineHeight = Math.Max(line.Max(l => l.Height), base.Font.Height);

                // Disegna direttamente usando i valori già salvati in memoria
                foreach (var segment in line)
                {
                    int segmentY = currentY + (maxLineHeight - segment.Height) / 2;
                    Rectangle segmentRect = new(currentX, segmentY, segment.Width, segment.Height);

                    // 1. Background
                    if (segment.BackColor.A > 0)
                    {
                        using var brush = new SolidBrush(segment.BackColor);
                        GraphicsUtils.FillRoundedRectangle(e.Graphics, brush, segmentRect, new(segment.BorderRadius));
                    }

                    // 2. Border
                    if (segment.BorderColor.A > 0 && segment.BorderWidth > 0)
                    {
                        float halfPen = segment.BorderWidth / 2f;
                        Rectangle borderRect = Rectangle.Round(
                            new(
                                segmentRect.X + halfPen,
                                segmentRect.Y + halfPen,
                                segmentRect.Width - segment.BorderWidth,
                                segmentRect.Height - segment.BorderWidth
                            )
                        );

                        using var pen = new Pen(segment.BorderColor, segment.BorderWidth) { Alignment = PenAlignment.Center };
                        GraphicsUtils.DrawRoundedRectangle(e.Graphics, pen, borderRect, new(segment.BorderRadius));
                    }

                    // 3. Text
                    Rectangle textRect = new(
                        currentX + segment.PaddingLeft,
                        segmentY + segment.PaddingTop,
                        segment.TextSize.Width + 2, // +2 tolleranza per caratteri speciali
                        segment.TextSize.Height
                    );

                    TextRenderer.DrawText(
                        e.Graphics,
                        segment.Text,
                        Font,
                        textRect,
                        segment.ForeColor,
                        TextFormatFlags.NoClipping | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.Left
                    );

                    currentX += segment.Width;
                }

                currentY += maxLineHeight + LINE_SPACING;
            }
        }

        #region Fast Attribute Parsing
        private static Dictionary<string, string> FastParseAttributes(string attributes)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in AttributeMatchRegex.Matches(attributes))
            {
                dict[m.Groups[1].Value] = m.Groups[2].Value;
            }
            return dict;
        }

        private static Color GetColorAttr(Dictionary<string, string> dict, string key, Color defaultColor)
        {
            if (dict.TryGetValue(key, out var val))
            {
                try
                {
                    return val.StartsWith('#') ? ColorTranslator.FromHtml(val) : Color.FromName(val);
                }
                catch { }
            }

            return defaultColor;
        }

        private static int GetIntAttr(Dictionary<string, string> dict, string key, int defaultValue)
        {
            return dict.TryGetValue(key, out var val) && int.TryParse(val, out int res) ? res : defaultValue;
        }
        #endregion
    }
}