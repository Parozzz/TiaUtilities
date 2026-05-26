using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.CustomColumns.SuggestionColumn
{
    internal class SuggestionDropDown : ToolStripDropDownMenu
    {
        protected override Padding DefaultPadding
        {
            get { return Padding.Empty; }
        }

        internal SuggestionDropDown(int width, int height)
        {
            this.ShowImageMargin = ShowCheckMargin = false;
            this.RenderMode = ToolStripRenderMode.Professional;
            this.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.MaximumSize = new Size(width, height);
            this.MinimumSize = new Size(0, this.MaxItemSize.Height * 2);
            this.AllowTransparency = true;
            this.AutoClose = false;
            this.ShowItemToolTips = false; // I don't need it. Sometimes ghost tool tips remain after clicking item.
            this.CanOverflow = false;
            this.DoubleBuffered = true;
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);

            var count = this.Items.Count;
            int multiplier = Math.Min(8, count <= 4 ? (count + 1) : count);

            this.MinimumSize = new Size(0, this.MaxItemSize.Height * multiplier);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
