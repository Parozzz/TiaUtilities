namespace TiaUtilities.Generation.GenModules
{
    public interface IGenModule : ICleanable
    {

        public void Init(GenModuleForm form);

        public object CreateSave();

        public void LoadSave(object? saveObject);

        public void ExportXML(string folderPath);

        public Control? GetTopControl();

        public Control? GetBottomControl();

        public string GetFormLocalizatedName();

        public bool ProcessCmdKey(ref Message msg, Keys keyData); //To be passed from the form!
    }
}
