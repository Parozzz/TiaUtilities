using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.SettingsNew
{
    public static class SettingsConstants
    {
        public static readonly Color SECTIONS_ITEM_FORE_COLOR = Color.Black;
        public static readonly Color SECTIONS_SELECTED_ITEM_BACK_COLOR = Color.FromArgb(60, Color.Gray);

        public static readonly Color SECTIONS_SELECTED_ITEM_BORDER_COLOR = Color.FromArgb(125, Color.Gray);
        public const int SECTIONS_SELECTED_ITEM_BORDER_WIDTH = 0;

        public const int SECTIONS_LIST_VIEW_WIDTH = 250;
        public const int SECTIONS_LIST_TILE_HEIGHT = 28;
        public const int SECTIONS_LEFT_PADDING = 5;

        public const float SECTIONS_NAME_COLUMN_SIZE = 120f;
        public const float SECTIONS_BORDER_COLUMN_SIZE = 6;
        public const float SECTIONS_SEPARATION = 12f;

        public static readonly Font SECTIONS_LEFT_FONT = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static readonly Font SECTIONS_RIGHT_NAME_FONT = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static readonly Font VALUE_TITLE_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
        public static readonly Font VALUE_TEXTBOX_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);
        public static readonly Font DESCRIPTIONS_FONT = new(SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);

    }
}
