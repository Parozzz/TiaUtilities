namespace TiaUtilities.Generation.GenForms.Alarm
{
    partial class AlarmConfigControl
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
            PlaceholdersLabel = new Label();
            configPanel = new Panel();
            configButtonPanel = new FlowLayoutPanel();
            fcConfigButton = new Button();
            alarmGenerationConfigButton = new Button();
            fieldDefaultValueConfigButton = new Button();
            fieldPrefixConfigButton = new Button();
            segmentNameConfigButton = new Button();
            textListConfigButton = new Button();
            groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            groupingTypeLabel = new Label();
            partitionTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            partitionTypeLabel = new Label();
            mainTableLayout = new TableLayoutPanel();
            configPanel.SuspendLayout();
            configButtonPanel.SuspendLayout();
            mainTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // PlaceholdersLabel
            // 
            PlaceholdersLabel.Dock = DockStyle.Top;
            PlaceholdersLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            PlaceholdersLabel.Location = new Point(3, 0);
            PlaceholdersLabel.Name = "PlaceholdersLabel";
            PlaceholdersLabel.Size = new Size(1394, 16);
            PlaceholdersLabel.TabIndex = 10;
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
            configPanel.Location = new Point(3, 19);
            configPanel.Name = "configPanel";
            configPanel.Size = new Size(1394, 68);
            configPanel.TabIndex = 11;
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(fcConfigButton);
            configButtonPanel.Controls.Add(alarmGenerationConfigButton);
            configButtonPanel.Controls.Add(fieldDefaultValueConfigButton);
            configButtonPanel.Controls.Add(fieldPrefixConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);
            configButtonPanel.Controls.Add(textListConfigButton);
            configButtonPanel.Location = new Point(0, 0);
            configButtonPanel.Margin = new Padding(0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(4852, 30);
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
            groupingTypeComboBox.Location = new Point(433, 38);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(134, 23);
            groupingTypeComboBox.TabIndex = 3;
            // 
            // groupingTypeLabel
            // 
            groupingTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupingTypeLabel.Location = new Point(256, 41);
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
            partitionTypeComboBox.Location = new Point(142, 38);
            partitionTypeComboBox.Name = "partitionTypeComboBox";
            partitionTypeComboBox.Size = new Size(108, 23);
            partitionTypeComboBox.TabIndex = 1;
            // 
            // partitionTypeLabel
            // 
            partitionTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            partitionTypeLabel.Location = new Point(7, 40);
            partitionTypeLabel.Name = "partitionTypeLabel";
            partitionTypeLabel.Size = new Size(136, 18);
            partitionTypeLabel.TabIndex = 0;
            partitionTypeLabel.Text = "Tipo ripartizione";
            partitionTypeLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(PlaceholdersLabel, 0, 0);
            mainTableLayout.Controls.Add(configPanel, 0, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 2;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1400, 90);
            mainTableLayout.TabIndex = 16;
            // 
            // AlarmConfigControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mainTableLayout);
            Name = "AlarmConfigControl";
            Size = new Size(1400, 90);
            configPanel.ResumeLayout(false);
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            mainTableLayout.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label PlaceholdersLabel;
        private Panel configPanel;
        private FlowLayoutPanel configButtonPanel;
        public Button fcConfigButton;
        public Button alarmGenerationConfigButton;
        public Button fieldDefaultValueConfigButton;
        public Button fieldPrefixConfigButton;
        public Button segmentNameConfigButton;
        public Button textListConfigButton;
        public TiaXmlReader.CustomControls.FlatComboBox groupingTypeComboBox;
        public Label groupingTypeLabel;
        public TiaXmlReader.CustomControls.FlatComboBox partitionTypeComboBox;
        public Label partitionTypeLabel;
        private TableLayoutPanel mainTableLayout;
    }
}
