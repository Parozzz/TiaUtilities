using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.Configuration.Lines;

namespace TiaXmlReader.Generation.Configuration
{
    public interface IConfigObject
    {
        Control? GetControl();
    }

    public interface IConfigLine : IConfigObject
    {
        bool IsLabelOnTop();
        string? GetLabelText();
        Font? GetLabelFont();

        int GetHeight();
    }

    public interface IConfigGroup : IConfigObject
    {
        ConfigForm GetConfigForm();
        C Add<C>(C configObject) where C : IConfigObject;
    }
}
