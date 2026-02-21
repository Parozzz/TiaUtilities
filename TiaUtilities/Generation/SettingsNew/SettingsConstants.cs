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
        public static readonly Color SECTIONS_SELECTED_ITEM_BACK_COLOR = ControlPaint.LightLight(Color.LightGray);

        public const int SECTIONS_LIST_VIEW_WIDTH = 250;
        public const int SECTIONS_LIST_TILE_HEIGHT = 28;
        public const int SECTIONS_LEFT_PADDING = 5;

        public const float SECTIONS_NAME_COLUMN_SIZE = 120f;
        public const float SECTIONS_BORDER_COLUMN_SIZE = 6;
        public const float SECTIONS_SEPARATION = 12f;

        public const float SECTION_VALUE_SEPERATION = 8f;

        public static readonly Font LIST_LEFT_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);

        public static readonly Font MACROSECTION_NAME_LABEL_FONT = new(SystemFonts.DefaultFont.FontFamily, 23f, FontStyle.Bold);
        public static readonly Font SECTION_NAME_LABEL_FONT = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static readonly Font VALUE_NAME_LABEL_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
        public static readonly Font VALUE_CONTROL_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);
        public static readonly Font DESCRIPTION_LABEL_FONT = new(SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);

    }
}
