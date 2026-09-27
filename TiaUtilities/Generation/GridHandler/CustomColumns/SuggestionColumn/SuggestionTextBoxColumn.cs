using System.Reflection;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.CustomColumns.SuggestionColumn
{
    public class SuggestionTextBoxColumn : DataGridViewTextBoxColumn, IGridCustomColumnProcessCmdKey
    {
        public required Func<IEnumerable<string>> ItemsCallback;

        internal ToolStripDropDown? ActiveDropDown { get; set; }

        internal bool inEditMode;

        public SuggestionTextBoxColumn()
        {
            this.CellTemplate = new SuggestionTextBoxCell();
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.ActiveDropDown == null || this.ActiveDropDown.IsDisposed || !inEditMode)
            {
                return false;
            }
            
            if (keyData == Keys.Up || keyData == Keys.Down)
            {//DropDown already support scrolling with arrows! This will focus it to enable it.
                var focused = this.ActiveDropDown.Focused;
                this.ActiveDropDown.Focus();
                this.ActiveDropDown.Select();
                if (!focused) //This will avoid that the first arrow sent is skipped!
                {
                    SendKeys.SendWait(keyData == Keys.Up ? "{UP}" : "{DOWN}");
                    return true; //Only ignore the first key to allow the message to be propagated to the dropdown.
                }
            }

            return false;
        }

        internal void UpdateVisibileSuggestions(string text)
        {
            if (this.ActiveDropDown == null || this.ActiveDropDown.IsDisposed || !inEditMode)
            {
                return;
            }

            this.ActiveDropDown.SuspendLayout(); //Without this is unusable. Cursor blink and is SLLLLLOOOOOOWWWW.

            int visibleItemCount = 0;
            foreach (ToolStripItem item in this.ActiveDropDown.Items)
            {
                var itemText = item.Text;
                item.Visible = text != null && itemText != null && itemText.Contains(text, StringComparison.OrdinalIgnoreCase);// && !itemText.Equals(text, StringComparison.OrdinalIgnoreCase);
                if (item.Visible)
                {
                    visibleItemCount++;
                }
            }

            this.ActiveDropDown.MinimumSize = new Size(0, visibleItemCount <= 6 ? 25 * visibleItemCount : 0); //If there too little elements, they are not displayed correctly. This fixes it.
            this.ActiveDropDown.ResumeLayout(true);
        }
    }
}
