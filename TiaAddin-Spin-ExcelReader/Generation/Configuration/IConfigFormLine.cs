
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public interface IConfigFormLine
    {
        string GetLabelText();

        Control GetControl();
    }
}
