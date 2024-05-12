namespace TiaXmlReader.Generation.IO.GenerationForm
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
            mainPanel = new TableLayoutPanel();
            TopTableLayoutPanel = new TableLayoutPanel();
            TopMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            importExportToolStripMenuItem = new ToolStripMenuItem();
            exportXMLToolStripMenuItem = new ToolStripMenuItem();
            importExcelToolStripMenuItem = new ToolStripMenuItem();
            importSuggestionsToolStripMenuItem = new ToolStripMenuItem();
            preferencesToolStripMenuItem = new ToolStripMenuItem();
            PlaceholdersLabel = new Label();
            configPanel = new Panel();
            configButtonPanel = new FlowLayoutPanel();
            fcConfigButton = new Button();
            dbConfigButton = new Button();
            variableTableConfigButton = new Button();
            ioTableConfigButton = new Button();
            segmentNameConfigButton = new Button();
            groupingTypeComboBox = new CustomControls.FlatComboBox();
            divisionTypeLabel = new Label();
            memoryTypeComboBox = new CustomControls.FlatComboBox();
            memoryTypeLabel = new Label();
            GridsSplitContainer = new SplitContainer();
            mainPanel.SuspendLayout();
            TopTableLayoutPanel.SuspendLayout();
            TopMenuStrip.SuspendLayout();
            configPanel.SuspendLayout();
            configButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridsSplitContainer).BeginInit();
            GridsSplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(TopTableLayoutPanel, 0, 0);
            mainPanel.Controls.Add(GridsSplitContainer, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            mainPanel.Size = new Size(1264, 681);
            mainPanel.TabIndex = 1;
            // 
            // TopTableLayoutPanel
            // 
            TopTableLayoutPanel.AutoSize = true;
            TopTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TopTableLayoutPanel.ColumnCount = 1;
            TopTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TopTableLayoutPanel.Controls.Add(TopMenuStrip, 0, 0);
            TopTableLayoutPanel.Controls.Add(PlaceholdersLabel, 0, 1);
            TopTableLayoutPanel.Controls.Add(configPanel, 0, 2);
            TopTableLayoutPanel.Dock = DockStyle.Top;
            TopTableLayoutPanel.Location = new Point(3, 3);
            TopTableLayoutPanel.Margin = new Padding(3, 3, 3, 0);
            TopTableLayoutPanel.Name = "TopTableLayoutPanel";
            TopTableLayoutPanel.RowCount = 2;
            TopTableLayoutPanel.RowStyles.Add(new RowStyle());
            TopTableLayoutPanel.RowStyles.Add(new RowStyle());
            TopTableLayoutPanel.RowStyles.Add(new RowStyle());
            TopTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            TopTableLayoutPanel.Size = new Size(1258, 106);
            TopTableLayoutPanel.TabIndex = 18;
            // 
            // TopMenuStrip
            // 
            TopMenuStrip.BackColor = Color.Transparent;
            TopMenuStrip.GripStyle = ToolStripGripStyle.Visible;
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, importExportToolStripMenuItem, preferencesToolStripMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Size = new Size(1258, 24);
            TopMenuStrip.TabIndex = 17;
            TopMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem, saveAsToolStripMenuItem, loadToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(111, 22);
            saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(111, 22);
            saveAsToolStripMenuItem.Text = "SaveAs";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(111, 22);
            loadToolStripMenuItem.Text = "Load";
            // 
            // importExportToolStripMenuItem
            // 
            importExportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLToolStripMenuItem, importExcelToolStripMenuItem, importSuggestionsToolStripMenuItem });
            importExportToolStripMenuItem.Name = "importExportToolStripMenuItem";
            importExportToolStripMenuItem.Size = new Size(94, 20);
            importExportToolStripMenuItem.Text = "Import/Export";
            // 
            // exportXMLToolStripMenuItem
            // 
            exportXMLToolStripMenuItem.Name = "exportXMLToolStripMenuItem";
            exportXMLToolStripMenuItem.Size = new Size(254, 22);
            exportXMLToolStripMenuItem.Text = "Export XML";
            // 
            // importExcelToolStripMenuItem
            // 
            importExcelToolStripMenuItem.Name = "importExcelToolStripMenuItem";
            importExcelToolStripMenuItem.Size = new Size(254, 22);
            importExcelToolStripMenuItem.Text = "Import Excel";
            // 
            // importSuggestionsToolStripMenuItem
            // 
            importSuggestionsToolStripMenuItem.Name = "importSuggestionsToolStripMenuItem";
            importSuggestionsToolStripMenuItem.Size = new Size(254, 22);
            importSuggestionsToolStripMenuItem.Text = "Import Suggestions From DB/Tags";
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.Size = new Size(80, 20);
            preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // PlaceholdersLabel
            // 
            PlaceholdersLabel.Dock = DockStyle.Top;
            PlaceholdersLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PlaceholdersLabel.Location = new Point(3, 24);
            PlaceholdersLabel.Name = "PlaceholdersLabel";
            PlaceholdersLabel.Size = new Size(1252, 16);
            PlaceholdersLabel.TabIndex = 9;
            PlaceholdersLabel.Text = "Placeholders: {memory_type} {bit} {byte} {io_name} {variable_name} {comment} {config_db_name} {config_db_number}";
            // 
            // configPanel
            // 
            configPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configPanel.Controls.Add(configButtonPanel);
            configPanel.Controls.Add(groupingTypeComboBox);
            configPanel.Controls.Add(divisionTypeLabel);
            configPanel.Controls.Add(memoryTypeComboBox);
            configPanel.Controls.Add(memoryTypeLabel);
            configPanel.Dock = DockStyle.Fill;
            configPanel.Location = new Point(3, 43);
            configPanel.Name = "configPanel";
            configPanel.Size = new Size(1252, 60);
            configPanel.TabIndex = 3;
            // 
            // configButtonPanel
            // 
            configButtonPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(fcConfigButton);
            configButtonPanel.Controls.Add(dbConfigButton);
            configButtonPanel.Controls.Add(variableTableConfigButton);
            configButtonPanel.Controls.Add(ioTableConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);
            configButtonPanel.Location = new Point(0, -1);
            configButtonPanel.Margin = new Padding(0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(1252, 30);
            configButtonPanel.TabIndex = 15;
            // 
            // fcConfigButton
            // 
            fcConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            fcConfigButton.Location = new Point(0, 0);
            fcConfigButton.Margin = new Padding(0);
            fcConfigButton.Name = "fcConfigButton";
            fcConfigButton.Size = new Size(71, 30);
            fcConfigButton.TabIndex = 10;
            fcConfigButton.Text = "FC";
            fcConfigButton.UseVisualStyleBackColor = true;
            // 
            // dbConfigButton
            // 
            dbConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            dbConfigButton.Location = new Point(71, 0);
            dbConfigButton.Margin = new Padding(0);
            dbConfigButton.Name = "dbConfigButton";
            dbConfigButton.Size = new Size(147, 30);
            dbConfigButton.TabIndex = 11;
            dbConfigButton.Text = "DB Appoggi";
            dbConfigButton.UseVisualStyleBackColor = true;
            // 
            // variableTableConfigButton
            // 
            variableTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            variableTableConfigButton.Location = new Point(218, 0);
            variableTableConfigButton.Margin = new Padding(0);
            variableTableConfigButton.Name = "variableTableConfigButton";
            variableTableConfigButton.Size = new Size(183, 30);
            variableTableConfigButton.TabIndex = 12;
            variableTableConfigButton.Text = "Tabella Appoggi";
            variableTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // ioTableConfigButton
            // 
            ioTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            ioTableConfigButton.Location = new Point(401, 0);
            ioTableConfigButton.Margin = new Padding(0);
            ioTableConfigButton.Name = "ioTableConfigButton";
            ioTableConfigButton.Size = new Size(134, 30);
            ioTableConfigButton.TabIndex = 13;
            ioTableConfigButton.Text = "Tabella IO";
            ioTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            segmentNameConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            segmentNameConfigButton.Location = new Point(535, 0);
            segmentNameConfigButton.Margin = new Padding(0);
            segmentNameConfigButton.Name = "segmentNameConfigButton";
            segmentNameConfigButton.Size = new Size(152, 30);
            segmentNameConfigButton.TabIndex = 14;
            segmentNameConfigButton.Text = "Nomi Segmenti";
            segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // groupingTypeComboBox
            // 
            groupingTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(332, 33);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(136, 23);
            groupingTypeComboBox.TabIndex = 3;
            // 
            // divisionTypeLabel
            // 
            divisionTypeLabel.AutoSize = true;
            divisionTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            divisionTypeLabel.Location = new Point(184, 34);
            divisionTypeLabel.Name = "divisionTypeLabel";
            divisionTypeLabel.Size = new Size(149, 18);
            divisionTypeLabel.TabIndex = 2;
            divisionTypeLabel.Text = "Tipo raggruppamento";
            // 
            // memoryTypeComboBox
            // 
            memoryTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            memoryTypeComboBox.FormattingEnabled = true;
            memoryTypeComboBox.Location = new Point(111, 33);
            memoryTypeComboBox.Name = "memoryTypeComboBox";
            memoryTypeComboBox.Size = new Size(65, 23);
            memoryTypeComboBox.TabIndex = 1;
            // 
            // memoryTypeLabel
            // 
            memoryTypeLabel.AutoSize = true;
            memoryTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            memoryTypeLabel.Location = new Point(9, 34);
            memoryTypeLabel.Name = "memoryTypeLabel";
            memoryTypeLabel.Size = new Size(100, 18);
            memoryTypeLabel.TabIndex = 0;
            memoryTypeLabel.Text = "Tipo memoria";
            // 
            // GridsSplitContainer
            // 
            GridsSplitContainer.Dock = DockStyle.Fill;
            GridsSplitContainer.Location = new Point(5, 114);
            GridsSplitContainer.Margin = new Padding(5);
            GridsSplitContainer.Name = "GridsSplitContainer";
            GridsSplitContainer.Size = new Size(1254, 562);
            GridsSplitContainer.SplitterDistance = 345;
            GridsSplitContainer.TabIndex = 19;
            // 
            // IOGenerationForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1264, 681);
            Controls.Add(mainPanel);
            DoubleBuffered = true;
            MainMenuStrip = TopMenuStrip;
            Name = "IOGenerationForm";
            Text = "IOGenerationForm";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            TopTableLayoutPanel.ResumeLayout(false);
            TopTableLayoutPanel.PerformLayout();
            TopMenuStrip.ResumeLayout(false);
            TopMenuStrip.PerformLayout();
            configPanel.ResumeLayout(false);
            configPanel.PerformLayout();
            configButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)GridsSplitContainer).EndInit();
            GridsSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public CustomControls.FlatComboBox groupingTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        public CustomControls.FlatComboBox memoryTypeComboBox;
        private System.Windows.Forms.Label memoryTypeLabel;
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
        private System.Windows.Forms.ToolStripMenuItem importSuggestionsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer GridsSplitContainer;
    }
}
