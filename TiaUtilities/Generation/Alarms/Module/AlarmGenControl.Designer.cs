using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.Alarms.Module
{
    partial class AlarmGenControl
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
            tabControl = new InteractableTabControl();
            templateButton = new Button();
            settingsButton = new Button();
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
            PlaceholdersLabel.Text = "Placeholders: {tab_name} {device_name} {device_description} {alarm_num_start} {alarm_num_end} {alarm_num} {alarm_description}";
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSize = true;
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTableLayout.Controls.Add(PlaceholdersLabel, 0, 0);
            mainTableLayout.Controls.Add(configButtonPanel, 0, 1);
            mainTableLayout.Controls.Add(tabControl, 0, 2);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 3;
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle());
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.Size = new Size(1400, 729);
            mainTableLayout.TabIndex = 16;
            // 
            // configButtonPanel
            // 
            configButtonPanel.AutoSize = true;
            configButtonPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configButtonPanel.Controls.Add(settingsButton);
            configButtonPanel.Controls.Add(templateButton);
            configButtonPanel.Dock = DockStyle.Left;
            configButtonPanel.Location = new Point(0, 19);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(222, 30);
            configButtonPanel.TabIndex = 15;
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Location = new Point(3, 52);
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(12, 5);
            tabControl.RequireConfirmationBeforeClosing = true;
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1394, 674);
            tabControl.TabIndex = 16;
            // 
            // templateButton
            // 
            templateButton.AutoSize = true;
            templateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            templateButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            templateButton.Location = new Point(110, 0);
            templateButton.Margin = new Padding(3, 0, 0, 0);
            templateButton.Name = "templateButton";
            templateButton.Padding = new Padding(8, 0, 8, 0);
            templateButton.Size = new Size(112, 30);
            templateButton.TabIndex = 22;
            templateButton.Text = "Template";
            templateButton.UseVisualStyleBackColor = true;
            // 
            // settingsButton
            // 
            settingsButton.AutoSize = true;
            settingsButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            settingsButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            settingsButton.Location = new Point(3, 0);
            settingsButton.Margin = new Padding(3, 0, 0, 0);
            settingsButton.Name = "settingsButton";
            settingsButton.Padding = new Padding(8, 0, 8, 0);
            settingsButton.Size = new Size(104, 30);
            settingsButton.TabIndex = 23;
            settingsButton.Text = "Settings";
            settingsButton.UseVisualStyleBackColor = true;
            // 
            // AlarmGenControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTableLayout);
            Name = "AlarmGenControl";
            Size = new Size(1400, 729);
            mainTableLayout.ResumeLayout(false);
            mainTableLayout.PerformLayout();
            configButtonPanel.ResumeLayout(false);
            configButtonPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label PlaceholdersLabel;
        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel configButtonPanel;
        public InteractableTabControl tabControl;
        public Button settingsButton;
        public Button templateButton;
    }
}
