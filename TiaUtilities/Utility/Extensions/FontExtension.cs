namespace TiaUtilities.Utility.Extensions
{
    public static class FontExtension
    {
        public static Font Copy(this Font font, float size)
        {
            return new Font(font.Name, size, font.Style);
        }

        public static Font Copy(this Font font, float size, FontStyle fontStyle)
        {
            return new Font(font.Name, size, fontStyle);
        }
    }
}
