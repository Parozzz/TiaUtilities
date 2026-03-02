using TiaUtilities.Generation.Alarms.Module.Template;
using TiaUtilities.Editors.ErrorReporting;

namespace TiaUtilities.Generation.Alarms.Module.Tab
{
    public partial class AlarmGenTabControl : UserControl
    {
        //UNUSED
        private readonly ErrorReportThread errorThread;
        private readonly AlarmGenTemplateHandler templateHandler;
        private readonly DataGridView dataGridView;

        public AlarmGenTabControl(ErrorReportThread errorThread, AlarmGenTemplateHandler templateHandler, DataGridView dataGridView)
        {
            this.errorThread = errorThread;
            this.templateHandler = templateHandler;
            this.dataGridView = dataGridView;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.Controls.Add(this.dataGridView);
        }

        public void Translate()
        {
        }

    }
}
