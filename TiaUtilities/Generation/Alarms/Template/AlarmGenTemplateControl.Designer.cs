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
            selectLabel = new Label();
            selectComboBox = new TiaUtilities.SettingsStep.CustomControls.ComboBoxFilterable();
            addButton = new Button();
            removeButton = new Button();
            renameButton = new Button();
            cloneButton = new Button();
            comboBoxFilterable1 = new TiaUtilities.SettingsStep.CustomControls.ComboBoxFilterable();
            comboBoxFilterable2 = new TiaUtilities.SettingsStep.CustomControls.ComboBoxFilterable();
            mainPanel.SuspendLayout();
            topPanel.SuspendLayout();
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
            mainPanel.Size = new Size(699, 251);
            mainPanel.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            topPanel.Controls.Add(selectLabel);
            topPanel.Controls.Add(selectComboBox);
            topPanel.Controls.Add(addButton);
            topPanel.Controls.Add(removeButton);
            topPanel.Controls.Add(renameButton);
            topPanel.Controls.Add(cloneButton);
            topPanel.Dock = DockStyle.Fill;
            topPanel.Location = new Point(3, 3);
            topPanel.Name = "topPanel";
            topPanel.Size = new Size(693, 37);
            topPanel.TabIndex = 0;
            // 
            // selectLabel
            // 
            selectLabel.AutoSize = true;
            selectLabel.Dock = DockStyle.Fill;
            selectLabel.Location = new Point(3, 0);
            selectLabel.Name = "selectLabel";
            selectLabel.Size = new Size(115, 37);
            selectLabel.TabIndex = 0;
            selectLabel.Text = "Select Template";
            selectLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // selectComboBox
            // 
            selectComboBox.Anchor = AnchorStyles.None;
            selectComboBox.AutoWidthFromItems = true;
            selectComboBox.AutoWidthRightPadding = 20;
            selectComboBox.DataSource = null;
            selectComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            selectComboBox.DropDownHoveredBackColor = Color.LightSeaGreen;
            selectComboBox.DropDownHoveredForeColor = Color.Black;
            selectComboBox.FilterPredicate = null;
            selectComboBox.FormattingEnabled = true;
            selectComboBox.Location = new Point(124, 4);
            selectComboBox.Margin = new Padding(3, 4, 3, 4);
            selectComboBox.Name = "selectComboBox";
            selectComboBox.Size = new Size(138, 28);
            selectComboBox.TabIndex = 5;
            // 
            // addButton
            // 
            addButton.AutoSize = true;
            addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            addButton.FlatAppearance.BorderColor = SystemColors.Control;
            addButton.FlatStyle = FlatStyle.Flat;
            addButton.Font = new Font("Segoe UI", 11F);
            addButton.Location = new Point(273, 0);
            addButton.Margin = new Padding(8, 0, 0, 0);
            addButton.Name = "addButton";
            addButton.Size = new Size(62, 37);
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
            removeButton.Location = new Point(343, 0);
            removeButton.Margin = new Padding(8, 0, 0, 0);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(98, 37);
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
            renameButton.Location = new Point(449, 0);
            renameButton.Margin = new Padding(8, 0, 0, 0);
            renameButton.Name = "renameButton";
            renameButton.Size = new Size(98, 37);
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
            cloneButton.Location = new Point(555, 0);
            cloneButton.Margin = new Padding(8, 0, 0, 0);
            cloneButton.Name = "cloneButton";
            cloneButton.Size = new Size(82, 37);
            cloneButton.TabIndex = 4;
            cloneButton.Text = "CLONE";
            cloneButton.UseVisualStyleBackColor = true;
            // 
            // comboBoxFilterable1
            // 
            comboBoxFilterable1.AutoWidthFromItems = true;
            comboBoxFilterable1.AutoWidthRightPadding = 20;
            comboBoxFilterable1.DataSource = null;
            comboBoxFilterable1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxFilterable1.DropDownHoveredBackColor = Color.LightSeaGreen;
            comboBoxFilterable1.DropDownHoveredForeColor = Color.Black;
            comboBoxFilterable1.FilterPredicate = null;
            comboBoxFilterable1.Location = new Point(0, 0);
            comboBoxFilterable1.Name = "comboBoxFilterable1";
            comboBoxFilterable1.Size = new Size(121, 28);
            comboBoxFilterable1.TabIndex = 0;
            // 
            // comboBoxFilterable2
            // 
            comboBoxFilterable2.AutoWidthFromItems = true;
            comboBoxFilterable2.AutoWidthRightPadding = 20;
            comboBoxFilterable2.DataSource = null;
            comboBoxFilterable2.DrawMode = DrawMode.OwnerDrawFixed;
            comboBoxFilterable2.DropDownHoveredBackColor = Color.LightSeaGreen;
            comboBoxFilterable2.DropDownHoveredForeColor = Color.Black;
            comboBoxFilterable2.FilterPredicate = null;
            comboBoxFilterable2.Location = new Point(0, 0);
            comboBoxFilterable2.Name = "comboBoxFilterable2";
            comboBoxFilterable2.Size = new Size(121, 28);
            comboBoxFilterable2.TabIndex = 0;
            // 
            // AlarmGenTemplateControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainPanel);
            Name = "AlarmGenTemplateControl";
            Size = new Size(699, 251);
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel topPanel;
        private Label selectLabel;
        private Button addButton;
        private Button removeButton;
        private Button renameButton;
        private Button cloneButton;
        private SettingsStep.CustomControls.ComboBoxFilterable comboBoxFilterable1;
        private SettingsStep.CustomControls.ComboBoxFilterable comboBoxFilterable2;
        private SettingsStep.CustomControls.ComboBoxFilterable selectComboBox;
    }
}
