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

        private const int DefaultBorderWidth = 1;
        private const int DefaultBorderRadius = 3;
        private const int DefaultVerticalPadding = 2;
        private const int LineSpacing = 2;

        // Regex compilate una sola volta per la massima velocità
        private static readonly Regex PatternTag = new(@"<s\s*([^>]*)>(.*?)</s>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex PatternStripAllTags = new(@"<[^>]+>", RegexOptions.Compiled);
        private static readonly Regex AttributeMatchRegex = new(@"(\w+)=([#\w\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        private readonly List<List<RenderSegment>> _parsedLines = new();
        private Size _cachedPreferredSize = Size.Empty;
        private bool _isParsed = false;

        public static string Wrap(
            string text,
            Color? borderColor = null,
            Color? backColor = null,
            Color? textColor = null,
            int borderWidth = DefaultBorderWidth,
            int borderRadius = DefaultBorderRadius,
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

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (base.Text == value) return;

                base.Text = StripHtmlTags(value);
                ParseAndLayoutHtmlText(value);
                Invalidate();
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _isParsed = false; // Forza il ricalcolo al cambio font
            Invalidate();
        }

        public LabelHtmlStyle()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
        }

        // Struttura alleggerita contenente le coordinate già calcolate per il disegno
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
            public int SegmentWidth { get; set; }
            public int SegmentHeight { get; set; }
        }

        private string StripHtmlTags(string? htmlText)
        {
            if (string.IsNullOrEmpty(htmlText)) return string.Empty;
            return PatternStripAllTags.Replace(htmlText, string.Empty);
        }

        private void ParseAndLayoutHtmlText(string? htmlText)
        {
            _parsedLines.Clear();
            _isParsed = true;

            if (string.IsNullOrEmpty(htmlText))
            {
                _cachedPreferredSize = Size.Empty;
                return;
            }

            var currentLine = new List<RenderSegment>();
            int lastIndex = 0;

            foreach (Match match in PatternTag.Matches(htmlText))
            {
                if (match.Index > lastIndex)
                {
                    string rawText = htmlText[lastIndex..match.Index];
                    AddSegments(currentLine, rawText, ForeColor, Color.Transparent, Color.Transparent, 0, 0, 0, 0, 0, 0);
                }

                string attributes = match.Groups[1].Value;
                string innerText = match.Groups[2].Value;

                // Parsing veloce attributi
                var attrDict = FastParseAttributes(attributes);

                Color borderColor = GetColorAttr(attrDict, AttributeColor, Color.Transparent);
                Color backColor = GetColorAttr(attrDict, AttributeBackground, Color.Transparent);
                Color foreColor = GetColorAttr(attrDict, AttributeForeground, ForeColor);

                int borderWidth = GetIntAttr(attrDict, AttributeWidth, borderColor != Color.Transparent ? DefaultBorderWidth : 0);
                int borderRadius = GetIntAttr(attrDict, AttributeRadius, DefaultBorderRadius);

                bool hasDecoration = backColor.A > 0 || (borderColor.A > 0 && borderWidth > 0);
                int defaultVPad = hasDecoration ? DefaultVerticalPadding : 0;

                int defaultHPad = GetIntAttr(attrDict, AttributePaddingHorizontal, 0);
                int paddingLeft = GetIntAttr(attrDict, AttributePaddingLeft, defaultHPad);
                int paddingRight = GetIntAttr(attrDict, AttributePaddingRight, defaultHPad);

                int defaultVPadAttr = GetIntAttr(attrDict, AttributePaddingVertical, defaultVPad);
                int paddingTop = GetIntAttr(attrDict, AttributePaddingTop, defaultVPadAttr);
                int paddingBottom = GetIntAttr(attrDict, AttributePaddingBottom, defaultVPadAttr);

                AddSegments(currentLine, innerText, foreColor, backColor, borderColor, borderWidth, borderRadius, paddingLeft, paddingRight, paddingTop, paddingBottom);

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < htmlText.Length)
            {
                string rawText = htmlText.Substring(lastIndex);
                AddSegments(currentLine, rawText, ForeColor, Color.Transparent, Color.Transparent, 0, 0, 0, 0, 0, 0);
            }

            if (currentLine.Count > 0)
            {
                _parsedLines.Add(currentLine);
            }

            // Pre-calcolo della dimensione e del layout per evitare di farlo in OnPaint o GetPreferredSize
            CalculateLayout();
        }

        private void AddSegments(
            List<RenderSegment> currentLine, string text, Color fore, Color back, Color border,
            int bWidth, int bRadius, int pLeft, int pRight, int pTop, int pBottom)
        {
            string[] subLines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 0; i < subLines.Length; i++)
            {
                if (i > 0)
                {
                    _parsedLines.Add(new List<RenderSegment>(currentLine));
                    currentLine.Clear();
                }

                if (!string.IsNullOrEmpty(subLines[i]))
                {
                    // Misura la stringa subito durante il parsing
                    Size tSize = TextRenderer.MeasureText(subLines[i], Font, Size.Empty, TextFormatFlags.NoPadding);

                    currentLine.Add(new RenderSegment
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
                        SegmentWidth = tSize.Width + pLeft + pRight,
                        SegmentHeight = tSize.Height + pTop + pBottom
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

                foreach (var seg in line)
                {
                    lineWidth += seg.SegmentWidth;
                    if (seg.SegmentHeight > maxLineHeight)
                        maxLineHeight = seg.SegmentHeight;
                }

                totalWidth = Math.Max(totalWidth, lineWidth);
                totalHeight += maxLineHeight + LineSpacing;
            }

            _cachedPreferredSize = new Size(totalWidth, totalHeight);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (!_isParsed) return base.GetPreferredSize(proposedSize);
            return _cachedPreferredSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_parsedLines.Count == 0 || Width <= 0 || Height <= 0) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int currentY = Padding.Top;

            foreach (var line in _parsedLines)
            {
                int currentX = Padding.Left + 2; // Offset +2px per evitare tagli a sinistra
                int maxLineHeight = Font.Height;

                // Trova altezza riga
                for (int i = 0; i < line.Count; i++)
                {
                    if (line[i].SegmentHeight > maxLineHeight)
                        maxLineHeight = line[i].SegmentHeight;
                }

                // Disegna direttamente usando i valori già salvati in memoria
                for (int i = 0; i < line.Count; i++)
                {
                    var segment = line[i];

                    int segmentY = currentY + (maxLineHeight - segment.SegmentHeight) / 2;
                    Rectangle segmentRect = new(currentX, segmentY, segment.SegmentWidth, segment.SegmentHeight);

                    // 1. Sfondo
                    if (segment.BackColor.A > 0)
                    {
                        using var brush = new SolidBrush(segment.BackColor);
                        if (segment.BorderRadius > 0)
                            GraphicsUtils.FillRoundedRectangle(e.Graphics, brush, segmentRect, new(segment.BorderRadius));
                        else
                            e.Graphics.FillRectangle(brush, segmentRect);
                    }

                    // 2. Bordo
                    if (segment.BorderColor.A > 0 && segment.BorderWidth > 0)
                    {
                        float halfPen = segment.BorderWidth / 2f;
                        RectangleF borderRect = new(
                            segmentRect.X + halfPen,
                            segmentRect.Y + halfPen,
                            segmentRect.Width - segment.BorderWidth,
                            segmentRect.Height - segment.BorderWidth
                        );

                        using var pen = new Pen(segment.BorderColor, segment.BorderWidth) { Alignment = PenAlignment.Center };
                        if (segment.BorderRadius > 0)
                            GraphicsUtils.DrawRoundedRectangle(e.Graphics, pen, Rectangle.Round(borderRect), new(segment.BorderRadius));
                        else
                            e.Graphics.DrawRectangle(pen, (int)borderRect.X, (int)borderRect.Y, (int)borderRect.Width, (int)borderRect.Height);
                    }

                    // 3. Testo
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
                        TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.Left
                    );

                    currentX += segment.SegmentWidth;
                }

                currentY += maxLineHeight + LineSpacing;
            }
        }

        #region Fast Attribute Parsing
        private Dictionary<string, string> FastParseAttributes(string attributes)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in AttributeMatchRegex.Matches(attributes))
            {
                dict[m.Groups[1].Value] = m.Groups[2].Value;
            }
            return dict;
        }

        private Color GetColorAttr(Dictionary<string, string> dict, string key, Color defaultColor)
        {
            if (dict.TryGetValue(key, out var val))
            {
                try
                {
                    if (val.StartsWith("#")) return ColorTranslator.FromHtml(val);
                    return Color.FromName(val);
                }
                catch { }
            }
            return defaultColor;
        }

        private int GetIntAttr(Dictionary<string, string> dict, string key, int defaultValue)
        {
            if (dict.TryGetValue(key, out var val) && int.TryParse(val, out int res))
            {
                return res;
            }
            return defaultValue;
        }
        #endregion
    }
}