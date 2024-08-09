using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.Alarm.Controls;

namespace TiaUtilities.Generation.GenForms.IO.Tab
{
    public partial class IOGenTabControl : UserControl
    {
        private readonly DataGridView dataGridView;

        public IOGenTabControl(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.generationTableLayout.Controls.Add(this.dataGridView);

            Translate();
        }

        private void Translate()
        {
            this.fcConfigButton.Text = Localization.Get("IO_GEN_CONFIG_FC");
            this.segmentNameConfigButton.Text = Localization.Get("IO_GEN_CONFIG_SEGMENT");
        }
    }
}
