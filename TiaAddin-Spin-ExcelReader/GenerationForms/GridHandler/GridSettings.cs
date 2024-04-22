using Newtonsoft.Json;
using System.Drawing;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public class GridSettings
    {
        [JsonProperty] public Color DragSelectedCellBorderColor = Color.DarkGreen;
        [JsonProperty] public Color SingleSelectedCellBorderColor = Color.PaleGreen;
        [JsonProperty] public Color SelectedCellTriangleColor = Color.Green;
        [JsonProperty] public Color PreviewColor = Color.MediumPurple;
        [JsonProperty] public Font GridFont = SystemFonts.DefaultFont;
    }
}
