using TiaUtilities.Generation.GenForms.Alarm;
namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    partial class IOGenBottomControl
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
            mainSplitContainer = new SplitContainer();
            gridsTabControl = new CustomControls.InteractableTabControl();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.Location = new Point(0, 0);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(gridsTabControl);
            mainSplitContainer.Size = new Size(1480, 600);
            mainSplitContainer.SplitterDistance = 250;
            mainSplitContainer.SplitterWidth = 20;
            mainSplitContainer.TabIndex = 0;
            // 
            // gridsTabControl
            // 
            gridsTabControl.Dock = DockStyle.Fill;
            gridsTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            gridsTabControl.Location = new Point(0, 0);
            gridsTabControl.Name = "gridsTabControl";
            gridsTabControl.Padding = new Point(12, 5);
            gridsTabControl.RequireConfirmationBeforeClosing = false;
            gridsTabControl.SelectedIndex = 0;
            gridsTabControl.Size = new Size(1210, 600);
            gridsTabControl.TabIndex = 0;
            // 
            // IOGenBottomControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainSplitContainer);
            Name = "IOGenBottomControl";
            Size = new Size(1480, 600);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer mainSplitContainer;
        public CustomControls.InteractableTabControl gridsTabControl;
    }
}
