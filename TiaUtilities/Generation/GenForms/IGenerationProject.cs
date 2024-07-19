using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GenForms.Alarm;

namespace TiaUtilities.Generation.GenForms
{
    public interface IGenerationProject
    {

        public void Init(GenerationProjectForm form);

        public IGenerationProjectSave CreateProjectSave();

        public IGenerationProjectSave? Load(ref string? filePath);

        public void ExportXML(string folderPath);

        public Control? GetTopControl();

        public Control? GetBottomControl();

        public string GetFormLocalizatedName();

        public bool ProcessCmdKey(ref Message msg, Keys keyData); //To be passed from the form!
    }
}
