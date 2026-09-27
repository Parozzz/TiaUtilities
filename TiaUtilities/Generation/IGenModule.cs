using TiaUtilities.Generation.TextsEditor;

namespace TiaUtilities.Generation
{
    public interface IGenModule : ICleanable, ISaveable<object>, ITextsEditorExporter
    {

        public class ModuleControl
        {
            public required string Name { get; init; }
            public required Func<Control> RequestControlCallback { get; init; }
        }

        public List<ModuleControl> ModuleControls { get; init; }

        public void Init(GenModuleForm form);

        public void Clear();

        public void ExportXML(string folderPath);

        public string GetFormLocalizatedName();

        public void OpenPlaceholderViewer(IWin32Window? window = null);

        public void ShowSettings();

    }
}
