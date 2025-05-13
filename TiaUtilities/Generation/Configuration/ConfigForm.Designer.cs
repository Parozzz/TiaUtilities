namespace TiaUtilities.Generation.Configuration
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mainPanel = new TableLayoutPanel();
            topPanel = new TableLayoutPanel();
            savePresetButton = new Button();
            titleLabel = new Label();
            mainPanel.SuspendLayout();
            topPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle());
            mainPanel.Controls.Add(topPanel, 0, 0);
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.ForeColor = SystemColors.ControlText;
            mainPanel.Location = new Point(0, 0);
            mainPanel.Margin = new Padding(2);
            mainPanel.Name = "mainPanel";
            mainPanel.Padding = new Padding(3);
            mainPanel.RowCount = 1;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.Size = new Size(242, 167);
            mainPanel.TabIndex = 0;
            // 
            // topPanel
            // 
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            topPanel.ColumnCount = 2;
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            topPanel.ColumnStyles.Add(new ColumnStyle());
            topPanel.Controls.Add(savePresetButton, 0, 0);
            topPanel.Controls.Add(titleLabel, 0, 0);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new Point(9, 9);
            topPanel.Name = "topPanel";
            topPanel.RowCount = 1;
            topPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            topPanel.Size = new Size(224, 24);
            topPanel.TabIndex = 1;
            // 
            // savePresetButton
            // 
            savePresetButton.AutoSize = true;
            savePresetButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            savePresetButton.BackgroundImage = TiaUtilities.Properties.Resources.Save;
            savePresetButton.BackgroundImageLayout = ImageLayout.Zoom;
            savePresetButton.Dock = DockStyle.Right;
            savePresetButton.FlatAppearance.BorderSize = 0;
            savePresetButton.FlatStyle = FlatStyle.Flat;
            savePresetButton.Location = new Point(199, 0);
            savePresetButton.Margin = new Padding(0);
            savePresetButton.MinimumSize = new Size(25, 0);
            savePresetButton.Name = "savePresetButton";
            savePresetButton.Size = new Size(25, 24);
            savePresetButton.TabIndex = 1;
            savePresetButton.UseVisualStyleBackColor = true;
            savePresetButton.Click += SavePresetButton_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            titleLabel.ImageAlign = ContentAlignment.MiddleRight;
            titleLabel.Location = new Point(3, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(193, 24);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "TITLE!";
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(242, 167);
            ControlBox = false;
            Controls.Add(mainPanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "ConfigForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Label titleLabel;
        private TableLayoutPanel topPanel;
        private Button savePresetButton;
    }
}
