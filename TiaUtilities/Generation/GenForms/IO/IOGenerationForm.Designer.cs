using TiaXmlReader.CustomControls;

namespace TiaUtilities.Generation.GenForms.IO
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
            fileMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            importExportMenuItem = new ToolStripMenuItem();
            exportXMLMenuItem = new ToolStripMenuItem();
            importExcelMenuItem = new ToolStripMenuItem();
            importSuggestionsMenuItem = new ToolStripMenuItem();
            preferencesMenuItem = new ToolStripMenuItem();
            PlaceholdersLabel = new Label();
            configPanel = new Panel();
            configButtonPanel = new FlowLayoutPanel();
            fcConfigButton = new Button();
            dbConfigButton = new Button();
            variableTableConfigButton = new Button();
            ioTableConfigButton = new Button();
            segmentNameConfigButton = new Button();
            groupingTypeComboBox = new FlatComboBox();
            groupingTypeLabel = new Label();
            memoryTypeComboBox = new FlatComboBox();
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
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, importExportMenuItem, preferencesMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Size = new Size(1258, 24);
            TopMenuStrip.TabIndex = 17;
            TopMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, saveAsMenuItem, loadMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(37, 20);
            fileMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(111, 22);
            saveMenuItem.Text = "Save";
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(111, 22);
            saveAsMenuItem.Text = "SaveAs";
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(111, 22);
            loadMenuItem.Text = "Load";
            // 
            // importExportMenuItem
            // 
            importExportMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLMenuItem, importExcelMenuItem, importSuggestionsMenuItem });
            importExportMenuItem.Name = "importExportMenuItem";
            importExportMenuItem.Size = new Size(94, 20);
            importExportMenuItem.Text = "Import/Export";
            // 
            // exportXMLMenuItem
            // 
            exportXMLMenuItem.Name = "exportXMLMenuItem";
            exportXMLMenuItem.Size = new Size(254, 22);
            exportXMLMenuItem.Text = "Export XML";
            // 
            // importExcelMenuItem
            // 
            importExcelMenuItem.Name = "importExcelMenuItem";
            importExcelMenuItem.Size = new Size(254, 22);
            importExcelMenuItem.Text = "Import Excel";
            // 
            // importSuggestionsMenuItem
            // 
            importSuggestionsMenuItem.Name = "importSuggestionsMenuItem";
            importSuggestionsMenuItem.Size = new Size(254, 22);
            importSuggestionsMenuItem.Text = "Import Suggestions From DB/Tags";
            // 
            // preferencesMenuItem
            // 
            preferencesMenuItem.Name = "preferencesMenuItem";
            preferencesMenuItem.Size = new Size(80, 20);
            preferencesMenuItem.Text = "Preferences";
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
            configPanel.Controls.Add(groupingTypeLabel);
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
            fcConfigButton.AutoSize = true;
            fcConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fcConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            fcConfigButton.Location = new Point(3, 0);
            fcConfigButton.Margin = new Padding(3, 0, 0, 0);
            fcConfigButton.Name = "fcConfigButton";
            fcConfigButton.Padding = new Padding(8, 0, 8, 0);
            fcConfigButton.Size = new Size(59, 30);
            fcConfigButton.TabIndex = 10;
            fcConfigButton.Text = "FC";
            fcConfigButton.UseVisualStyleBackColor = true;
            // 
            // dbConfigButton
            // 
            dbConfigButton.AutoSize = true;
            dbConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            dbConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            dbConfigButton.Location = new Point(65, 0);
            dbConfigButton.Margin = new Padding(3, 0, 0, 0);
            dbConfigButton.Name = "dbConfigButton";
            dbConfigButton.Padding = new Padding(8, 0, 8, 0);
            dbConfigButton.Size = new Size(135, 30);
            dbConfigButton.TabIndex = 11;
            dbConfigButton.Text = "DB Appoggi";
            dbConfigButton.UseVisualStyleBackColor = true;
            // 
            // variableTableConfigButton
            // 
            variableTableConfigButton.AutoSize = true;
            variableTableConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            variableTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            variableTableConfigButton.Location = new Point(203, 0);
            variableTableConfigButton.Margin = new Padding(3, 0, 0, 0);
            variableTableConfigButton.Name = "variableTableConfigButton";
            variableTableConfigButton.Padding = new Padding(8, 0, 8, 0);
            variableTableConfigButton.Size = new Size(169, 30);
            variableTableConfigButton.TabIndex = 12;
            variableTableConfigButton.Text = "Tabella Appoggi";
            variableTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // ioTableConfigButton
            // 
            ioTableConfigButton.AutoSize = true;
            ioTableConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ioTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            ioTableConfigButton.Location = new Point(375, 0);
            ioTableConfigButton.Margin = new Padding(3, 0, 0, 0);
            ioTableConfigButton.Name = "ioTableConfigButton";
            ioTableConfigButton.Padding = new Padding(8, 0, 8, 0);
            ioTableConfigButton.Size = new Size(121, 30);
            ioTableConfigButton.TabIndex = 13;
            ioTableConfigButton.Text = "Tabella IO";
            ioTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            segmentNameConfigButton.AutoSize = true;
            segmentNameConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            segmentNameConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            segmentNameConfigButton.Location = new Point(499, 0);
            segmentNameConfigButton.Margin = new Padding(3, 0, 0, 0);
            segmentNameConfigButton.Name = "segmentNameConfigButton";
            segmentNameConfigButton.Padding = new Padding(8, 0, 8, 0);
            segmentNameConfigButton.Size = new Size(162, 30);
            segmentNameConfigButton.TabIndex = 14;
            segmentNameConfigButton.Text = "Nomi Segmenti";
            segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // groupingTypeComboBox
            // 
            groupingTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(397, 33);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(156, 23);
            groupingTypeComboBox.TabIndex = 3;
            // 
            // groupingTypeLabel
            // 
            groupingTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupingTypeLabel.Location = new Point(211, 35);
            groupingTypeLabel.Name = "groupingTypeLabel";
            groupingTypeLabel.Size = new Size(185, 18);
            groupingTypeLabel.TabIndex = 2;
            groupingTypeLabel.Text = "Tipo raggruppamento";
            groupingTypeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // memoryTypeComboBox
            // 
            memoryTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            memoryTypeComboBox.FormattingEnabled = true;
            memoryTypeComboBox.Location = new Point(140, 33);
            memoryTypeComboBox.Name = "memoryTypeComboBox";
            memoryTypeComboBox.Size = new Size(65, 23);
            memoryTypeComboBox.TabIndex = 1;
            // 
            // memoryTypeLabel
            // 
            memoryTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            memoryTypeLabel.Location = new Point(6, 35);
            memoryTypeLabel.Name = "memoryTypeLabel";
            memoryTypeLabel.Size = new Size(135, 18);
            memoryTypeLabel.TabIndex = 0;
            memoryTypeLabel.Text = "Tipo memoria";
            memoryTypeLabel.TextAlign = ContentAlignment.MiddleRight;
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
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GridsSplitContainer).EndInit();
            GridsSplitContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public FlatComboBox groupingTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        public FlatComboBox memoryTypeComboBox;
        private System.Windows.Forms.Label PlaceholdersLabel;
        public System.Windows.Forms.Button fcConfigButton;
        public System.Windows.Forms.Button dbConfigButton;
        public System.Windows.Forms.Button variableTableConfigButton;
        public System.Windows.Forms.Button ioTableConfigButton;
        public System.Windows.Forms.Button segmentNameConfigButton;
        private System.Windows.Forms.FlowLayoutPanel configButtonPanel;
        private System.Windows.Forms.MenuStrip TopMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMenuItem;
        private System.Windows.Forms.TableLayoutPanel TopTableLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem importExportMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportXMLMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importExcelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSuggestionsMenuItem;
        private System.Windows.Forms.SplitContainer GridsSplitContainer;
        public Label groupingTypeLabel;
        public Label memoryTypeLabel;
    }
}
