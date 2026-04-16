using TiaUtilities.Utility;

namespace TiaUtilities.SettingsNew.FormHelpers
{
    public partial class OtherConfigurationForm : Form
    {
        private const int WM_NCACTIVATE = 0x86;

        public bool CloseOnOutsideClick { get; set; } = true;

        public OtherConfigurationForm()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.Shown += (sender, args) => formReadyToClose = true;
        }

        private bool formReadyToClose = false;
        protected override void WndProc(ref Message m)
        {
            try
            {
                // if click outside dialog -> Close Dlg
                //CanFocus is false if there is a modal window open to avoid closing for cliking it (Like color dialog or file dialog)
                if (CloseOnOutsideClick && formReadyToClose && m.Msg == WM_NCACTIVATE && this.CanFocus && !this.RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position))
                {
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            base.WndProc(ref m);
        }
    }
}
