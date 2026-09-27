namespace TiaUtilities.SettingsStep
{
    partial class SettingsForm
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
            settingsControl = new SettingsControl();
            SuspendLayout();
            // 
            // settingsControl
            // 
            settingsControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settingsControl.Dock = DockStyle.Fill;
            settingsControl.Location = new Point(0, 0);
            settingsControl.Name = "settingsControl";
            settingsControl.Size = new Size(882, 721);
            settingsControl.TabIndex = 0;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 721);
            Controls.Add(settingsControl);
            Name = "SettingsForm";
            Text = "SettingsForm";
            ResumeLayout(false);
        }

        #endregion

        private SettingsControl settingsControl;
    }
}