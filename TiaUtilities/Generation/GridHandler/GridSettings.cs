using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaUtilities.Generation.Configuration;

namespace TiaUtilities.Generation.GridHandler
{
    public class GridSettings : ObservableConfiguration
    {
        [JsonProperty] public Color SelectedCellBackColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color SelectedCellForeColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color DragDropStartCellSelectedBackColor { get => this.GetAs<Color>(); set => this.Set(value); }

        [JsonProperty] public int BorderWeight { get => this.GetAs<int>(); set => this.Set(value); }
        [JsonProperty] public Color DragSelectedCellBorderColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color SingleSelectedCellBorderColor { get => this.GetAs<Color>(); set => this.Set(value); }

        [JsonProperty] public Color SelectedCellTriangleColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color PreviewColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Font GridFont { get => this.GetAs<Font>(); set => this.Set(value); }

        public GridSettings()
        {
            this.SelectedCellBackColor = Color.LightGray;
            this.SelectedCellForeColor = Color.Black;
            this.DragDropStartCellSelectedBackColor = Color.Honeydew;

            this.BorderWeight = 2;
            this.DragSelectedCellBorderColor = Color.FromArgb(0x40, 0x80, 0x80);
            this.SingleSelectedCellBorderColor = Color.Blue; 

            this.SelectedCellTriangleColor = Color.Green;
            this.PreviewColor = Color.MediumPurple;
            this.GridFont = SystemFonts.DefaultFont;
        }
    }
}
