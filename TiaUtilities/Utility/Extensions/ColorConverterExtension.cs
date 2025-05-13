namespace TiaXmlReader.Utility.Extensions
{
    public static class ColorConverterExtension
    {
        // #RRGGBB
        public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        // RGB(R, G, B)
        public static string ToRgbString(this Color c) => $"RGB({c.R}, {c.G}, {c.B})";

        // #RRGGBBAA
        public static string ToHexaString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";

        private static double ToProportion(byte b) => b / (double)Byte.MaxValue;

        // RGBA(R, G, B, A)
        public static string ToRgbaString(this Color c) => $"RGBA({c.R}, {c.G}, {c.B}, {ToProportion(c.A):N2})";
    }
}
