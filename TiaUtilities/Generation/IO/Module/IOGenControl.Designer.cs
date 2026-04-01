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
            tableLayoutPanel1 = new TableLayoutPanel();
            PlaceholdersLabel = new Label();
            configButtonPanel = new FlowLayoutPanel();
            setupButton = new Button();
            mainSplitContainer = new SplitContainer();
            tabControl = new TiaUtilities.CustomControls.InteractableTabControl();
            mainTableLayout.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            mainTableLayout.Controls.Add(tableLayoutPanel1, 0, 0);
            mainTableLayout.Controls.Add(mainSplitContainer, 0, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 2;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1069, 727);
            mainTableLayout.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(PlaceholdersLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(configButtonPanel, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1063, 49);
            tableLayoutPanel1.TabIndex = 17;
            // 
            // PlaceholdersLabel
            // 
            PlaceholdersLabel.Dock = DockStyle.Top;
            PlaceholdersLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PlaceholdersLabel.Location = new Point(3, 0);
            PlaceholdersLabel.Name = "PlaceholdersLabel";
            PlaceholdersLabel.Size = new Size(1057, 16);
            PlaceholdersLabel.TabIndex = 10;
            PlaceholdersLabel.Text = "Placeholders: [tab_name] {device_name} {device_address} {device_description} {alarm_num_start} {alarm_num_end} {alarm_num} {alarm_description}";
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSize = true;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(setupButton);
            configButtonPanel.Dock = DockStyle.Fill;
            configButtonPanel.Location = new Point(0, 19);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(1063, 30);
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
            mainSplitContainer.Location = new Point(3, 58);
            mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(tabControl);
            mainSplitContainer.Size = new Size(1063, 666);
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
            tabControl.Size = new Size(864, 666);
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
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
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
        public CustomControls.InteractableTabControl tabControl;
        private TableLayoutPanel tableLayoutPanel1;
        private Label PlaceholdersLabel;
        private FlowLayoutPanel configButtonPanel;
        public Button setupButton;
    }
}
