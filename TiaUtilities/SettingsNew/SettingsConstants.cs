using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.SettingsNew
{
    public static class SettingsConstants
    {
        public static int BUTTONS_SIZE { get; } = 30;

        public static Color SECTIONS_ITEM_FORE_COLOR { get; } = Color.Black;
        public static Color SECTIONS_SELECTED_ITEM_BACK_COLOR { get; } = ControlPaint.LightLight(Color.LightGray);

        public static int SECTIONS_LIST_VIEW_WIDTH { get; } = 290;
        public static int SECTIONS_LIST_TILE_HEIGHT { get; } = 26;
        public static int SECTIONS_LEFT_PADDING { get; } = 5;

        public static float SECTIONS_NAME_COLUMN_SIZE { get; } = 150f;
        public static float SECTIONS_BORDER_COLUMN_SIZE { get; } = 6;
        public static float SECTIONS_SEPARATION { get; } = 12f;

        public static float SECTIONS_SEPERATION { get; } = 12f;
        public static float VALUES_SEPERATION { get; } = 8f;

        public static bool DEFAULT_VALUE_DESCRIPTION_VISIBILITY { get; } = true;
        public static int VALUE_DESCRIPTION_MAX_SIZE { get; } = 400;

        public static Font LIST_LEFT_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);

        public static Font MACROSECTION_NAME_LABEL_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 23f, FontStyle.Bold);
        public static Font SECTION_NAME_LABEL_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static Font VALUE_NAME_LABEL_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
        public static Font VALUE_CONTROL_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);
        public static Font DESCRIPTION_LABEL_FONT { get; } = new(SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);

    }
}
