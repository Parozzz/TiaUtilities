namespace TiaUtilities.Generation.GridHandler
{
    partial class GridFilterPopupControl
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
            topLabel = new Label();
            filterModeComboBox = new ComboBox();
            filterTextBox = new TextBox();
            selectedItemListView = new ListView();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 2;
            mainPanel.ColumnStyles.Add(new ColumnStyle());
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topLabel, 0, 0);
            mainPanel.Controls.Add(filterModeComboBox, 0, 1);
            mainPanel.Controls.Add(filterTextBox, 1, 1);
            mainPanel.Controls.Add(selectedItemListView, 0, 2);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 3;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Size = new Size(320, 450);
            mainPanel.TabIndex = 0;
            // 
            // topLabel
            // 
            topLabel.AutoSize = true;
            mainPanel.SetColumnSpan(topLabel, 2);
            topLabel.Dock = DockStyle.Fill;
            topLabel.Font = new Font("Segoe UI", 10F);
            topLabel.Location = new Point(3, 0);
            topLabel.Name = "topLabel";
            topLabel.Size = new Size(314, 23);
            topLabel.TabIndex = 0;
            topLabel.Text = "Filter on column";
            topLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // filterModeComboBox
            // 
            filterModeComboBox.Dock = DockStyle.Fill;
            filterModeComboBox.FormattingEnabled = true;
            filterModeComboBox.Location = new Point(3, 26);
            filterModeComboBox.MaximumSize = new Size(60, 0);
            filterModeComboBox.MinimumSize = new Size(60, 0);
            filterModeComboBox.Name = "filterModeComboBox";
            filterModeComboBox.Size = new Size(60, 28);
            filterModeComboBox.TabIndex = 1;
            // 
            // filterTextBox
            // 
            filterTextBox.Dock = DockStyle.Fill;
            filterTextBox.Location = new Point(69, 26);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new Size(248, 27);
            filterTextBox.TabIndex = 2;
            // 
            // selectedItemListView
            // 
            selectedItemListView.BorderStyle = BorderStyle.FixedSingle;
            selectedItemListView.CheckBoxes = true;
            mainPanel.SetColumnSpan(selectedItemListView, 2);
            selectedItemListView.Dock = DockStyle.Fill;
            selectedItemListView.Font = new Font("Segoe UI", 10F);
            selectedItemListView.Location = new Point(6, 63);
            selectedItemListView.Margin = new Padding(6);
            selectedItemListView.Name = "selectedItemListView";
            selectedItemListView.Size = new Size(308, 381);
            selectedItemListView.TabIndex = 3;
            selectedItemListView.UseCompatibleStateImageBehavior = false;
            // 
            // GridFilterPopupControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mainPanel);
            Name = "GridFilterPopupControl";
            Size = new Size(320, 450);
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private Label topLabel;
        private ComboBox filterModeComboBox;
        private TextBox filterTextBox;
        private ListView selectedItemListView;
    }
}
