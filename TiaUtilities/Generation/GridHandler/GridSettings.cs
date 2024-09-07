using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridSettings : ObservableConfiguration
    {
        [JsonProperty] public Color DragSelectedCellBorderColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color SingleSelectedCellBorderColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color SelectedCellTriangleColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Color PreviewColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public Font GridFont { get => this.GetAs<Font>(); set => this.Set(value); }

        public GridSettings()
        {
            this.DragSelectedCellBorderColor = Color.FromArgb(0x40, 0x80, 0x80);
            this.SingleSelectedCellBorderColor = Color.Blue;
            this.SelectedCellTriangleColor = Color.Green;
            this.PreviewColor = Color.MediumPurple;
            this.GridFont = SystemFonts.DefaultFont;
        }

        public void ShowConfigForm(IWin32Window owner)
        {
            var configForm = new ConfigForm(Localization.Get("GRID_PREFERENCES")) { ControlWidth = 300 };

            var mainGroup = configForm.Init();
            mainGroup.AddColorPicker().LocalizedLabel("GRID_PREFERENCES_SELECTED_CELL_BORDER")
                .ApplyColor(this.SingleSelectedCellBorderColor)
                .ColorChanged(color => this.SingleSelectedCellBorderColor = color);

            mainGroup.AddColorPicker().LocalizedLabel("GRID_PREFERENCES_DRAGGED_CELL_BACK")
                 .ApplyColor(this.DragSelectedCellBorderColor)
                 .ColorChanged(color => this.DragSelectedCellBorderColor = color);

            mainGroup.AddColorPicker().LocalizedLabel("GRID_PREFERENCES_DRAG_TRIANGLE_COLOR")
                 .ApplyColor(this.SelectedCellTriangleColor)
                 .ColorChanged(color => this.SelectedCellTriangleColor = color);

            mainGroup.AddColorPicker().LocalizedLabel("GRID_PREFERENCES_PREVIEW_FORE")
                 .ApplyColor(this.PreviewColor)
                 .ColorChanged(color => this.PreviewColor = color);

            configForm.StartShowingAtLocation(Cursor.Position);
            configForm.Init();
            configForm.Show(owner);
        }
    }
}
