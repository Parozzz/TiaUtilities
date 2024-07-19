namespace TiaUtilities.Generation.GenForms.Alarm
{
    partial class DeviceAlarmGridControl
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
            gridSplitContainer = new SplitContainer();
            mainTableLayout = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)gridSplitContainer).BeginInit();
            gridSplitContainer.SuspendLayout();
            mainTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // gridSplitContainer
            // 
            gridSplitContainer.Dock = DockStyle.Fill;
            gridSplitContainer.Location = new Point(3, 3);
            gridSplitContainer.Name = "gridSplitContainer";
            gridSplitContainer.Size = new Size(1474, 594);
            gridSplitContainer.SplitterDistance = 549;
            gridSplitContainer.SplitterWidth = 20;
            gridSplitContainer.TabIndex = 0;
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(gridSplitContainer, 0, 0);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 1;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1480, 600);
            mainTableLayout.TabIndex = 0;
            // 
            // DeviceAlarmGridControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "DeviceAlarmGridControl";
            Size = new Size(1480, 600);
            ((System.ComponentModel.ISupportInitialize)gridSplitContainer).EndInit();
            gridSplitContainer.ResumeLayout(false);
            mainTableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer gridSplitContainer;
        private TableLayoutPanel mainTableLayout;
    }
}
