using TiaUtilities.CustomControls;

namespace TiaUtilities.Generation.GenModules.Alarm
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
            fcConfigButton = new Button();
            segmentNameConfigButton = new Button();
            textListConfigButton = new Button();
            enableCustomVarPanel = new TableLayoutPanel();
            enableCustomVarToggleButton = new RJToggleButton();
            enableCustomVarLabel = new Label();
            enableTimerPanel = new TableLayoutPanel();
            enableTimerToggleButton = new RJToggleButton();
            enableTimerLabel = new Label();
            tabControl = new InteractableTabControl();
            mainTableLayout.SuspendLayout();
            configButtonPanel.SuspendLayout();
            enableCustomVarPanel.SuspendLayout();
            enableTimerPanel.SuspendLayout();
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
            configButtonPanel.Controls.Add(fcConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);
            configButtonPanel.Controls.Add(textListConfigButton);
            configButtonPanel.Controls.Add(enableCustomVarPanel);
            configButtonPanel.Controls.Add(enableTimerPanel);
            configButtonPanel.Dock = DockStyle.Left;
            configButtonPanel.Location = new Point(0, 19);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(824, 30);
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
            // enableCustomVarPanel
            // 
            enableCustomVarPanel.AutoSize = true;
            enableCustomVarPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            enableCustomVarPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            enableCustomVarPanel.ColumnCount = 2;
            enableCustomVarPanel.ColumnStyles.Add(new ColumnStyle());
            enableCustomVarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            enableCustomVarPanel.Controls.Add(enableCustomVarToggleButton, 1, 0);
            enableCustomVarPanel.Controls.Add(enableCustomVarLabel, 0, 0);
            enableCustomVarPanel.Location = new Point(353, 0);
            enableCustomVarPanel.Margin = new Padding(3, 0, 0, 0);
            enableCustomVarPanel.Name = "enableCustomVarPanel";
            enableCustomVarPanel.Padding = new Padding(3, 0, 3, 0);
            enableCustomVarPanel.RowCount = 1;
            enableCustomVarPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            enableCustomVarPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            enableCustomVarPanel.Size = new Size(266, 30);
            enableCustomVarPanel.TabIndex = 19;
            // 
            // enableCustomVarToggleButton
            // 
            enableCustomVarToggleButton.AutoSize = true;
            enableCustomVarToggleButton.BorderColor = Color.Gray;
            enableCustomVarToggleButton.BorderWidth = 2;
            enableCustomVarToggleButton.Location = new Point(214, 4);
            enableCustomVarToggleButton.MinimumSize = new Size(45, 22);
            enableCustomVarToggleButton.Name = "enableCustomVarToggleButton";
            enableCustomVarToggleButton.OffBackColor = Color.Gray;
            enableCustomVarToggleButton.OffToggleColor = Color.Gainsboro;
            enableCustomVarToggleButton.OnBackColor = Color.MediumSlateBlue;
            enableCustomVarToggleButton.OnToggleColor = Color.WhiteSmoke;
            enableCustomVarToggleButton.Size = new Size(45, 22);
            enableCustomVarToggleButton.TabIndex = 18;
            enableCustomVarToggleButton.ToggleWidthPercentage = 15;
            enableCustomVarToggleButton.UseVisualStyleBackColor = true;
            // 
            // enableCustomVarLabel
            // 
            enableCustomVarLabel.AutoSize = true;
            enableCustomVarLabel.Dock = DockStyle.Fill;
            enableCustomVarLabel.Font = new Font("Microsoft Sans Serif", 12.75F);
            enableCustomVarLabel.Location = new Point(7, 1);
            enableCustomVarLabel.Name = "enableCustomVarLabel";
            enableCustomVarLabel.Size = new Size(200, 28);
            enableCustomVarLabel.TabIndex = 19;
            enableCustomVarLabel.Text = "ENABLE_CUSTOM_VAR";
            enableCustomVarLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // enableTimerPanel
            // 
            enableTimerPanel.AutoSize = true;
            enableTimerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            enableTimerPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            enableTimerPanel.ColumnCount = 2;
            enableTimerPanel.ColumnStyles.Add(new ColumnStyle());
            enableTimerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            enableTimerPanel.Controls.Add(enableTimerToggleButton, 1, 0);
            enableTimerPanel.Controls.Add(enableTimerLabel, 0, 0);
            enableTimerPanel.Location = new Point(622, 0);
            enableTimerPanel.Margin = new Padding(3, 0, 0, 0);
            enableTimerPanel.Name = "enableTimerPanel";
            enableTimerPanel.Padding = new Padding(3, 0, 3, 0);
            enableTimerPanel.RowCount = 1;
            enableTimerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            enableTimerPanel.Size = new Size(202, 30);
            enableTimerPanel.TabIndex = 20;
            // 
            // enableTimerToggleButton
            // 
            enableTimerToggleButton.AutoSize = true;
            enableTimerToggleButton.BorderColor = Color.Gray;
            enableTimerToggleButton.BorderWidth = 2;
            enableTimerToggleButton.Location = new Point(150, 4);
            enableTimerToggleButton.MinimumSize = new Size(45, 22);
            enableTimerToggleButton.Name = "enableTimerToggleButton";
            enableTimerToggleButton.OffBackColor = Color.Gray;
            enableTimerToggleButton.OffToggleColor = Color.Gainsboro;
            enableTimerToggleButton.OnBackColor = Color.MediumSlateBlue;
            enableTimerToggleButton.OnToggleColor = Color.WhiteSmoke;
            enableTimerToggleButton.Size = new Size(45, 22);
            enableTimerToggleButton.TabIndex = 18;
            enableTimerToggleButton.ToggleWidthPercentage = 15;
            enableTimerToggleButton.UseVisualStyleBackColor = true;
            // 
            // enableTimerLabel
            // 
            enableTimerLabel.AutoSize = true;
            enableTimerLabel.Dock = DockStyle.Fill;
            enableTimerLabel.Font = new Font("Microsoft Sans Serif", 12.75F);
            enableTimerLabel.Location = new Point(7, 1);
            enableTimerLabel.Name = "enableTimerLabel";
            enableTimerLabel.Size = new Size(136, 28);
            enableTimerLabel.TabIndex = 19;
            enableTimerLabel.Text = "ENABLE_TIMER";
            enableTimerLabel.TextAlign = ContentAlignment.MiddleCenter;
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
            enableCustomVarPanel.ResumeLayout(false);
            enableCustomVarPanel.PerformLayout();
            enableTimerPanel.ResumeLayout(false);
            enableTimerPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label PlaceholdersLabel;
        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel configButtonPanel;
        public Button fcConfigButton;
        public Button segmentNameConfigButton;
        public Button textListConfigButton;
        public InteractableTabControl tabControl;
        private TableLayoutPanel enableCustomVarPanel;
        private RJToggleButton enableCustomVarToggleButton;
        private Label enableCustomVarLabel;
        private TableLayoutPanel enableTimerPanel;
        private RJToggleButton enableTimerToggleButton;
        private Label enableTimerLabel;
    }
}
