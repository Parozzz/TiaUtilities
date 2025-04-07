namespace TiaUtilities.Generation.GenModules.Alarm.Tab
{
    partial class AlarmGenTabControl
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
            configFlowLayout = new FlowLayoutPanel();
            generationConfigButton = new Button();
            defaultValuesConfigButton = new Button();
            valuesPrefixesConfigButton = new Button();
            editTemplateConfigButton = new Button();
            groupingTypeTableLayout = new TableLayoutPanel();
            groupingTypeLabel = new Label();
            groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            mainTableLayout.SuspendLayout();
            configFlowLayout.SuspendLayout();
            groupingTypeTableLayout.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(configFlowLayout, 0, 0);
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
            configFlowLayout.Controls.Add(editTemplateConfigButton);
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
            generationConfigButton.Size = new Size(195, 36);
            generationConfigButton.TabIndex = 0;
            generationConfigButton.Text = "Generazione Codice";
            generationConfigButton.UseVisualStyleBackColor = true;
            // 
            // defaultValuesConfigButton
            // 
            defaultValuesConfigButton.AutoSize = true;
            defaultValuesConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            defaultValuesConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            defaultValuesConfigButton.Location = new Point(204, 3);
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
            valuesPrefixesConfigButton.Location = new Point(372, 3);
            valuesPrefixesConfigButton.Name = "valuesPrefixesConfigButton";
            valuesPrefixesConfigButton.Padding = new Padding(3);
            valuesPrefixesConfigButton.Size = new Size(145, 36);
            valuesPrefixesConfigButton.TabIndex = 2;
            valuesPrefixesConfigButton.Text = "Prefissi Valori";
            valuesPrefixesConfigButton.UseVisualStyleBackColor = true;
            // 
            // editTemplateConfigButton
            // 
            editTemplateConfigButton.AutoSize = true;
            editTemplateConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            editTemplateConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            editTemplateConfigButton.Location = new Point(523, 3);
            editTemplateConfigButton.Name = "editTemplateConfigButton";
            editTemplateConfigButton.Padding = new Padding(3);
            editTemplateConfigButton.Size = new Size(141, 36);
            editTemplateConfigButton.TabIndex = 3;
            editTemplateConfigButton.Text = "Edit Template";
            editTemplateConfigButton.UseVisualStyleBackColor = true;
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
            groupingTypeTableLayout.Location = new Point(670, 3);
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
            groupingTypeComboBox.Font = new Font("Microsoft Sans Serif", 11.25F);
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(135, 5);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(180, 26);
            groupingTypeComboBox.TabIndex = 0;
            // 
            // AlarmGenTabControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "AlarmGenTabControl";
            Size = new Size(1480, 600);
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            configFlowLayout.ResumeLayout(false);
            configFlowLayout.PerformLayout();
            groupingTypeTableLayout.ResumeLayout(false);
            groupingTypeTableLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel mainTableLayout;
        public FlowLayoutPanel configFlowLayout;
        public Button generationConfigButton;
        public Button defaultValuesConfigButton;
        public Button valuesPrefixesConfigButton;
        public TableLayoutPanel groupingTypeTableLayout;
        public Label groupingTypeLabel;
        public TiaXmlReader.CustomControls.FlatComboBox groupingTypeComboBox;
        public Button editTemplateConfigButton;
    }
}
