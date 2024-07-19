using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public partial class DeviceAlarmGridControl : UserControl
    {
        private readonly DataGridView dataGridViewLeft;
        private readonly DataGridView dataGridViewRight;

        public DeviceAlarmGridControl(DataGridView dataGridViewLeft, DataGridView dataGridViewRight)
        {
            this.dataGridViewLeft = dataGridViewLeft;
            this.dataGridViewRight = dataGridViewRight;

            InitializeComponent();

            //This is a subordinated control. Init is called in the class that add this.
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.gridSplitContainer.Panel1.Controls.Add(this.dataGridViewLeft);
            this.gridSplitContainer.Panel2.Controls.Add(this.dataGridViewRight);
        }
    }
}
