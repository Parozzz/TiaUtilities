using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Generation.GenForms.IO;
using TiaXmlReader.Generation;

namespace TiaUtilities.Generation.GenForms
{
    public interface IGenerationProjectSave
    {
        public bool Populate(ref string? filePath);
        public bool Save(ref string? filePath, bool showFileDialog = false);
    }
}
