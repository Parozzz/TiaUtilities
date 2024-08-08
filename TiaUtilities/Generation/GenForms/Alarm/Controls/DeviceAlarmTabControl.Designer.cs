using TiaUtilities.Generation.GenForms.Alarm;
namespace TiaUtilities.Generation.GenForms.Alarm.Controls
{
    partial class DeviceAlarmTabControl
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
            configFlowLayout = new FlowLayoutPanel();
            generationConfigButton = new Button();
            defaultValuesConfigButton = new Button();
            valuesPrefixesConfigButton = new Button();
            partitionTypeTableLayout = new TableLayoutPanel();
            partitionTypeLabel = new Label();
            partitionTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            groupingTypeTableLayout = new TableLayoutPanel();
            groupingTypeLabel = new Label();
            groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            ((System.ComponentModel.ISupportInitialize)gridSplitContainer).BeginInit();
            gridSplitContainer.SuspendLayout();
            mainTableLayout.SuspendLayout();
            configFlowLayout.SuspendLayout();
            partitionTypeTableLayout.SuspendLayout();
            groupingTypeTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // gridSplitContainer
            // 
            gridSplitContainer.Dock = DockStyle.Fill;
            gridSplitContainer.Location = new Point(3, 51);
            gridSplitContainer.Name = "gridSplitContainer";
            gridSplitContainer.Size = new Size(1474, 546);
            gridSplitContainer.SplitterDistance = 549;
            gridSplitContainer.SplitterWidth = 20;
            gridSplitContainer.TabIndex = 0;
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(configFlowLayout, 0, 0);
            mainTableLayout.Controls.Add(gridSplitContainer, 0, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 2;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1480, 600);
            mainTableLayout.TabIndex = 0;
            // 
            // configFlowLayout
            // 
            configFlowLayout.AutoSize = true;
            configFlowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configFlowLayout.Controls.Add(generationConfigButton);
            configFlowLayout.Controls.Add(defaultValuesConfigButton);
            configFlowLayout.Controls.Add(valuesPrefixesConfigButton);
            configFlowLayout.Controls.Add(partitionTypeTableLayout);
            configFlowLayout.Controls.Add(groupingTypeTableLayout);
            configFlowLayout.Dock = DockStyle.Fill;
            configFlowLayout.Location = new Point(3, 3);
            configFlowLayout.Name = "configFlowLayout";
            configFlowLayout.Size = new Size(1474, 42);
            configFlowLayout.TabIndex = 0;
            // 
            // generationConfigButton
            // 
            generationConfigButton.AutoSize = true;
            generationConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            generationConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            generationConfigButton.Location = new Point(3, 3);
            generationConfigButton.Name = "generationConfigButton";
            generationConfigButton.Padding = new Padding(3);
            generationConfigButton.Size = new Size(131, 36);
            generationConfigButton.TabIndex = 0;
            generationConfigButton.Text = "Generazione";
            generationConfigButton.UseVisualStyleBackColor = true;
            // 
            // defaultValuesConfigButton
            // 
            defaultValuesConfigButton.AutoSize = true;
            defaultValuesConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            defaultValuesConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            defaultValuesConfigButton.Location = new Point(140, 3);
            defaultValuesConfigButton.Name = "defaultValuesConfigButton";
            defaultValuesConfigButton.Padding = new Padding(3);
            defaultValuesConfigButton.Size = new Size(162, 36);
            defaultValuesConfigButton.TabIndex = 1;
            defaultValuesConfigButton.Text = "Valori di Default";
            defaultValuesConfigButton.UseVisualStyleBackColor = true;
            // 
            // valuesPrefixesConfigButton
            // 
            valuesPrefixesConfigButton.AutoSize = true;
            valuesPrefixesConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            valuesPrefixesConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            valuesPrefixesConfigButton.Location = new Point(308, 3);
            valuesPrefixesConfigButton.Name = "valuesPrefixesConfigButton";
            valuesPrefixesConfigButton.Padding = new Padding(3);
            valuesPrefixesConfigButton.Size = new Size(145, 36);
            valuesPrefixesConfigButton.TabIndex = 2;
            valuesPrefixesConfigButton.Text = "Prefissi Valori";
            valuesPrefixesConfigButton.UseVisualStyleBackColor = true;
            // 
            // partitionTypeTableLayout
            // 
            partitionTypeTableLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            partitionTypeTableLayout.AutoSize = true;
            partitionTypeTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            partitionTypeTableLayout.ColumnCount = 2;
            partitionTypeTableLayout.ColumnStyles.Add(new ColumnStyle());
            partitionTypeTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            partitionTypeTableLayout.Controls.Add(partitionTypeLabel, 0, 0);
            partitionTypeTableLayout.Controls.Add(partitionTypeComboBox, 1, 0);
            partitionTypeTableLayout.Location = new Point(459, 3);
            partitionTypeTableLayout.Name = "partitionTypeTableLayout";
            partitionTypeTableLayout.RowCount = 1;
            partitionTypeTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            partitionTypeTableLayout.Size = new Size(232, 36);
            partitionTypeTableLayout.TabIndex = 0;
            // 
            // partitionTypeLabel
            // 
            partitionTypeLabel.AutoSize = true;
            partitionTypeLabel.Dock = DockStyle.Fill;
            partitionTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            partitionTypeLabel.Location = new Point(5, 5);
            partitionTypeLabel.Margin = new Padding(5);
            partitionTypeLabel.Name = "partitionTypeLabel";
            partitionTypeLabel.Size = new Size(86, 26);
            partitionTypeLabel.TabIndex = 0;
            partitionTypeLabel.Text = "Ripartizione";
            partitionTypeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // partitionTypeComboBox
            // 
            partitionTypeComboBox.Anchor = AnchorStyles.None;
            partitionTypeComboBox.BackColor = SystemColors.Control;
            partitionTypeComboBox.BorderColor = SystemColors.Control;
            partitionTypeComboBox.ButtonColor = Color.Gray;
            partitionTypeComboBox.Font = new Font("Microsoft Sans Serif", 11.25F);
            partitionTypeComboBox.FormattingEnabled = true;
            partitionTypeComboBox.Location = new Point(99, 5);
            partitionTypeComboBox.Name = "partitionTypeComboBox";
            partitionTypeComboBox.Size = new Size(130, 26);
            partitionTypeComboBox.TabIndex = 0;
            // 
            // groupingTypeTableLayout
            // 
            groupingTypeTableLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            groupingTypeTableLayout.AutoSize = true;
            groupingTypeTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupingTypeTableLayout.ColumnCount = 2;
            groupingTypeTableLayout.ColumnStyles.Add(new ColumnStyle());
            groupingTypeTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            groupingTypeTableLayout.Controls.Add(groupingTypeLabel, 0, 0);
            groupingTypeTableLayout.Controls.Add(groupingTypeComboBox, 1, 0);
            groupingTypeTableLayout.Location = new Point(697, 3);
            groupingTypeTableLayout.Name = "groupingTypeTableLayout";
            groupingTypeTableLayout.RowCount = 1;
            groupingTypeTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            groupingTypeTableLayout.Size = new Size(318, 36);
            groupingTypeTableLayout.TabIndex = 1;
            // 
            // groupingTypeLabel
            // 
            groupingTypeLabel.AutoSize = true;
            groupingTypeLabel.Dock = DockStyle.Fill;
            groupingTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupingTypeLabel.Location = new Point(5, 5);
            groupingTypeLabel.Margin = new Padding(5);
            groupingTypeLabel.Name = "groupingTypeLabel";
            groupingTypeLabel.Size = new Size(122, 26);
            groupingTypeLabel.TabIndex = 0;
            groupingTypeLabel.Text = "Raggruppamento";
            groupingTypeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupingTypeComboBox
            // 
            groupingTypeComboBox.Anchor = AnchorStyles.None;
            groupingTypeComboBox.BackColor = SystemColors.Control;
            groupingTypeComboBox.BorderColor = SystemColors.Control;
            groupingTypeComboBox.ButtonColor = Color.DarkGray;
            groupingTypeComboBox.Font = new Font("Microsoft Sans Serif", 11.25F);
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(135, 5);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(180, 26);
            groupingTypeComboBox.TabIndex = 0;
            // 
            // DeviceAlarmTabControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "DeviceAlarmTabControl";
            Size = new Size(1480, 600);
            ((System.ComponentModel.ISupportInitialize)gridSplitContainer).EndInit();
            gridSplitContainer.ResumeLayout(false);
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            configFlowLayout.ResumeLayout(false);
            configFlowLayout.PerformLayout();
            partitionTypeTableLayout.ResumeLayout(false);
            partitionTypeTableLayout.PerformLayout();
            groupingTypeTableLayout.ResumeLayout(false);
            groupingTypeTableLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer gridSplitContainer;
        private TableLayoutPanel mainTableLayout;
        public FlowLayoutPanel configFlowLayout;
        public Button generationConfigButton;
        public Button defaultValuesConfigButton;
        public Button valuesPrefixesConfigButton;
        public TableLayoutPanel partitionTypeTableLayout;
        public Label partitionTypeLabel;
        public TableLayoutPanel groupingTypeTableLayout;
        public Label groupingTypeLabel;
        public TiaXmlReader.CustomControls.FlatComboBox partitionTypeComboBox;
        public TiaXmlReader.CustomControls.FlatComboBox groupingTypeComboBox;
    }
}
