using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms;
using TiaUtilities.Generation.Configuration.Utility;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Languages;

namespace TiaXmlReader.Generation.GridHandler
{
    public class GridSettings : ISettingsAutoSave
    {
        [JsonProperty] public Color DragSelectedCellBorderColor = Color.FromArgb(0x40, 0x80, 0x80);
        [JsonProperty] public Color SingleSelectedCellBorderColor = Color.LightYellow;
        [JsonProperty] public Color SelectedCellTriangleColor = Color.Green;
        [JsonProperty] public Color PreviewColor = Color.MediumPurple;
        [JsonProperty] public Font GridFont = SystemFonts.DefaultFont;

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
