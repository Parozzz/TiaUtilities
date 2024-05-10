using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.Configuration
{
    public interface IConfigObject
    {
        Control GetControl();
    }

    public interface IConfigLine : IConfigObject
    {
        bool IsLabelOnTop();
        string GetLabelText();
        Font GetLabelFont(); //NULLABLE

        int GetHeight();
    }

    public interface IConfigGroup : IConfigObject
    {

    }
}
