using TiaXmlReader.Utility;

namespace TiaUtilities.CustomControls
{
    public partial class FloatingTextBox : Form
    {
        public string InputText { get => this.textBox.Text; set => this.textBox.Text = value; }

        public FloatingTextBox()
        {
            InitializeComponent();

            this.Shown += (sender, args) => formReadyToClose = true;
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            try
            {
                // if click outside dialog -> Close Dlg
                //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
                if (formReadyToClose && m.Msg == 0x86 && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position)) //0x86 WM_NCACTIVATE
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return true;
                case Keys.Enter:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return true; //Return required otherwise will write the letter.
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
