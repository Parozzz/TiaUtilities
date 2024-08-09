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

namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    public partial class IOGenBottomControl : UserControl
    {
        private readonly DataGridView suggestionDataGridView;

        public IOGenBottomControl(DataGridView suggestionDataGridView)
        {//This is a subordinated control. Init is called in the class that add this.
            this.suggestionDataGridView = suggestionDataGridView;

            InitializeComponent();

            this.AutoSize = true; //AutoSize set here otherwise while doing the UI, everything will be shrinked to minimun (So useless)
            this.Dock = DockStyle.Fill;

            this.mainSplitContainer.Panel1.Controls.Add(this.suggestionDataGridView);
        }
    }
}
