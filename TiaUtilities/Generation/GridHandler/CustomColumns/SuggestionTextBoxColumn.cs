using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.GridHandler.CustomColumns
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
                ShowImageMargin = ShowCheckMargin = false;
                RenderMode = ToolStripRenderMode.System;
                MaximumSize = new Size(width, height);
                AllowTransparency = true;
                AutoClose = false;
                ShowItemToolTips = false; // I don't need it. Sometimes ghost tool tips remain after clicking item.
                CanOverflow = true;

                typeof(MyDropDown).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, this, new object[] { true });
            }
        }

        private readonly Func<string[]> GetItemsFunc;

        private readonly DataGridViewCellCancelEventHandler cellBeginEditEvent;
        private readonly DataGridViewCellEventHandler cellEndEditEvent;
        private readonly DataGridViewEditingControlShowingEventHandler editingControlShowingEvent;

        private readonly EventHandler editingControlTextChangedEvent;

        private ToolStripDropDown dropDown;

        private DataGridViewTextBoxEditingControl editingControl;
        private bool inEditMode;

        public SuggestionTextBoxColumn(Func<string[]> GetItemsFunc)
        {
            this.GetItemsFunc = GetItemsFunc;

            this.cellBeginEditEvent = this.CellBeginEditEvent;
            this.cellEndEditEvent = this.CellEndEditEvent;
            this.editingControlShowingEvent = this.EditingControlShowingEvent;

            this.editingControlTextChangedEvent = this.EditingControlTextChangedEvent;
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

        private void CellBeginEditEvent(object sender, DataGridViewCellCancelEventArgs args)
        {
            var dataGridView = (DataGridView)sender;
            if (args.RowIndex < 0 || args.ColumnIndex < 0 || dataGridView.Columns[args.ColumnIndex] != this)
            {
                return;
            }

            var cell = (DataGridViewTextBoxCell)dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex];

            var items = GetItemsFunc();
            if (items == null || items.Length == 0)
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
            dropDown.ItemClicked += (sender1, args1) =>
            {
                if (editingControl == null)
                {
                    return;
                }

                editingControl.Text = args1.ClickedItem.Text;
                this.DataGridView.EndEdit();
            };
            dropDown.Show(dataGridView, showPoint);
        }

        private void CellEndEditEvent(object sender, DataGridViewCellEventArgs e)
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
            if (editingControl == null || dropDown == null || !inEditMode || (keyData != Keys.Up && keyData != Keys.Down))
            {
                return false;
            }

            //DropDown already support scrolling with arrows! This will focus it to enable it.
            var focused = dropDown.Focused;
            dropDown.Focus();
            if (!focused) //This will avoid that the first arrow sent is skipped!
            {
                SendKeys.SendWait(keyData == Keys.Up ? "{UP}" : "{DOWN}");
            }

            return true;
        }

        private void EditingControlShowingEvent(object sender, DataGridViewEditingControlShowingEventArgs args)
        {
            var dataGridView = (DataGridView)sender;
            if (dropDown == null || !inEditMode)
            {
                return;
            }

            if (args.Control is DataGridViewTextBoxEditingControl editingControl)
            {
                this.editingControl = editingControl;

                /* TEST EHEHEHEH
                var btn = new Button();
                btn.Padding = new Padding(0);
                btn.Margin = new Padding(0);
                btn.AutoSize = true;
                btn.Text = "";
                btn.Width = editingControl.Width / 5;
                btn.Height = editingControl.Height;
                btn.Dock = DockStyle.Right;
                editingControl.Controls.Add(btn);
                */
                this.UpdateVisibileSuggestions();
                editingControl.TextChanged += editingControlTextChangedEvent;
                //editingControl
            }
        }

        private void EditingControlTextChangedEvent(object sender, EventArgs args)
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

            var text = editingControl.Text.ToLower();
            foreach (ToolStripItem item in dropDown.Items)
            {
                var lowerCaseItemText = item.Text.ToLower();
                item.Visible = lowerCaseItemText.Contains(text) && !lowerCaseItemText.Equals(text);
            }

            this.dropDown.ResumeLayout();
        }
    }
}
