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
            mainPanel = new TableLayoutPanel();
            TopTableLayoutPanel = new TableLayoutPanel();
            TopMenuStrip = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            importExportMenuItem = new ToolStripMenuItem();
            exportXMLMenuItem = new ToolStripMenuItem();
            preferencesMenuItem = new ToolStripMenuItem();
            PlaceholdersLabel = new Label();
            configPanel = new Panel();
            configButtonPanel = new FlowLayoutPanel();
            fcConfigButton = new Button();
            alarmGenerationConfigButton = new Button();
            fieldDefaultValueConfigButton = new Button();
            fieldPrefixConfigButton = new Button();
            segmentNameConfigButton = new Button();
            textListConfigButton = new Button();
            groupingTypeComboBox = new CustomControls.FlatComboBox();
            groupingTypeLabel = new Label();
            partitionTypeComboBox = new CustomControls.FlatComboBox();
            partitionTypeLabel = new Label();
            GridsSplitPanel = new SplitContainer();
            mainPanel.SuspendLayout();
            TopTableLayoutPanel.SuspendLayout();
            TopMenuStrip.SuspendLayout();
            configPanel.SuspendLayout();
            configButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridsSplitPanel).BeginInit();
            GridsSplitPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(TopTableLayoutPanel, 0, 0);
            mainPanel.Controls.Add(GridsSplitPanel, 0, 1);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Margin = new Padding(0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Size = new Size(1384, 811);
            mainPanel.TabIndex = 1;
            // 
            // TopTableLayoutPanel
            // 
            TopTableLayoutPanel.AutoSize = true;
            TopTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TopTableLayoutPanel.ColumnCount = 1;
            TopTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
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
            TopTableLayoutPanel.Size = new Size(1378, 106);
            TopTableLayoutPanel.TabIndex = 18;
            // 
            // TopMenuStrip
            // 
            TopMenuStrip.BackColor = Color.Transparent;
            TopMenuStrip.GripStyle = ToolStripGripStyle.Visible;
            TopMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, importExportMenuItem, preferencesMenuItem });
            TopMenuStrip.Location = new Point(0, 0);
            TopMenuStrip.Name = "TopMenuStrip";
            TopMenuStrip.Size = new Size(3664, 24);
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
            importExportMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportXMLMenuItem });
            importExportMenuItem.Name = "importExportMenuItem";
            importExportMenuItem.Size = new Size(94, 20);
            importExportMenuItem.Text = "Import/Export";
            // 
            // exportXMLMenuItem
            // 
            exportXMLMenuItem.Name = "exportXMLMenuItem";
            exportXMLMenuItem.Size = new Size(135, 22);
            exportXMLMenuItem.Text = "Export XML";
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
            PlaceholdersLabel.Size = new Size(3658, 16);
            PlaceholdersLabel.TabIndex = 9;
            PlaceholdersLabel.Text = "Placeholders: {device_name} {device_address} {device_description} {alarm_num_start} {alarm_num_end} {alarm_num} {alarm_description}";
            // 
            // configPanel
            // 
            configPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configPanel.Controls.Add(configButtonPanel);
            configPanel.Controls.Add(groupingTypeComboBox);
            configPanel.Controls.Add(groupingTypeLabel);
            configPanel.Controls.Add(partitionTypeComboBox);
            configPanel.Controls.Add(partitionTypeLabel);
            configPanel.Dock = DockStyle.Fill;
            configPanel.Location = new Point(3, 43);
            configPanel.Name = "configPanel";
            configPanel.Size = new Size(3658, 60);
            configPanel.TabIndex = 3;
            // 
            // configButtonPanel
            // 
            configButtonPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(fcConfigButton);
            configButtonPanel.Controls.Add(alarmGenerationConfigButton);
            configButtonPanel.Controls.Add(fieldDefaultValueConfigButton);
            configButtonPanel.Controls.Add(fieldPrefixConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);
            configButtonPanel.Controls.Add(textListConfigButton);
            configButtonPanel.Location = new Point(0, -1);
            configButtonPanel.Margin = new Padding(0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(3658, 30);
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
            // alarmGenerationConfigButton
            // 
            alarmGenerationConfigButton.AutoSize = true;
            alarmGenerationConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            alarmGenerationConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            alarmGenerationConfigButton.Location = new Point(65, 0);
            alarmGenerationConfigButton.Margin = new Padding(3, 0, 0, 0);
            alarmGenerationConfigButton.Name = "alarmGenerationConfigButton";
            alarmGenerationConfigButton.Padding = new Padding(8, 0, 8, 0);
            alarmGenerationConfigButton.Size = new Size(206, 30);
            alarmGenerationConfigButton.TabIndex = 11;
            alarmGenerationConfigButton.Text = "Generazione Allarmi";
            alarmGenerationConfigButton.UseVisualStyleBackColor = true;
            // 
            // fieldDefaultValueConfigButton
            // 
            fieldDefaultValueConfigButton.AutoSize = true;
            fieldDefaultValueConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fieldDefaultValueConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            fieldDefaultValueConfigButton.Location = new Point(274, 0);
            fieldDefaultValueConfigButton.Margin = new Padding(3, 0, 0, 0);
            fieldDefaultValueConfigButton.Name = "fieldDefaultValueConfigButton";
            fieldDefaultValueConfigButton.Padding = new Padding(8, 0, 8, 0);
            fieldDefaultValueConfigButton.Size = new Size(203, 30);
            fieldDefaultValueConfigButton.TabIndex = 12;
            fieldDefaultValueConfigButton.Text = "Valori default campi";
            fieldDefaultValueConfigButton.UseVisualStyleBackColor = true;
            // 
            // fieldPrefixConfigButton
            // 
            fieldPrefixConfigButton.AutoSize = true;
            fieldPrefixConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fieldPrefixConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            fieldPrefixConfigButton.Location = new Point(480, 0);
            fieldPrefixConfigButton.Margin = new Padding(3, 0, 0, 0);
            fieldPrefixConfigButton.Name = "fieldPrefixConfigButton";
            fieldPrefixConfigButton.Padding = new Padding(8, 0, 8, 0);
            fieldPrefixConfigButton.Size = new Size(159, 30);
            fieldPrefixConfigButton.TabIndex = 13;
            fieldPrefixConfigButton.Text = "Prefissi Campi";
            fieldPrefixConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            segmentNameConfigButton.AutoSize = true;
            segmentNameConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            segmentNameConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            segmentNameConfigButton.Location = new Point(642, 0);
            segmentNameConfigButton.Margin = new Padding(3, 0, 0, 0);
            segmentNameConfigButton.Name = "segmentNameConfigButton";
            segmentNameConfigButton.Padding = new Padding(8, 0, 8, 0);
            segmentNameConfigButton.Size = new Size(162, 30);
            segmentNameConfigButton.TabIndex = 14;
            segmentNameConfigButton.Text = "Nomi Segmenti";
            segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // textListConfigButton
            // 
            textListConfigButton.AutoSize = true;
            textListConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            textListConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            textListConfigButton.Location = new Point(807, 0);
            textListConfigButton.Margin = new Padding(3, 0, 0, 0);
            textListConfigButton.Name = "textListConfigButton";
            textListConfigButton.Padding = new Padding(8, 0, 8, 0);
            textListConfigButton.Size = new Size(120, 30);
            textListConfigButton.TabIndex = 16;
            textListConfigButton.Text = "Lista testi";
            textListConfigButton.UseVisualStyleBackColor = true;
            // 
            // groupingTypeComboBox
            // 
            groupingTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            groupingTypeComboBox.FlatStyle = FlatStyle.Flat;
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(434, 33);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(134, 23);
            groupingTypeComboBox.TabIndex = 3;
            // 
            // groupingTypeLabel
            // 
            groupingTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupingTypeLabel.Location = new Point(257, 35);
            groupingTypeLabel.Name = "groupingTypeLabel";
            groupingTypeLabel.Size = new Size(176, 18);
            groupingTypeLabel.TabIndex = 2;
            groupingTypeLabel.Text = "Tipo raggruppamento";
            groupingTypeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // partitionTypeComboBox
            // 
            partitionTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            partitionTypeComboBox.FlatStyle = FlatStyle.Flat;
            partitionTypeComboBox.FormattingEnabled = true;
            partitionTypeComboBox.Location = new Point(143, 33);
            partitionTypeComboBox.Name = "partitionTypeComboBox";
            partitionTypeComboBox.Size = new Size(108, 23);
            partitionTypeComboBox.TabIndex = 1;
            // 
            // partitionTypeLabel
            // 
            partitionTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            partitionTypeLabel.Location = new Point(8, 34);
            partitionTypeLabel.Name = "partitionTypeLabel";
            partitionTypeLabel.Size = new Size(136, 18);
            partitionTypeLabel.TabIndex = 0;
            partitionTypeLabel.Text = "Tipo ripartizione";
            partitionTypeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // GridsSplitPanel
            // 
            GridsSplitPanel.Dock = DockStyle.Fill;
            GridsSplitPanel.Location = new Point(5, 114);
            GridsSplitPanel.Margin = new Padding(5);
            GridsSplitPanel.Name = "GridsSplitPanel";
            GridsSplitPanel.Size = new Size(1374, 692);
            GridsSplitPanel.SplitterDistance = 550;
            GridsSplitPanel.SplitterWidth = 20;
            GridsSplitPanel.TabIndex = 19;
            // 
            // AlarmGenerationForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1384, 811);
            Controls.Add(mainPanel);
            DoubleBuffered = true;
            MainMenuStrip = TopMenuStrip;
            Name = "AlarmGenerationForm";
            Text = "AlarmsForm";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            TopTableLayoutPanel.ResumeLayout(false);
            TopTableLayoutPanel.PerformLayout();
            TopMenuStrip.ResumeLayout(false);
            TopMenuStrip.PerformLayout();
            configPanel.ResumeLayout(false);
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GridsSplitPanel).EndInit();
            GridsSplitPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Panel configPanel;
        public CustomControls.FlatComboBox groupingTypeComboBox;
        public CustomControls.FlatComboBox partitionTypeComboBox;
        private System.Windows.Forms.Label PlaceholdersLabel;
        public System.Windows.Forms.Button fcConfigButton;
        public System.Windows.Forms.Button alarmGenerationConfigButton;
        public System.Windows.Forms.Button fieldDefaultValueConfigButton;
        public System.Windows.Forms.Button fieldPrefixConfigButton;
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
        private System.Windows.Forms.ToolStripMenuItem preferencesMenuItem;
        private System.Windows.Forms.SplitContainer GridsSplitPanel;
        public System.Windows.Forms.Button textListConfigButton;
        public Label partitionTypeLabel;
        public Label groupingTypeLabel;
    }
}
