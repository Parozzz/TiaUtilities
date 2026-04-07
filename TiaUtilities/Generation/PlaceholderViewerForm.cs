using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaUtilities.Generation.SettingsNew;
using TiaUtilities.Languages;
using TiaUtilities.Utility;

namespace TiaUtilities.Generation
{
    public partial class PlaceholderViewerForm : Form
    {

        public PlaceholderViewerForm(IEnumerable<string> placeholders)
        {
            InitializeComponent();

            this.placeholderListBox.Items.AddRange([.. placeholders]);
            this.Init();
        }

        private void Init()
        {
            Utils.SetDoubleBuffered(this.placeholderListBox);
            this.placeholderListBox.DoubleClick += (sender, args) => this.CopySelectedItem();

            this.StartPosition = FormStartPosition.Manual;

            var startPos = Cursor.Position;
            startPos.Offset(-Cursor.Size.Width, -Cursor.Size.Height);
            this.Location = startPos;

            this.Translate();
        }

        private void Translate()
        {
            this.Text = Locale.GEN_PLACEHOLDER_VIEWER_FORM;
        }

        private void CopySelectedItem()
        {
            var selected = this.placeholderListBox.SelectedItem;
            if (selected is string selectedStr)
            {
                Clipboard.Clear();
                Clipboard.SetText(selectedStr);

                this.Close();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.Close();
                        return true;
                    case Keys.C | Keys.Control:
                        this.CopySelectedItem();
                        return true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
