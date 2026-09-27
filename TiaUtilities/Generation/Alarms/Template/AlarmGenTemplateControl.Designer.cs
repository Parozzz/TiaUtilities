using TiaUtilities.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TiaUtilities.Generation.Alarms.Template
{
    partial class AlarmGenTemplateControl
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
            mainPanel = new TableLayoutPanel();
            topPanel = new FlowLayoutPanel();
            selectPanel = new TableLayoutPanel();
            selectLabel = new Label();
            selectComboBox = new TiaUtilities.CustomControls.FlatComboBox();
            addButton = new Button();
            removeButton = new Button();
            renameButton = new Button();
            cloneButton = new Button();
            mainPanel.SuspendLayout();
            topPanel.SuspendLayout();
            selectPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 2;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Size = new Size(1584, 761);
            mainPanel.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            topPanel.Controls.Add(selectPanel);
            topPanel.Controls.Add(addButton);
            topPanel.Controls.Add(removeButton);
            topPanel.Controls.Add(renameButton);
            topPanel.Controls.Add(cloneButton);
            topPanel.Dock = DockStyle.Fill;
            topPanel.Location = new Point(3, 3);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(1578, 37);
            topPanel.TabIndex = 0;
            // 
            // selectPanel
            // 
            selectPanel.AutoSize = true;
            selectPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            selectPanel.ColumnCount = 2;
            selectPanel.ColumnStyles.Add(new ColumnStyle());
            selectPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            selectPanel.Controls.Add(selectLabel, 0, 0);
            selectPanel.Controls.Add(selectComboBox, 1, 0);
            selectPanel.Location = new Point(3, 3);
            selectPanel.Name = "selectPanel";
            selectPanel.RowCount = 1;
            selectPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            selectPanel.Size = new Size(265, 31);
            selectPanel.TabIndex = 0;
            // 
            // selectLabel
            // 
            selectLabel.AutoSize = true;
            selectLabel.Dock = DockStyle.Fill;
            selectLabel.Location = new Point(4, 1);
            selectLabel.Name = "selectLabel";
            selectLabel.Size = new Size(90, 29);
            selectLabel.TabIndex = 0;
            selectLabel.Text = "Select Template";
            selectLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // selectComboBox
            // 
            selectComboBox.BackColor = SystemColors.Control;
            selectComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            selectComboBox.FormattingEnabled = true;
            selectComboBox.Location = new Point(101, 4);
            selectComboBox.Name = "selectComboBox";
            selectComboBox.Size = new Size(160, 28);
            selectComboBox.TabIndex = 1;
            // 
            // addButton
            // 
            addButton.AutoSize = true;
            addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            addButton.FlatAppearance.BorderColor = SystemColors.Control;
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.Font = new Font("Segoe UI", 11F);
            addButton.Location = new Point(279, 0);
            addButton.Margin = new Padding(8, 0, 0, 0);
            addButton.Name = "addButton";
            addButton.Size = new Size(37, 37);
            addButton.TabIndex = 1;
            addButton.Text = "ADD";
            addButton.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            removeButton.AutoSize = true;
            removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            removeButton.FlatAppearance.BorderColor = SystemColors.Control;
            removeButton.FlatStyle = FlatStyle.Flat;
            removeButton.Font = new Font("Segoe UI", 11F);
            removeButton.Location = new Point(324, 0);
            removeButton.Margin = new Padding(8, 0, 0, 0);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(37, 37);
            removeButton.TabIndex = 2;
            removeButton.Text = "REMOVE";
            removeButton.UseVisualStyleBackColor = true;
            // 
            // renameButton
            // 
            renameButton.AutoSize = true;
            renameButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            renameButton.FlatAppearance.BorderColor = SystemColors.Control;
            renameButton.FlatStyle = FlatStyle.Flat;
            renameButton.Font = new Font("Segoe UI", 11F);
            renameButton.Location = new Point(369, 0);
            renameButton.Margin = new Padding(8, 0, 0, 0);
            renameButton.Name = "renameButton";
            renameButton.Size = new Size(40, 37);
            renameButton.TabIndex = 3;
            renameButton.Text = "RENAME";
            renameButton.UseVisualStyleBackColor = true;
            // 
            // cloneButton
            // 
            cloneButton.AutoSize = true;
            cloneButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            cloneButton.FlatAppearance.BorderColor = SystemColors.Control;
            cloneButton.FlatStyle = FlatStyle.Flat;
            cloneButton.Font = new Font("Segoe UI", 11F);
            cloneButton.Location = new Point(417, 0);
            cloneButton.Margin = new Padding(8, 0, 0, 0);
            cloneButton.Name = "cloneButton";
            cloneButton.Size = new Size(40, 37);
            cloneButton.TabIndex = 4;
            cloneButton.Text = "CLONE";
            cloneButton.UseVisualStyleBackColor = true;
            // 
            // AlarmGenTemplateControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainPanel);
            Name = "AlarmGenTemplateControl";
            Size = new Size(700, 250);
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            selectPanel.ResumeLayout(false);
            selectPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel topPanel;
        private TableLayoutPanel selectPanel;
        private Label selectLabel;
        private TiaUtilities.CustomControls.FlatComboBox selectComboBox;
        private Button addButton;
        private Button removeButton;
        private Button renameButton;
        private Button cloneButton;
    }
}
