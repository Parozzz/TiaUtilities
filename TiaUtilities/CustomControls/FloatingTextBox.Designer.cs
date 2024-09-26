namespace TiaUtilities.CustomControls
{
    partial class FloatingTextBox
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
            textBox = new RJTextBox();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            textBox.BackColor = SystemColors.ControlLight;
            textBox.BorderColor = Color.Black;
            textBox.BorderFocusColor = Color.Black;
            textBox.BorderRadius = 0;
            textBox.BorderSize = 2;
            textBox.Dock = DockStyle.Fill;
            textBox.Font = new Font("Segoe UI", 10.5F);
            textBox.Location = new Point(0, 0);
            textBox.Margin = new Padding(0);
            textBox.Multiline = false;
            textBox.Name = "textBox";
            textBox.Padding = new Padding(10);
            textBox.PasswordChar = false;
            textBox.ReadOnly = false;
            textBox.ScrollBars = ScrollBars.None;
            textBox.Size = new Size(800, 40);
            textBox.TabIndex = 0;
            textBox.TextAlign = HorizontalAlignment.Left;
            textBox.TextLeftPadding = 10;
            textBox.TextTopBottomPadding = 10;
            textBox.UnderlineColor = Color.HotPink;
            textBox.Underlined = false;
            // 
            // FloatingTextBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(800, 40);
            Controls.Add(textBox);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FloatingTextBox";
            Text = "FloatingTextBox";
            ResumeLayout(false);
        }

        #endregion

        private RJTextBox textBox;
    }
}