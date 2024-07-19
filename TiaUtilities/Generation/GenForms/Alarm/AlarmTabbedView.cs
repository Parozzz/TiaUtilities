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
    public partial class AlarmTabbedView : UserControl
    {
        private readonly AlarmGenerationProject project;
        public AlarmTabbedView(AlarmGenerationProject project)
        {
            this.project = project;

            InitializeComponent();
        }

        public void Init()
        {
            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            var deviceAlarmGridControl = new DeviceAlarmGridControl(project.deviceGridHandler.DataGridView, project.alarmGridHandler.DataGridView);
            deviceAlarmGridControl.Init();

            this.defaultTabPage.Controls.Add(deviceAlarmGridControl);
        }
    }
}
