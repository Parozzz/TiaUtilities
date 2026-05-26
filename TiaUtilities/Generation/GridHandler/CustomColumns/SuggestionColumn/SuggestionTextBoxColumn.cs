using System.Reflection;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.CustomColumns.SuggestionColumn
{
    public class SuggestionTextBoxColumn : DataGridViewTextBoxColumn, IGridCustomColumnProcessCmdKey
    {
        internal ToolStripDropDown? dropDown;
        internal Func<IEnumerable<string?>>? GetItemsFunc;

        internal bool inEditMode;

        public SuggestionTextBoxColumn()
        {
            this.CellTemplate = new SuggestionTextBoxCell();
        }

        public void SetGetItemsFunc(Func<IEnumerable<string?>> GetItemsFunc)
        {
            this.GetItemsFunc = GetItemsFunc;
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.dropDown == null || !inEditMode)
            {
                return false;
            }

            if (keyData == Keys.Up || keyData == Keys.Down)
            {
                //DropDown already support scrolling with arrows! This will focus it to enable it.
                var focused = dropDown.Focused;
                dropDown.Focus();
                if (!focused) //This will avoid that the first arrow sent is skipped!
                {
                    SendKeys.SendWait(keyData == Keys.Up ? "{UP}" : "{DOWN}");
                }
                return true;
            }

            return false;
        }


        internal void UpdateVisibileSuggestions(string text)
        {
            if (this.dropDown == null || !inEditMode)
            {
                return;
            }

            this.dropDown.SuspendLayout(); //Without this is unusable. Cursor blink and is SLLLLLOOOOOOWWWW.

            int visibleItemCount = 0;
            foreach (ToolStripItem item in this.dropDown.Items)
            {
                var itemText = item.Text;
                item.Visible = text != null && itemText != null && itemText.Contains(text, StringComparison.OrdinalIgnoreCase);// && !itemText.Equals(text, StringComparison.OrdinalIgnoreCase);
                if (item.Visible)
                {
                    visibleItemCount++;
                }
            }

            this.dropDown.MinimumSize = new Size(0, visibleItemCount <= 6 ? 25 * visibleItemCount : 0); //If there too little elements, they are not displayed correctly. This fixes it.
            this.dropDown.ResumeLayout(true);
        }
    }
}
