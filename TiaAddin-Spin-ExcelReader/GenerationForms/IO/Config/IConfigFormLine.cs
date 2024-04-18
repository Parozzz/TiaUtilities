
using System.Windows.Forms;

namespace TiaXmlReader.GenerationForms.IO.Config
{
    public interface IConfigFormLine
    {
        string GetLabelText();

        Control GetControl();
    }
}
