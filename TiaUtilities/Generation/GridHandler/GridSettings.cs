using Newtonsoft.Json;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaXmlReader.Generation.Configuration;

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
            var configForm = new ConfigForm(Locale.GRID_PREFERENCES, this) { ControlWidth = 300 };

            var mainGroup = configForm.Init();
            mainGroup.AddColorPicker().Label(Locale.GRID_PREFERENCES_SELECTED_CELL_BORDER).BindColor(() => this.SingleSelectedCellBorderColor);
            mainGroup.AddColorPicker().Label(Locale.GRID_PREFERENCES_DRAGGED_CELL_BACK).BindColor(() => this.DragSelectedCellBorderColor);
            mainGroup.AddColorPicker().Label(Locale.GRID_PREFERENCES_DRAG_TRIANGLE_COLOR).BindColor(() => this.SelectedCellTriangleColor);
            mainGroup.AddColorPicker().Label(Locale.GRID_PREFERENCES_PREVIEW_FORE).BindColor(() => this.PreviewColor);

            configForm.StartShowingAtLocation(Cursor.Position);
            configForm.Init();
            configForm.Show(owner);
        }
    }
}
