using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.TextsEditor
{
    public interface ITextsEditorExporter
    {
        public List<GenModuleEditableTextReference> GetTextsReferences();

        public void SetTextsReferences(List<GenModuleEditableTextReference> textReferences);
    }
}
