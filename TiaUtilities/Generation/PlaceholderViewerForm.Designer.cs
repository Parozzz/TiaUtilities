using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation
{
    partial class PlaceholderViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            placeholderListBox = new NoScrollbarListBox();
            SuspendLayout();
            // 
            // placeholderListBox
            // 
            placeholderListBox.BorderStyle = BorderStyle.None;
            placeholderListBox.Dock = DockStyle.Fill;
            placeholderListBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            placeholderListBox.FormattingEnabled = true;
            placeholderListBox.ItemHeight = 17;
            placeholderListBox.Location = new Point(0, 0);
            placeholderListBox.Name = "placeholderListBox";
            placeholderListBox.ShowScrollbar = false;
            placeholderListBox.Size = new Size(259, 161);
            placeholderListBox.TabIndex = 0;
            // 
            // PlaceholderViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(259, 161);
            Controls.Add(placeholderListBox);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "PlaceholderViewerForm";
            ShowInTaskbar = false;
            Text = "PlaceholderViewerForm";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private NoScrollbarListBox placeholderListBox;
    }
}