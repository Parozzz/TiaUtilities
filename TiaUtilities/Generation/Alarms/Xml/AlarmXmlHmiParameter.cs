using Newtonsoft.Json;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.Alarms.Xml
{

    public class AlarmXmlHmiParameter
    {
        public enum DisplayTypeEnum
        {
            TEXT_LIST,
            TEXT,
            BINARY,
            DECIMAL,
            FLOATING_POINT,
            HEX,
            OCTAL
        }

        public enum AlignmentEnum
        {
            LEFT,
            RIGHT
        }

        public static readonly string TYPE_PARAMETER = "AlarmParameterWithOrWithoutCommonTextList";

        [JsonIgnore] public string Type { get; init; } = TYPE_PARAMETER;

        [JsonProperty] public string Tag { get; set; } = "";
        [JsonProperty] public string TextList { get; set; } = "";

        [JsonProperty] public AlarmXmlHmiParameter.DisplayTypeEnum? DisplayType { get; set; } = DisplayTypeEnum.DECIMAL;
        [JsonProperty] public int Length { get; set; } = 2;
        [JsonProperty] public int Precision { get; set; } = 0; //DecimalPlaces

        [JsonProperty] public AlarmXmlHmiParameter.AlignmentEnum? Alignment { get; set; } = AlignmentEnum.LEFT;
        [JsonProperty] public bool ZeroPadding { get; set; } = false; //Leading Zeros


        public string GetAsString(int refId, int parameterNum)
        {
            var str = $"<ref id = {refId}; " +
                $"type = {this.Type}; " +
                $"Parameter = Parameter {parameterNum}; " +
                $"Tag = {this.Tag}; " +
                $"DisplayType = {this.GetDisplayTypeAsString()}; " +
                $"Length = {this.Length}; " +
                $"Precision = {this.Precision}; " +
                $"Alignment = {this.GetAlignmentAsString()}; " +
                $"ZeroPadding = {this.ZeroPadding.ToString().ToTitleCase()};";
            if (this.DisplayType == DisplayTypeEnum.TEXT_LIST)
            {
                str += $"TextList = {this.TextList};";
            }
            str += ">";
            return str;
        }

        private string GetDisplayTypeAsString()
        {
            return this.DisplayType switch
            {
                DisplayTypeEnum.TEXT_LIST => "TextList",
                DisplayTypeEnum.TEXT => "Text",
                DisplayTypeEnum.BINARY => "Binary",
                DisplayTypeEnum.DECIMAL => "Decimal",
                DisplayTypeEnum.FLOATING_POINT => "Float",
                DisplayTypeEnum.HEX => "HexaDecimal",
                DisplayTypeEnum.OCTAL => "Octal",
                _ => throw new InvalidDataException($"Invalid DisplayTypeEnum {this.DisplayType} for AlarmXmlHmiParameter"),
            };
        }

        private string GetAlignmentAsString()
        {
            return this.Alignment switch
            {
                AlignmentEnum.LEFT => "Left",
                AlignmentEnum.RIGHT => "Right",
                _ => throw new InvalidDataException($"Invalid AlignmentEnum {this.Alignment} for AlarmXmlHmiParameter"),
            };
        }
    }
}
