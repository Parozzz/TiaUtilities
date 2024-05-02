using System;
using System.Linq;
using System.Windows.Forms;

namespace TiaXmlReader.Generation.GridHandler.CustomColumns
{
    public class SuggestionTextBoxColumn : DataGridViewTextBoxColumn, IGridCustomColumn
    {
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

            inEditMode = true;

            dropDown = new ToolStripDropDown();
            GetItemsFunc.Invoke().ToList().ForEach(str => dropDown.Items.Add(str));
            dropDown.AllowTransparency = true;
            dropDown.AutoClose = false;
            dropDown.ShowItemToolTips = false; //I don't need it. Sometimes ghost tool tips remain after clicking item.
            dropDown.ItemClicked += (sender1, args1) =>
            {
                if(editingControl != null)
                {
                    editingControl.Text = args1.ClickedItem.Text;
                    editingControl.SelectionStart = editingControl.TextLength; //Move Caret to end!
                }
            };

            var point = dataGridView.GetCellDisplayRectangle(args.ColumnIndex, args.RowIndex, false).Location;
            point.Y += cell.Size.Height;
            dropDown.Show(dataGridView, point);
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

            if(editingControl != null)
            {
                editingControl.TextChanged -= editingControlTextChangedEvent;
                editingControl = null;
            }
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
            if(editingControl == null)
            {
                return;
            }

            var text = editingControl.Text;
            foreach (ToolStripItem item in dropDown.Items)
            {//Done this way otherwise it will make the caret and cursor blink strage.
                if (!item.Text.StartsWith(text) || item.Text.Equals(text))
                {
                    if (item.Visible)
                    {
                        item.Visible = false;
                    }
                }
                else
                {
                    if (!item.Visible)
                    {
                        item.Visible = true;
                    }
                }
            }
        }
    }
}
