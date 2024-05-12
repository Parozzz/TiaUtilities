using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Utility;

namespace TiaUtilities.Generation.GridHandler
{
    public partial class GridSearchForm : Form
    {
        public GridSearchForm()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch(keyData)
            {
                case Keys.Enter:
                    this.FindButton.PerformClick();
                    return true;
                case Keys.Enter | Keys.Control:
                    this.ReplaceButton.PerformClick();
                    return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
