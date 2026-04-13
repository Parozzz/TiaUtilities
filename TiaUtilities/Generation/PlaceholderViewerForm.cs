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

            this.Shown += (sender, args) =>
            {
                this.Focus();
                this.placeholderListBox.Focus();

                this.placeholderListBox.SelectedIndex = 0;
            };
        }

        private void Init()
        {
            this.Height = this.placeholderListBox.PreferredHeight + this.placeholderListBox.ItemHeight * 3;
            
            this.placeholderListBox.DoubleClick += (sender, args) => this.CopySelectedItem();
            this.placeholderListBox.MouseWheel += (sender, args) =>
            {
                var index = this.placeholderListBox.SelectedIndex;
                if (args.Delta >= 0)
                {
                    index = Math.Max(0, index - 1);
                }
                else
                {
                    index = Math.Min(this.placeholderListBox.Items.Count - 1, index + 1);
                }
                this.placeholderListBox.SelectedIndex = index;
            };

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


                var owner = this.Owner;
                if(owner != null)
                {
                    owner.Focus(); //If not focused, tooltip does not show.
                    owner.ActiveControl?.Focus();

                    var tooltip = Utils.CreateQuickToolTip();
                    tooltip.ShowAlways = true;
                    tooltip.Show(Locale.GENERICS_COPIED, owner, owner.PointToClient(Cursor.Position), 1000);
                }

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
