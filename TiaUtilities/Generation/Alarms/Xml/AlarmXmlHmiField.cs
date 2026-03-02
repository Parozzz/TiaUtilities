using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Xml
{

    public class AlarmXmlHmiField(string tag, string alignment = "Left")
    {
        public static readonly string TYPE_PARAMETER = "AlarmParameterWithOrWithoutCommonTextList";

        public static readonly string ALIGNMENT_LEFT = "Left";
        public static readonly string ALIGNMENT_RIGHT = "Right";

        public static readonly string DISPLAY_TYPE_TEXT_LIST = "TextList";
        public static readonly string DISPLAY_TYPE_TEXT = "Text";
        public static readonly string DISPLAY_TYPE_BINARY = "Binary";
        public static readonly string DISPLAY_TYPE_DECIMAL = "Decimal";
        public static readonly string DISPLAY_TYPE_FLOATING_POINT = "Float";
        public static readonly string DISPLAY_TYPE_HEX = "HexaDecimal";
        public static readonly string DISPLAY_TYPE_OCTAL = "Octal";

        public string Type { get; init; } = TYPE_PARAMETER;

        public string Tag { get; init; } = tag;
        public string? TextList { get; set; }

        public string DisplayType { get; set; } = DISPLAY_TYPE_DECIMAL;
        public int Length { get; set; } = 0;
        public int Precision { get; set; } = 0; //DecimalPlaces

        public string Alignment { get; set; } = alignment; //Left - Right
        public bool ZeroPadding { get; set; } = false; //Leading Zeros

        public string GetAsString(int refId, int parameterNum)
        {
            var str = $"<ref id = {refId}; " +
                $"type = {this.Type}; " +
                $"Parameter = Parameter {parameterNum}; " +
                $"Tag = {Tag}; " +
                $"Length = {Length}; " +
                $"Precision = {Precision}; " +
                $"Alignment = {Alignment}; " +
                $"ZeroPadding = {ZeroPadding.ToString().ToTitleCase()}";
            if (this.DisplayType == DISPLAY_TYPE_TEXT_LIST)
            {
                str += $"TextList = {TextList};";
            }
            str += ">";
            return str;
        }
    }
}
