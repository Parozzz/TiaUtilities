namespace TiaXmlReader.GenerationForms.IO
{
    partial class IOGenerationForm
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
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TopTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TopMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PlaceholdersLabel = new System.Windows.Forms.Label();
            this.configPanel = new System.Windows.Forms.Panel();
            this.configButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.fcConfigButton = new System.Windows.Forms.Button();
            this.dbConfigButton = new System.Windows.Forms.Button();
            this.variableTableConfigButton = new System.Windows.Forms.Button();
            this.ioTableConfigButton = new System.Windows.Forms.Button();
            this.segmentNameConfigButton = new System.Windows.Forms.Button();
            this.groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.divisionTypeLabel = new System.Windows.Forms.Label();
            this.memoryTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.memoryTypeLabel = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.mainPanel.SuspendLayout();
            this.TopTableLayoutPanel.SuspendLayout();
            this.TopMenuStrip.SuspendLayout();
            this.configPanel.SuspendLayout();
            this.configButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.TopTableLayoutPanel, 0, 0);
            this.mainPanel.Controls.Add(this.dataGridView, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainPanel.Size = new System.Drawing.Size(1093, 537);
            this.mainPanel.TabIndex = 1;
            // 
            // TopTableLayoutPanel
            // 
            this.TopTableLayoutPanel.AutoSize = true;
            this.TopTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopTableLayoutPanel.ColumnCount = 1;
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TopTableLayoutPanel.Controls.Add(this.TopMenuStrip, 0, 0);
            this.TopTableLayoutPanel.Controls.Add(this.PlaceholdersLabel, 0, 1);
            this.TopTableLayoutPanel.Controls.Add(this.configPanel, 0, 2);
            this.TopTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.TopTableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.TopTableLayoutPanel.Name = "TopTableLayoutPanel";
            this.TopTableLayoutPanel.RowCount = 2;
            this.TopTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TopTableLayoutPanel.Size = new System.Drawing.Size(1087, 106);
            this.TopTableLayoutPanel.TabIndex = 18;
            // 
            // TopMenuStrip
            // 
            this.TopMenuStrip.BackColor = System.Drawing.Color.Transparent;
            this.TopMenuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.TopMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.importExportToolStripMenuItem,
            this.preferencesToolStripMenuItem});
            this.TopMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.TopMenuStrip.Name = "TopMenuStrip";
            this.TopMenuStrip.Size = new System.Drawing.Size(1087, 24);
            this.TopMenuStrip.TabIndex = 17;
            this.TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.saveAsToolStripMenuItem.Text = "SaveAs";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // importExportToolStripMenuItem
            // 
            this.importExportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportXMLToolStripMenuItem,
            this.importExcelToolStripMenuItem});
            this.importExportToolStripMenuItem.Name = "importExportToolStripMenuItem";
            this.importExportToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.importExportToolStripMenuItem.Text = "Import/Export";
            // 
            // exportXMLToolStripMenuItem
            // 
            this.exportXMLToolStripMenuItem.Name = "exportXMLToolStripMenuItem";
            this.exportXMLToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.exportXMLToolStripMenuItem.Text = "Export XML";
            // 
            // importExcelToolStripMenuItem
            // 
            this.importExcelToolStripMenuItem.Name = "importExcelToolStripMenuItem";
            this.importExcelToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.importExcelToolStripMenuItem.Text = "Import Excel";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // PlaceholdersLabel
            // 
            this.PlaceholdersLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PlaceholdersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlaceholdersLabel.Location = new System.Drawing.Point(3, 24);
            this.PlaceholdersLabel.Name = "PlaceholdersLabel";
            this.PlaceholdersLabel.Size = new System.Drawing.Size(1081, 16);
            this.PlaceholdersLabel.TabIndex = 9;
            this.PlaceholdersLabel.Text = "Placeholders: {memory_type} {bit} {byte} {io_name} {db_name} {variable_name} {com" +
    "ment} ";
            // 
            // configPanel
            // 
            this.configPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configPanel.Controls.Add(this.configButtonPanel);
            this.configPanel.Controls.Add(this.groupingTypeComboBox);
            this.configPanel.Controls.Add(this.divisionTypeLabel);
            this.configPanel.Controls.Add(this.memoryTypeComboBox);
            this.configPanel.Controls.Add(this.memoryTypeLabel);
            this.configPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configPanel.Location = new System.Drawing.Point(3, 43);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(1081, 60);
            this.configPanel.TabIndex = 3;
            // 
            // configButtonPanel
            // 
            this.configButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.configButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configButtonPanel.Controls.Add(this.fcConfigButton);
            this.configButtonPanel.Controls.Add(this.dbConfigButton);
            this.configButtonPanel.Controls.Add(this.variableTableConfigButton);
            this.configButtonPanel.Controls.Add(this.ioTableConfigButton);
            this.configButtonPanel.Controls.Add(this.segmentNameConfigButton);
            this.configButtonPanel.Location = new System.Drawing.Point(0, -1);
            this.configButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.configButtonPanel.Name = "configButtonPanel";
            this.configButtonPanel.Size = new System.Drawing.Size(1081, 30);
            this.configButtonPanel.TabIndex = 15;
            // 
            // fcConfigButton
            // 
            this.fcConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.fcConfigButton.Location = new System.Drawing.Point(0, 0);
            this.fcConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.fcConfigButton.Name = "fcConfigButton";
            this.fcConfigButton.Size = new System.Drawing.Size(71, 30);
            this.fcConfigButton.TabIndex = 10;
            this.fcConfigButton.Text = "FC";
            this.fcConfigButton.UseVisualStyleBackColor = true;
            // 
            // dbConfigButton
            // 
            this.dbConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.dbConfigButton.Location = new System.Drawing.Point(71, 0);
            this.dbConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.dbConfigButton.Name = "dbConfigButton";
            this.dbConfigButton.Size = new System.Drawing.Size(147, 30);
            this.dbConfigButton.TabIndex = 11;
            this.dbConfigButton.Text = "DB Appoggi";
            this.dbConfigButton.UseVisualStyleBackColor = true;
            // 
            // variableTableConfigButton
            // 
            this.variableTableConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.variableTableConfigButton.Location = new System.Drawing.Point(218, 0);
            this.variableTableConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.variableTableConfigButton.Name = "variableTableConfigButton";
            this.variableTableConfigButton.Size = new System.Drawing.Size(183, 30);
            this.variableTableConfigButton.TabIndex = 12;
            this.variableTableConfigButton.Text = "Tabella Appoggi";
            this.variableTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // ioTableConfigButton
            // 
            this.ioTableConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.ioTableConfigButton.Location = new System.Drawing.Point(401, 0);
            this.ioTableConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.ioTableConfigButton.Name = "ioTableConfigButton";
            this.ioTableConfigButton.Size = new System.Drawing.Size(134, 30);
            this.ioTableConfigButton.TabIndex = 13;
            this.ioTableConfigButton.Text = "Tabella IO";
            this.ioTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            this.segmentNameConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.segmentNameConfigButton.Location = new System.Drawing.Point(535, 0);
            this.segmentNameConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.segmentNameConfigButton.Name = "segmentNameConfigButton";
            this.segmentNameConfigButton.Size = new System.Drawing.Size(152, 30);
            this.segmentNameConfigButton.TabIndex = 14;
            this.segmentNameConfigButton.Text = "Nomi Segmenti";
            this.segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // groupingTypeComboBox
            // 
            this.groupingTypeComboBox.FormattingEnabled = true;
            this.groupingTypeComboBox.Location = new System.Drawing.Point(332, 33);
            this.groupingTypeComboBox.Name = "groupingTypeComboBox";
            this.groupingTypeComboBox.Size = new System.Drawing.Size(112, 21);
            this.groupingTypeComboBox.TabIndex = 3;
            // 
            // divisionTypeLabel
            // 
            this.divisionTypeLabel.AutoSize = true;
            this.divisionTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.divisionTypeLabel.Location = new System.Drawing.Point(184, 34);
            this.divisionTypeLabel.Name = "divisionTypeLabel";
            this.divisionTypeLabel.Size = new System.Drawing.Size(149, 18);
            this.divisionTypeLabel.TabIndex = 2;
            this.divisionTypeLabel.Text = "Tipo raggruppamento";
            // 
            // memoryTypeComboBox
            // 
            this.memoryTypeComboBox.FormattingEnabled = true;
            this.memoryTypeComboBox.Location = new System.Drawing.Point(111, 33);
            this.memoryTypeComboBox.Name = "memoryTypeComboBox";
            this.memoryTypeComboBox.Size = new System.Drawing.Size(65, 21);
            this.memoryTypeComboBox.TabIndex = 1;
            // 
            // memoryTypeLabel
            // 
            this.memoryTypeLabel.AutoSize = true;
            this.memoryTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryTypeLabel.Location = new System.Drawing.Point(9, 34);
            this.memoryTypeLabel.Name = "memoryTypeLabel";
            this.memoryTypeLabel.Size = new System.Drawing.Size(100, 18);
            this.memoryTypeLabel.TabIndex = 0;
            this.memoryTypeLabel.Text = "Tipo memoria";
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 109);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(1087, 405);
            this.dataGridView.TabIndex = 2;
            // 
            // IOGenerationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1093, 537);
            this.Controls.Add(this.mainPanel);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.TopMenuStrip;
            this.Name = "IOGenerationForm";
            this.Text = "IOGenerationForm";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.TopTableLayoutPanel.ResumeLayout(false);
            this.TopTableLayoutPanel.PerformLayout();
            this.TopMenuStrip.ResumeLayout(false);
            this.TopMenuStrip.PerformLayout();
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            this.configButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public CustomControls.FlatComboBox groupingTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        public CustomControls.FlatComboBox memoryTypeComboBox;
        private System.Windows.Forms.Label memoryTypeLabel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label PlaceholdersLabel;
        public System.Windows.Forms.Button fcConfigButton;
        public System.Windows.Forms.Button dbConfigButton;
        public System.Windows.Forms.Button variableTableConfigButton;
        public System.Windows.Forms.Button ioTableConfigButton;
        public System.Windows.Forms.Button segmentNameConfigButton;
        private System.Windows.Forms.FlowLayoutPanel configButtonPanel;
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel TopTableLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem importExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
    }
}