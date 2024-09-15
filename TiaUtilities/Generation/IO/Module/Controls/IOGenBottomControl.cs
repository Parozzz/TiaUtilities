namespace TiaUtilities.Generation.GenModules.IO.Controls
{
    public partial class IOGenBottomControl : UserControl
    {
        private readonly DataGridView suggestionDataGridView;

        public IOGenBottomControl(DataGridView suggestionDataGridView)
        {//This is a subordinated control. Init is called in the class that add this.
            this.suggestionDataGridView = suggestionDataGridView;

            InitializeComponent();

            this.gridsTabControl.RequireConfirmationBeforeClosing = true;

            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.mainSplitContainer.Panel1.Controls.Add(this.suggestionDataGridView);
        }
    }
}
