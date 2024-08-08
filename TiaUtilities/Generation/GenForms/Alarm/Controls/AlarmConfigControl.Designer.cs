using TiaUtilities.Generation.GenForms.Alarm;
namespace TiaUtilities.Generation.GenForms.Alarm.Controls
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
            mainTableLayout = new TableLayoutPanel();
            configButtonPanel = new FlowLayoutPanel();
            fcConfigButton = new Button();
            segmentNameConfigButton = new Button();
            textListConfigButton = new Button();
            mainTableLayout.SuspendLayout();
            configButtonPanel.SuspendLayout();
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
            mainTableLayout.Size = new Size(1400, 50);
            mainTableLayout.TabIndex = 16;
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(fcConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);
            configButtonPanel.Controls.Add(textListConfigButton);
            configButtonPanel.Dock = DockStyle.Fill;
            configButtonPanel.Location = new Point(0, 19);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(1400, 31);
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
            // segmentNameConfigButton
            // 
            segmentNameConfigButton.AutoSize = true;
            segmentNameConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            segmentNameConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            segmentNameConfigButton.Location = new Point(65, 0);
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
            textListConfigButton.Location = new Point(230, 0);
            textListConfigButton.Margin = new Padding(3, 0, 0, 0);
            textListConfigButton.Name = "textListConfigButton";
            textListConfigButton.Padding = new Padding(8, 0, 8, 0);
            textListConfigButton.Size = new Size(120, 30);
            textListConfigButton.TabIndex = 16;
            textListConfigButton.Text = "Lista testi";
            textListConfigButton.UseVisualStyleBackColor = true;
            // 
            // AlarmConfigControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "AlarmConfigControl";
            Size = new Size(1400, 50);
            mainTableLayout.ResumeLayout(false);
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label PlaceholdersLabel;
        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel configButtonPanel;
        public Button fcConfigButton;
        public Button segmentNameConfigButton;
        public Button textListConfigButton;
    }
}
