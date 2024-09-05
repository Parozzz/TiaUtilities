namespace TiaUtilities.Generation.GenForms
{
    public interface IGenProject
    {

        public void Init(GenProjectForm form);

        public bool IsDirty(bool clear = false);

        public IGenProjectSave CreateSave();

        public IGenProjectSave? LoadSave(ref string? filePath);

        public void ExportXML(string folderPath);

        public Control? GetTopControl();

        public Control? GetBottomControl();

        public string GetFormLocalizatedName();

        public bool ProcessCmdKey(ref Message msg, Keys keyData); //To be passed from the form!
    }
}
