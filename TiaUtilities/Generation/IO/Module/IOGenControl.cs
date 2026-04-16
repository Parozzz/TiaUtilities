using TiaUtilities.Generation.Configuration.Utility;
using TiaUtilities.Languages;
using TiaUtilities.Generation.Configuration;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Generation.IO.Configurations;

namespace TiaUtilities.Generation.IO.Module
{
    public partial class IOGenControl : UserControl
    {
        private readonly DataGridView suggestionDataGridView;
        public IOGenControl(DataGridView suggestionDataGridView)
        {
            //This is a subordinated control. Init is called in the class that add this.
            this.suggestionDataGridView = suggestionDataGridView;

            InitializeComponent();

            this.tabControl.RequireConfirmationBeforeClosing = true;

            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.mainSplitContainer.Panel1.Controls.Add(this.suggestionDataGridView);

            Translate();
        }

        private void Translate()
        {
            this.setupButton.Text = Locale.GENERICS_SETUP;
        }
    }
}
