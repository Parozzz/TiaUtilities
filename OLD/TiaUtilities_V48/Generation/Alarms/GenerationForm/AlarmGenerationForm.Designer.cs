namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    partial class AlarmGenerationForm
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
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PlaceholdersLabel = new System.Windows.Forms.Label();
            this.configPanel = new System.Windows.Forms.Panel();
            this.configButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.fcConfigButton = new System.Windows.Forms.Button();
            this.alarmGenerationConfigButton = new System.Windows.Forms.Button();
            this.emptyAlarmGenerationConfigButton = new System.Windows.Forms.Button();
            this.fieldDefaultValueConfigButton = new System.Windows.Forms.Button();
            this.fieldPrefixConfigButton = new System.Windows.Forms.Button();
            this.segmentNameConfigButton = new System.Windows.Forms.Button();
            this.textListConfigButton = new System.Windows.Forms.Button();
            this.groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.divisionTypeLabel = new System.Windows.Forms.Label();
            this.partitionTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            this.memoryTypeLabel = new System.Windows.Forms.Label();
            this.GridsSplitPanel = new System.Windows.Forms.SplitContainer();
            this.mainPanel.SuspendLayout();
            this.TopTableLayoutPanel.SuspendLayout();
            this.TopMenuStrip.SuspendLayout();
            this.configPanel.SuspendLayout();
            this.configButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GridsSplitPanel)).BeginInit();
            this.GridsSplitPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.AutoSize = true;
            this.mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.TopTableLayoutPanel, 0, 0);
            this.mainPanel.Controls.Add(this.GridsSplitPanel, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 2;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(1384, 811);
            this.mainPanel.TabIndex = 1;
            // 
            // TopTableLayoutPanel
            // 
            this.TopTableLayoutPanel.AutoSize = true;
            this.TopTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopTableLayoutPanel.ColumnCount = 1;
            this.TopTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
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
            this.TopTableLayoutPanel.Size = new System.Drawing.Size(1378, 106);
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
            this.TopMenuStrip.Size = new System.Drawing.Size(3664, 24);
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
            this.exportXMLToolStripMenuItem});
            this.importExportToolStripMenuItem.Name = "importExportToolStripMenuItem";
            this.importExportToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.importExportToolStripMenuItem.Text = "Import/Export";
            // 
            // exportXMLToolStripMenuItem
            // 
            this.exportXMLToolStripMenuItem.Name = "exportXMLToolStripMenuItem";
            this.exportXMLToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.exportXMLToolStripMenuItem.Text = "Export XML";
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
            this.PlaceholdersLabel.Size = new System.Drawing.Size(3658, 16);
            this.PlaceholdersLabel.TabIndex = 9;
            this.PlaceholdersLabel.Text = "Placeholders: {device_name} {device_address} {device_description} {alarm_num_star" +
    "t} {alarm_num_end} {alarm_num} {alarm_description}";
            // 
            // configPanel
            // 
            this.configPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configPanel.Controls.Add(this.configButtonPanel);
            this.configPanel.Controls.Add(this.groupingTypeComboBox);
            this.configPanel.Controls.Add(this.divisionTypeLabel);
            this.configPanel.Controls.Add(this.partitionTypeComboBox);
            this.configPanel.Controls.Add(this.memoryTypeLabel);
            this.configPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configPanel.Location = new System.Drawing.Point(3, 43);
            this.configPanel.Name = "configPanel";
            this.configPanel.Size = new System.Drawing.Size(3658, 60);
            this.configPanel.TabIndex = 3;
            // 
            // configButtonPanel
            // 
            this.configButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.configButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.configButtonPanel.Controls.Add(this.fcConfigButton);
            this.configButtonPanel.Controls.Add(this.alarmGenerationConfigButton);
            this.configButtonPanel.Controls.Add(this.emptyAlarmGenerationConfigButton);
            this.configButtonPanel.Controls.Add(this.fieldDefaultValueConfigButton);
            this.configButtonPanel.Controls.Add(this.fieldPrefixConfigButton);
            this.configButtonPanel.Controls.Add(this.segmentNameConfigButton);
            this.configButtonPanel.Controls.Add(this.textListConfigButton);
            this.configButtonPanel.Location = new System.Drawing.Point(0, -1);
            this.configButtonPanel.Margin = new System.Windows.Forms.Padding(0);
            this.configButtonPanel.Name = "configButtonPanel";
            this.configButtonPanel.Size = new System.Drawing.Size(3658, 30);
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
            // alarmGenerationConfigButton
            // 
            this.alarmGenerationConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.alarmGenerationConfigButton.Location = new System.Drawing.Point(71, 0);
            this.alarmGenerationConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.alarmGenerationConfigButton.Name = "alarmGenerationConfigButton";
            this.alarmGenerationConfigButton.Size = new System.Drawing.Size(216, 30);
            this.alarmGenerationConfigButton.TabIndex = 11;
            this.alarmGenerationConfigButton.Text = "Generazione Allarmi";
            this.alarmGenerationConfigButton.UseVisualStyleBackColor = true;
            // 
            // emptyAlarmGenerationConfigButton
            // 
            this.emptyAlarmGenerationConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.emptyAlarmGenerationConfigButton.Location = new System.Drawing.Point(287, 0);
            this.emptyAlarmGenerationConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.emptyAlarmGenerationConfigButton.Name = "emptyAlarmGenerationConfigButton";
            this.emptyAlarmGenerationConfigButton.Size = new System.Drawing.Size(265, 30);
            this.emptyAlarmGenerationConfigButton.TabIndex = 15;
            this.emptyAlarmGenerationConfigButton.Text = "Generazione Allarmi Vuoti";
            this.emptyAlarmGenerationConfigButton.UseVisualStyleBackColor = true;
            // 
            // fieldDefaultValueConfigButton
            // 
            this.fieldDefaultValueConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.fieldDefaultValueConfigButton.Location = new System.Drawing.Point(552, 0);
            this.fieldDefaultValueConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.fieldDefaultValueConfigButton.Name = "fieldDefaultValueConfigButton";
            this.fieldDefaultValueConfigButton.Size = new System.Drawing.Size(210, 30);
            this.fieldDefaultValueConfigButton.TabIndex = 12;
            this.fieldDefaultValueConfigButton.Text = "Valori default campi";
            this.fieldDefaultValueConfigButton.UseVisualStyleBackColor = true;
            // 
            // fieldPrefixConfigButton
            // 
            this.fieldPrefixConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.fieldPrefixConfigButton.Location = new System.Drawing.Point(762, 0);
            this.fieldPrefixConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.fieldPrefixConfigButton.Name = "fieldPrefixConfigButton";
            this.fieldPrefixConfigButton.Size = new System.Drawing.Size(156, 30);
            this.fieldPrefixConfigButton.TabIndex = 13;
            this.fieldPrefixConfigButton.Text = "Prefissi Campi";
            this.fieldPrefixConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            this.segmentNameConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.segmentNameConfigButton.Location = new System.Drawing.Point(918, 0);
            this.segmentNameConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.segmentNameConfigButton.Name = "segmentNameConfigButton";
            this.segmentNameConfigButton.Size = new System.Drawing.Size(152, 30);
            this.segmentNameConfigButton.TabIndex = 14;
            this.segmentNameConfigButton.Text = "Nomi Segmenti";
            this.segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // textListConfigButton
            // 
            this.textListConfigButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold);
            this.textListConfigButton.Location = new System.Drawing.Point(1070, 0);
            this.textListConfigButton.Margin = new System.Windows.Forms.Padding(0);
            this.textListConfigButton.Name = "textListConfigButton";
            this.textListConfigButton.Size = new System.Drawing.Size(122, 30);
            this.textListConfigButton.TabIndex = 16;
            this.textListConfigButton.Text = "Lista testi";
            this.textListConfigButton.UseVisualStyleBackColor = true;
            // 
            // groupingTypeComboBox
            // 
            this.groupingTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupingTypeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupingTypeComboBox.FormattingEnabled = true;
            this.groupingTypeComboBox.Location = new System.Drawing.Point(377, 33);
            this.groupingTypeComboBox.Name = "groupingTypeComboBox";
            this.groupingTypeComboBox.Size = new System.Drawing.Size(133, 21);
            this.groupingTypeComboBox.TabIndex = 3;
            // 
            // divisionTypeLabel
            // 
            this.divisionTypeLabel.AutoSize = true;
            this.divisionTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.divisionTypeLabel.Location = new System.Drawing.Point(229, 34);
            this.divisionTypeLabel.Name = "divisionTypeLabel";
            this.divisionTypeLabel.Size = new System.Drawing.Size(149, 18);
            this.divisionTypeLabel.TabIndex = 2;
            this.divisionTypeLabel.Text = "Tipo raggruppamento";
            // 
            // partitionTypeComboBox
            // 
            this.partitionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.partitionTypeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.partitionTypeComboBox.FormattingEnabled = true;
            this.partitionTypeComboBox.Location = new System.Drawing.Point(123, 33);
            this.partitionTypeComboBox.Name = "partitionTypeComboBox";
            this.partitionTypeComboBox.Size = new System.Drawing.Size(100, 21);
            this.partitionTypeComboBox.TabIndex = 1;
            // 
            // memoryTypeLabel
            // 
            this.memoryTypeLabel.AutoSize = true;
            this.memoryTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryTypeLabel.Location = new System.Drawing.Point(9, 34);
            this.memoryTypeLabel.Name = "memoryTypeLabel";
            this.memoryTypeLabel.Size = new System.Drawing.Size(113, 18);
            this.memoryTypeLabel.TabIndex = 0;
            this.memoryTypeLabel.Text = "Tipo ripartizione";
            // 
            // GridsSplitPanel
            // 
            this.GridsSplitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridsSplitPanel.Location = new System.Drawing.Point(5, 114);
            this.GridsSplitPanel.Margin = new System.Windows.Forms.Padding(5);
            this.GridsSplitPanel.Name = "GridsSplitPanel";
            this.GridsSplitPanel.Size = new System.Drawing.Size(1374, 692);
            this.GridsSplitPanel.SplitterDistance = 550;
            this.GridsSplitPanel.SplitterWidth = 20;
            this.GridsSplitPanel.TabIndex = 19;
            // 
            // AlarmGenerationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1384, 811);
            this.Controls.Add(this.mainPanel);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.TopMenuStrip;
            this.Name = "AlarmGenerationForm";
            this.Text = "AlarmsForm";
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.TopTableLayoutPanel.ResumeLayout(false);
            this.TopTableLayoutPanel.PerformLayout();
            this.TopMenuStrip.ResumeLayout(false);
            this.TopMenuStrip.PerformLayout();
            this.configPanel.ResumeLayout(false);
            this.configPanel.PerformLayout();
            this.configButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GridsSplitPanel)).EndInit();
            this.GridsSplitPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public CustomControls.FlatComboBox groupingTypeComboBox;
        private System.Windows.Forms.Label divisionTypeLabel;
        public CustomControls.FlatComboBox partitionTypeComboBox;
        private System.Windows.Forms.Label memoryTypeLabel;
        private System.Windows.Forms.Label PlaceholdersLabel;
        public System.Windows.Forms.Button fcConfigButton;
        public System.Windows.Forms.Button alarmGenerationConfigButton;
        public System.Windows.Forms.Button fieldDefaultValueConfigButton;
        public System.Windows.Forms.Button fieldPrefixConfigButton;
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
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.SplitContainer GridsSplitPanel;
        public System.Windows.Forms.Button emptyAlarmGenerationConfigButton;
        public System.Windows.Forms.Button textListConfigButton;
    }
}
