using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.Configuration
{
    public static class ConfigStyle
    {
        public static readonly Font LABEL_FONT = new(SystemFonts.DefaultFont.Name, 12.5f, FontStyle.Bold);
        public static readonly Font CONTROL_FONT = new(SystemFonts.DefaultFont.Name, LABEL_FONT.Size - 1.5f);

        public static readonly Color FORE_COLOR = SystemColors.ControlText;
        public static readonly Color BACK_COLOR = SystemColors.Control;
        public static readonly Color UNDERLINE_COLOR = Color.SlateGray;

        public static readonly Color DETAIL_COLOR_DARK = SystemColors.ControlDark;
        public static readonly Color DETAIL_COLOR_DARKDARK = SystemColors.ControlDarkDark;
    }
}
