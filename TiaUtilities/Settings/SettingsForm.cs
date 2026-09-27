using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaUtilities.SettingsStep
{
    public partial class SettingsForm : Form
    {
        private const int WS_EX_COMPOSITED = 0x02000000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Attiva lo stile WS_EX_COMPOSITED (0x02000000)
                // Costringe Windows a ridisegnare tutti i controlli figli bottom-up in un unico buffer
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        public SettingsForm()
        {
            InitializeComponent();

            this.settingsControl.InitControls();
        }

        public void SetSequences(IEnumerable<SettingsSequence> sequences) => this.settingsControl.SetSequences(sequences);

    }
}
