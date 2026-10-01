using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Styles
{
    public static class StyleManager
    {
        public static class EditableTabControl
        {
            public readonly static Color TAB_FORE = Color.Black;
            public readonly static Color FOCUSED_TAB_FORE = Color.White;

            public readonly static Color TAB_BACK = Color.Transparent;
            public readonly static Color FOCUSED_TAB_BACK = Color.LightSlateGray;

            public static readonly Color PANEL_BORDER_COLOR = Color.LightSlateGray;

            public readonly static Color ADD_TAB_FORE_COLOR = Color.DarkOliveGreen;
        }

        public static class  Fonts
        {
            public static readonly Font BIG_BOLD = new("Segoe UI", 11.5f, FontStyle.Bold);
            public static readonly Font BIG_SEMIBOLD = new("Segoe UI Semibold", 11.5f, FontStyle.Bold);
            public static readonly Font BIG = new("Segoe UI", 11.5f, FontStyle.Regular);
                          
            public static readonly Font NORMAL_BOLD = new("Segoe UI", 9.25f, FontStyle.Bold);
            public static readonly Font NORMAL_SEMIBOLD = new("Segoe UI Semibold", 9.25f, FontStyle.Bold);
            public static readonly Font NORMAL = new("Segoe UI", 9.25f, FontStyle.Regular);
                          
            public static readonly Font SMALL_BOLD = new("Segoe UI", 7f, FontStyle.Bold);
            public static readonly Font SMALL_SEMIBOLD = new("Segoe UI Semibold", 7f, FontStyle.Bold);
            public static readonly Font SMALL = new("Segoe UI", 7f, FontStyle.Regular);
        }
    }
}
