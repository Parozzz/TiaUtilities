using TiaUtilities.CustomControls.EditableTab;

namespace TiaUtilities.Generation.IO.Module
{
    partial class IOGenControl
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
            mainTableLayout = new TableLayoutPanel();
            configButtonPanel = new FlowLayoutPanel();
            setupButton = new Button();
            mainSplitContainer = new SplitContainer();
            tabControl = new EditableTabControl();
            mainTableLayout.SuspendLayout();
            configButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSize = true;
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(configButtonPanel, 0, 0);
            mainTableLayout.Controls.Add(mainSplitContainer, 0, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 2;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainTableLayout.Size = new Size(1069, 727);
            mainTableLayout.TabIndex = 0;
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSize = true;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(setupButton);
            configButtonPanel.Dock = DockStyle.Fill;
            configButtonPanel.Location = new Point(0, 3);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(1069, 30);
            configButtonPanel.TabIndex = 15;
            // 
            // setupButton
            // 
            setupButton.Anchor = AnchorStyles.None;
            setupButton.AutoSize = true;
            setupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            setupButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            setupButton.Location = new Point(3, 0);
            setupButton.Margin = new Padding(3, 0, 0, 0);
            setupButton.Name = "setupButton";
            setupButton.Padding = new Padding(8, 0, 8, 0);
            setupButton.Size = new Size(83, 30);
            setupButton.TabIndex = 17;
            setupButton.Text = "Setup";
            setupButton.UseVisualStyleBackColor = true;
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.Location = new Point(3, 36);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(tabControl);
            mainSplitContainer.Size = new Size(1063, 688);
            mainSplitContainer.SplitterDistance = 179;
            mainSplitContainer.SplitterWidth = 20;
            mainSplitContainer.TabIndex = 1;
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(12, 5);
            tabControl.RequireConfirmationBeforeClosing = false;
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(864, 688);
            tabControl.TabIndex = 0;
            // 
            // IOGenControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "IOGenControl";
            Size = new Size(1069, 727);
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainTableLayout;
        private SplitContainer mainSplitContainer;
        public EditableTabControl tabControl;
        private FlowLayoutPanel configButtonPanel;
        public Button setupButton;
    }
}
