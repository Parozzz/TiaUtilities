using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Editors
{
    public class EditorOptions
    {
        public bool ShowLineNumbers { get; init; } = true;
        public bool ShowErrorIndicator { get; init; } = true;

        public ScintillaNET.BorderStyle? BorderStyle { get; init; } = null;
        public Color? BackColor { get; init; } = null;
        public Color? ForeColor { get; init; } = null;
    }
}
