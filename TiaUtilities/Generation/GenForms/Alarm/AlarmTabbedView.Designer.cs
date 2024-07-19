namespace TiaUtilities.Generation.GenForms.Alarm
{
    partial class AlarmTabbedView
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            gridsTabControl = new CustomControls.InteractableTabControl();
            defaultTabPage = new TabPage();
            gridsTabControl.SuspendLayout();
            SuspendLayout();
            // 
            // gridsTabControl
            // 
            gridsTabControl.Controls.Add(defaultTabPage);
            gridsTabControl.Dock = DockStyle.Fill;
            gridsTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            gridsTabControl.Location = new Point(0, 0);
            gridsTabControl.Margin = new Padding(0);
            gridsTabControl.Name = "gridsTabControl";
            gridsTabControl.Padding = new Point(12, 5);
            gridsTabControl.SelectedIndex = 0;
            gridsTabControl.Size = new Size(1480, 600);
            gridsTabControl.TabIndex = 0;
            // 
            // defaultTabPage
            // 
            defaultTabPage.Location = new Point(4, 28);
            defaultTabPage.Name = "defaultTabPage";
            defaultTabPage.Padding = new Padding(3);
            defaultTabPage.Size = new Size(1472, 568);
            defaultTabPage.TabIndex = 0;
            defaultTabPage.Text = "tabPage1";
            defaultTabPage.UseVisualStyleBackColor = true;
            // 
            // AlarmTabbedView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(gridsTabControl);
            Name = "AlarmTabbedView";
            Size = new Size(1480, 600);
            gridsTabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private CustomControls.InteractableTabControl gridsTabControl;
        private TabPage defaultTabPage;
    }
}
