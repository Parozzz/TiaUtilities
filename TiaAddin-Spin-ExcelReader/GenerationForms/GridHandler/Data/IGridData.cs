using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Localization;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public interface IGridData
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!
        void Clear();
        bool IsEmpty();
    }
}
