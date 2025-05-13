using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.CustomColumns
{
    public class SuggestionTextBoxColumn : DataGridViewTextBoxColumn, IGridCustomColumn
    {
        private class MyDropDown : ToolStripDropDownMenu
        {
            protected override Padding DefaultPadding
            {
                get { return Padding.Empty; }
            }

            public MyDropDown(int width, int height)
            {
                this.ShowImageMargin = ShowCheckMargin = false;
                this.RenderMode = ToolStripRenderMode.Professional;
                this.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
                this.MaximumSize = new Size(width, height);
                this.AllowTransparency = true;
                this.AutoClose = false;
                this.ShowItemToolTips = false; // I don't need it. Sometimes ghost tool tips remain after clicking item.
                this.CanOverflow = false;
                this.DoubleBuffered = true;
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }


        private readonly DataGridViewCellCancelEventHandler cellBeginEditEvent;
        private readonly DataGridViewCellEventHandler cellEndEditEvent;
        private readonly DataGridViewEditingControlShowingEventHandler editingControlShowingEvent;

        private readonly EventHandler editingControlTextChangedEvent;

        private ToolStripDropDown? dropDown;
        private Func<IEnumerable<string?>>? GetItemsFunc;

        private DataGridViewTextBoxEditingControl? editingControl;
        private bool inEditMode;

        public SuggestionTextBoxColumn()
        {
            this.cellBeginEditEvent = this.CellBeginEditEvent;
            this.cellEndEditEvent = this.CellEndEditEvent;
            this.editingControlShowingEvent = this.EditingControlShowingEvent;

            this.editingControlTextChangedEvent = this.EditingControlTextChangedEvent;
        }

        public void SetGetItemsFunc(Func<IEnumerable<string?>> GetItemsFunc)
        {
            this.GetItemsFunc = GetItemsFunc;
        }

        public void RegisterEvents(DataGridView dataGridView)
        {
            dataGridView.CellBeginEdit += cellBeginEditEvent;
            dataGridView.CellEndEdit += cellEndEditEvent;
            dataGridView.EditingControlShowing += editingControlShowingEvent;
        }

        public void UnregisterEvents(DataGridView dataGridView)
        {
            dataGridView.CellBeginEdit -= cellBeginEditEvent;
            dataGridView.CellEndEdit -= cellEndEditEvent;
            dataGridView.EditingControlShowing -= editingControlShowingEvent;
        }

        private void CellBeginEditEvent(object? sender, DataGridViewCellCancelEventArgs args)
        {
            if (sender is DataGridView == false)
            {
                return;
            }

            var dataGridView = (DataGridView)sender;
            if (args.RowIndex < 0 || args.ColumnIndex < 0 || dataGridView.Columns[args.ColumnIndex] != this || GetItemsFunc == null)
            {
                return;
            }

            var cell = (DataGridViewTextBoxCell)dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];

            var items = GetItemsFunc().Where(v => v != null);
            if (items == null || !items.Any())
            {
                return;
            }

            inEditMode = true;

            var showPoint = dataGridView.GetCellDisplayRectangle(args.ColumnIndex, args.RowIndex, false).Location;
            showPoint.Y += cell.Size.Height;

            var height = 400;
            if ((showPoint.Y + height) > dataGridView.Height)
            {
                height = Math.Min(height, dataGridView.Height - showPoint.Y);
            }

            dropDown = new MyDropDown(cell.Size.Width, height);
            dropDown.Items.AddRange(items.Select(i => new ToolStripMenuItem() { Text = i }).ToArray()); //THIS IS WAAAAY FASTER! Without AddRange is super slow.
            dropDown.KeyDown += (sender1, args1) =>
            {//This is cool. If i start tipying again i want to trasnfer to the editing control so the mouse is not needed!
                if (this.editingControl == null || this.editingControl.Focused)
                {
                    return;
                }

                if (args1.KeyData == Keys.Enter || args1.KeyData == Keys.Up || args1.KeyData == Keys.Down)
                {
                    return;
                }

                args1.Handled = true;

                this.editingControl.FindForm()?.Focus();
                this.editingControl.Focus();
                //This trasfer the key data from the DropDown to the editing control.
                DllImports.PostMessage(this.editingControl.Handle, DllImports.WM_KEYDOWN, (int)args1.KeyData, 0);
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
        }

        private void CellEndEditEvent(object? sender, DataGridViewCellEventArgs e)
        {
            inEditMode = false;

            if (dropDown != null)
            {
                dropDown.AutoClose = true; //If i do not set AutoClose to true, the dropdown WILL NOT CLOSE!
                dropDown.Hide();
                dropDown = null;
            }

            if (editingControl != null)
            {
                editingControl.TextChanged -= editingControlTextChangedEvent;
                editingControl = null;
            }
        }

        public bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (editingControl == null || dropDown == null || !inEditMode)
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

        private void EditingControlShowingEvent(object? sender, DataGridViewEditingControlShowingEventArgs args)
        {
            if (sender is DataGridView == false)
            {
                return;
            }

            var dataGridView = (DataGridView)sender;
            if (dropDown == null || !inEditMode)
            {
                return;
            }

            if (args.Control is DataGridViewTextBoxEditingControl editingControl)
            {
                this.editingControl = editingControl;

                // TEST EHEHEHEH
                /*var btn = new Button
                {
                    Padding = new Padding(0),
                    Margin = new Padding(0),
                    AutoSize = true,
                    Text = "...",
                    Width = editingControl.Width / 5,
                    Height = editingControl.Height,
                    Dock = DockStyle.Right
                };
                editingControl.Controls.Add(btn);
                */
                this.UpdateVisibileSuggestions();
                editingControl.TextChanged += editingControlTextChangedEvent;
                //editingControl
            }
        }

        private void EditingControlTextChangedEvent(object? sender, EventArgs args)
        {
            this.UpdateVisibileSuggestions();
        }

        private void UpdateVisibileSuggestions()
        {
            if (editingControl == null || dropDown == null || !inEditMode)
            {
                return;
            }

            this.dropDown.SuspendLayout(); //Without this is unusable. Cursor blink and is SLLLLLOOOOOOWWWW.

            int visibleItemCount = 0;

            var text = editingControl.Text;
            foreach (ToolStripItem item in dropDown.Items)
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
