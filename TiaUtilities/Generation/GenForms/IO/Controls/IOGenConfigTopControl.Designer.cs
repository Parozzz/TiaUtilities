using TiaUtilities.Generation.GenForms.Alarm;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
namespace TiaUtilities.Generation.GenForms.IO
{
    partial class IOGenConfigTopControl
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
            mainTableLayout = new TableLayoutPanel();
            configButtonPanel = new FlowLayoutPanel();
            dbConfigButton = new Button();
            variableTableConfigButton = new Button();
            ioTableConfigButton = new Button();
            memoryTypeTableLayout = new TableLayoutPanel();
            memoryTypeLabel = new Label();
            memoryTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            groupingTypeTableLayout = new TableLayoutPanel();
            groupingTypeLabel = new Label();
            groupingTypeComboBox = new TiaXmlReader.CustomControls.FlatComboBox();
            mainTableLayout.SuspendLayout();
            configButtonPanel.SuspendLayout();
            memoryTypeTableLayout.SuspendLayout();
            groupingTypeTableLayout.SuspendLayout();
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
            // mainTableLayout
            // 
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(PlaceholdersLabel, 0, 0);
            mainTableLayout.Controls.Add(configButtonPanel, 0, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 2;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1400, 60);
            mainTableLayout.TabIndex = 16;
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSize = true;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(memoryTypeTableLayout);
            configButtonPanel.Controls.Add(groupingTypeTableLayout);
            configButtonPanel.Controls.Add(ioTableConfigButton);
            configButtonPanel.Controls.Add(dbConfigButton);
            configButtonPanel.Controls.Add(variableTableConfigButton);
            configButtonPanel.Dock = DockStyle.Left;
            configButtonPanel.Location = new Point(0, 19);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(996, 41);
            configButtonPanel.TabIndex = 15;
            // 
            // dbConfigButton
            // 
            dbConfigButton.Anchor = AnchorStyles.None;
            dbConfigButton.AutoSize = true;
            dbConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            dbConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            dbConfigButton.Location = new Point(689, 4);
            dbConfigButton.Margin = new Padding(3, 0, 0, 0);
            dbConfigButton.Name = "dbConfigButton";
            dbConfigButton.Padding = new Padding(8, 0, 8, 0);
            dbConfigButton.Size = new Size(135, 30);
            dbConfigButton.TabIndex = 16;
            dbConfigButton.Text = "DB Appoggi";
            dbConfigButton.UseVisualStyleBackColor = true;
            // 
            // variableTableConfigButton
            // 
            variableTableConfigButton.Anchor = AnchorStyles.None;
            variableTableConfigButton.AutoSize = true;
            variableTableConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            variableTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            variableTableConfigButton.Location = new Point(827, 4);
            variableTableConfigButton.Margin = new Padding(3, 0, 0, 0);
            variableTableConfigButton.Name = "variableTableConfigButton";
            variableTableConfigButton.Padding = new Padding(8, 0, 8, 0);
            variableTableConfigButton.Size = new Size(169, 30);
            variableTableConfigButton.TabIndex = 17;
            variableTableConfigButton.Text = "Tabella Appoggi";
            variableTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // ioTableConfigButton
            // 
            ioTableConfigButton.Anchor = AnchorStyles.None;
            ioTableConfigButton.AutoSize = true;
            ioTableConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ioTableConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            ioTableConfigButton.Location = new Point(565, 4);
            ioTableConfigButton.Margin = new Padding(3, 0, 0, 0);
            ioTableConfigButton.Name = "ioTableConfigButton";
            ioTableConfigButton.Padding = new Padding(8, 0, 8, 0);
            ioTableConfigButton.Size = new Size(121, 30);
            ioTableConfigButton.TabIndex = 18;
            ioTableConfigButton.Text = "Tabella IO";
            ioTableConfigButton.UseVisualStyleBackColor = true;
            // 
            // memoryTypeTableLayout
            // 
            memoryTypeTableLayout.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            memoryTypeTableLayout.AutoSize = true;
            memoryTypeTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            memoryTypeTableLayout.ColumnCount = 2;
            memoryTypeTableLayout.ColumnStyles.Add(new ColumnStyle());
            memoryTypeTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            memoryTypeTableLayout.Controls.Add(memoryTypeLabel, 0, 0);
            memoryTypeTableLayout.Controls.Add(memoryTypeComboBox, 1, 0);
            memoryTypeTableLayout.Location = new Point(3, 3);
            memoryTypeTableLayout.Name = "memoryTypeTableLayout";
            memoryTypeTableLayout.RowCount = 1;
            memoryTypeTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            memoryTypeTableLayout.Size = new Size(232, 32);
            memoryTypeTableLayout.TabIndex = 20;
            // 
            // memoryTypeLabel
            // 
            memoryTypeLabel.AutoSize = true;
            memoryTypeLabel.Dock = DockStyle.Fill;
            memoryTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            memoryTypeLabel.Location = new Point(5, 5);
            memoryTypeLabel.Margin = new Padding(5);
            memoryTypeLabel.Name = "memoryTypeLabel";
            memoryTypeLabel.Size = new Size(86, 22);
            memoryTypeLabel.TabIndex = 0;
            memoryTypeLabel.Text = "Ripartizione";
            memoryTypeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // memoryTypeComboBox
            // 
            memoryTypeComboBox.Anchor = AnchorStyles.None;
            memoryTypeComboBox.BackColor = SystemColors.Control;
            memoryTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            memoryTypeComboBox.Font = new Font("Microsoft Sans Serif", 11.25F);
            memoryTypeComboBox.FormattingEnabled = true;
            memoryTypeComboBox.Location = new Point(99, 3);
            memoryTypeComboBox.Name = "memoryTypeComboBox";
            memoryTypeComboBox.Size = new Size(130, 26);
            memoryTypeComboBox.TabIndex = 0;
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
            groupingTypeTableLayout.Location = new Point(241, 3);
            groupingTypeTableLayout.Name = "groupingTypeTableLayout";
            groupingTypeTableLayout.RowCount = 1;
            groupingTypeTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            groupingTypeTableLayout.Size = new Size(318, 32);
            groupingTypeTableLayout.TabIndex = 21;
            // 
            // groupingTypeLabel
            // 
            groupingTypeLabel.AutoSize = true;
            groupingTypeLabel.Dock = DockStyle.Fill;
            groupingTypeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupingTypeLabel.Location = new Point(5, 5);
            groupingTypeLabel.Margin = new Padding(5);
            groupingTypeLabel.Name = "groupingTypeLabel";
            groupingTypeLabel.Size = new Size(122, 22);
            groupingTypeLabel.TabIndex = 0;
            groupingTypeLabel.Text = "Raggruppamento";
            groupingTypeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupingTypeComboBox
            // 
            groupingTypeComboBox.Anchor = AnchorStyles.None;
            groupingTypeComboBox.BackColor = SystemColors.Control;
            groupingTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            groupingTypeComboBox.Font = new Font("Microsoft Sans Serif", 11.25F);
            groupingTypeComboBox.FormattingEnabled = true;
            groupingTypeComboBox.Location = new Point(135, 3);
            groupingTypeComboBox.Name = "groupingTypeComboBox";
            groupingTypeComboBox.Size = new Size(180, 26);
            groupingTypeComboBox.TabIndex = 0;
            // 
            // IOGenConfigTopControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "IOGenConfigTopControl";
            Size = new Size(1400, 60);
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            memoryTypeTableLayout.ResumeLayout(false);
            memoryTypeTableLayout.PerformLayout();
            groupingTypeTableLayout.ResumeLayout(false);
            groupingTypeTableLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label PlaceholdersLabel;
        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel configButtonPanel;
        public Button dbConfigButton;
        public Button variableTableConfigButton;
        public Button ioTableConfigButton;
        public TableLayoutPanel memoryTypeTableLayout;
        public Label memoryTypeLabel;
        public TiaXmlReader.CustomControls.FlatComboBox memoryTypeComboBox;
        public TableLayoutPanel groupingTypeTableLayout;
        public Label groupingTypeLabel;
        public TiaXmlReader.CustomControls.FlatComboBox groupingTypeComboBox;
    }
}
