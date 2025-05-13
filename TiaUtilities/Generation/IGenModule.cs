namespace TiaUtilities.Generation
{
    public interface IGenModule : ICleanable, ISaveable<object>
    {

        public void Init(GenModuleForm form);

        public void Clear();

        public void ExportXML(string folderPath);

        public Control? GetControl();

        public string GetFormLocalizatedName();

        public bool ProcessCmdKey(ref Message msg, Keys keyData); //To be passed from the form!
    }
}
