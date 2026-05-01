using System.Reflection;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.CustomColumns
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


    public class SuggestionTextBoxCell : DataGridViewTextBoxCell
    {
        private EventHandler? editorControlTextChangedEvent;

        private DataGridViewTextBoxEditingControl? GetEditorControl()
        {
            var editingTextBoxProperty = typeof(DataGridViewTextBoxCell).GetProperty("EditingTextBox", BindingFlags.NonPublic | BindingFlags.Instance);
            if (editingTextBoxProperty == null)
            {
                return null;
            }

            return (DataGridViewTextBoxEditingControl?) editingTextBoxProperty.GetValue(this);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (this.DataGridView is not DataGridView dataGridView ||  rowIndex < 0 ||  this.ColumnIndex < 0)
            {
                return;
            }

            if(this.OwningColumn is not SuggestionTextBoxColumn column || column.GetItemsFunc == null)
            {
                return;
            }

            if(column.dropDown != null)
            {
                return;
            }

            DataGridViewTextBoxEditingControl? editingControl = this.GetEditorControl();
            if (editingControl == null)
            {
                return;
            }

            var items = column.GetItemsFunc().Where(v => v != null);
            if (items == null || !items.Any())
            {
                return;
            }

            column.inEditMode = true;

            column.UpdateVisibileSuggestions(text: "");
            this.editorControlTextChangedEvent = (sender, args) => column.UpdateVisibileSuggestions(editingControl.Text);

            var showPoint = dataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, false).Location;
            showPoint.Y += this.Size.Height;

            var height = 400;
            if ((showPoint.Y + height) > dataGridView.Height)
            {
                height = Math.Min(height, dataGridView.Height - showPoint.Y);
            }

            var dropDown = new SuggestionDropDown(this.Size.Width, height);
            dropDown.Items.AddRange(items.Select(i => new ToolStripMenuItem() { Text = i }).ToArray()); //THIS IS WAAAAY FASTER! Without AddRange is super slow.
            dropDown.KeyDown += (sender1, args1) =>
            {//This is cool. If i start tipying again i want to trasnfer to the editing control so the mouse is not needed!
                if (editingControl == null || editingControl.Focused)
                {
                    return;
                }

                if (args1.KeyData == Keys.Enter || args1.KeyData == Keys.Up || args1.KeyData == Keys.Down)
                {
                    return;
                }

                args1.Handled = true;

                editingControl.FindForm()?.Focus();
                editingControl.Focus();
                //This trasfer the key data from the DropDown to the editing control.
                DllImports.PostMessage(editingControl.Handle, DllImports.WM_KEYDOWN, (int)args1.KeyData, 0);
            };

            dropDown.ItemClicked += (sender1, args1) =>
            {
                if (editingControl == null || this.DataGridView == null || args1.ClickedItem == null)
                {
                    return;
                }

                editingControl.Text = args1.ClickedItem.Text;
                this.DataGridView.EndEdit();
            };
            dropDown.Show(dataGridView, showPoint);

            column.dropDown = dropDown;
        }

        public override void DetachEditingControl()
        {
            var editorControl = this.GetEditorControl();
            if(editorControl != null && this.OwningColumn is SuggestionTextBoxColumn column)
            {
                if(this.editorControlTextChangedEvent != null)
                {
                    editorControl.TextChanged -= this.editorControlTextChangedEvent;
                }
                column.inEditMode = false;

                if (column.dropDown != null)
                {
                    column.dropDown.AutoClose = true; //If i do not set AutoClose to true, the dropdown WILL NOT CLOSE!
                    column.dropDown.Hide();
                    column.dropDown.Close();

                    column.dropDown = null;
                }
            }

            base.DetachEditingControl();
        }
    }

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
