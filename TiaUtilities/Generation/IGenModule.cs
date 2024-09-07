using TiaUtilities.Generation.GenModules;

namespace TiaUtilities.Generation
{
    public interface IGenModule : ICleanable
    {

        public void Init(GenModuleForm form);

        public void Clear();

        public object CreateSave();

        public void LoadSave(object? saveObject);

        public void ExportXML(string folderPath);

        public Control? GetControl();

        public string GetFormLocalizatedName();

        public bool ProcessCmdKey(ref Message msg, Keys keyData); //To be passed from the form!
    }
}
