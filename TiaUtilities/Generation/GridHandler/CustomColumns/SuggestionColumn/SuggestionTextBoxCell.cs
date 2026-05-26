using System.Reflection;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation.GridHandler.CustomColumns.SuggestionColumn
{
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

            return (DataGridViewTextBoxEditingControl?)editingTextBoxProperty.GetValue(this);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (this.DataGridView is not DataGridView dataGridView || rowIndex < 0 || this.ColumnIndex < 0)
            {
                return;
            }

            if (this.OwningColumn is not SuggestionTextBoxColumn column || column.GetItemsFunc == null)
            {
                return;
            }

            if (column.dropDown != null)
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
            editingControl.TextChanged += this.editorControlTextChangedEvent;

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
            if (editorControl != null && this.OwningColumn is SuggestionTextBoxColumn column)
            {
                if (this.editorControlTextChangedEvent != null)
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
}
