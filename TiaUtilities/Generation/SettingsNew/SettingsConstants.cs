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

        public const float SECTIONS_LIST_VIEW_WIDTH = 250f;
        public const int SECTIONS_LEFT_PADDING = 5;

        public const float GROUP_NAME_COLUMN_SIZE = 120f;
        public const float GROUP_BORDER_COLUMN_SIZE = 6;
        public const float GROUP_SEPARATION = 12f;

        public static readonly Font SECTIONS_FONT = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static readonly Font SETTINGS_GROUP_NAME_FONT = new(SystemFonts.DefaultFont.FontFamily, 13f, FontStyle.Regular);
        public static readonly Font SETTINGS_VALUE_TITLE_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Bold);
        public static readonly Font SETTINGS_VALUE_TEXTBOX_FONT = new(SystemFonts.DefaultFont.FontFamily, 11f, FontStyle.Regular);
        public static readonly Font SETTINGS_DESCRIPTIONS_FONT = new(SystemFonts.DefaultFont.FontFamily, 8.5f, FontStyle.Regular);

    }
}
