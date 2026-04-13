using TiaUtilities.CustomControls.EditableTab;

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
            mainTableLayout = new TableLayoutPanel();
            configButtonPanel = new FlowLayoutPanel();
            setupButton = new Button();
            changeTemplateButton = new Button();
            tabControl = new EditableTabControl();
            mainTableLayout.SuspendLayout();
            configButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.AutoSize = true;
            mainTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTableLayout.ColumnCount = 1;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
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
            configButtonPanel.Controls.Add(setupButton);
            configButtonPanel.Controls.Add(changeTemplateButton);
            configButtonPanel.Dock = DockStyle.Left;
            configButtonPanel.Location = new Point(0, 3);
            configButtonPanel.Margin = new Padding(0, 3, 0, 0);
            configButtonPanel.Name = "configButtonPanel";
            configButtonPanel.Size = new Size(270, 30);
            configButtonPanel.TabIndex = 15;
            // 
            // setupButton
            // 
            setupButton.AutoSize = true;
            setupButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            setupButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            setupButton.Location = new Point(3, 0);
            setupButton.Margin = new Padding(3, 0, 0, 0);
            setupButton.Name = "setupButton";
            setupButton.Padding = new Padding(8, 0, 8, 0);
            setupButton.Size = new Size(83, 30);
            setupButton.TabIndex = 23;
            setupButton.Text = "Setup";
            setupButton.UseVisualStyleBackColor = true;
            // 
            // changeTemplateButton
            // 
            changeTemplateButton.AutoSize = true;
            changeTemplateButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            changeTemplateButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            changeTemplateButton.Location = new Point(89, 0);
            changeTemplateButton.Margin = new Padding(3, 0, 0, 0);
            changeTemplateButton.Name = "changeTemplateButton";
            changeTemplateButton.Padding = new Padding(8, 0, 8, 0);
            changeTemplateButton.Size = new Size(181, 30);
            changeTemplateButton.TabIndex = 22;
            changeTemplateButton.Text = "Change Template";
            changeTemplateButton.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Location = new Point(3, 36);
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(12, 5);
            tabControl.RequireConfirmationBeforeClosing = true;
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1394, 690);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TableLayoutPanel mainTableLayout;
        private FlowLayoutPanel configButtonPanel;
        public EditableTabControl tabControl;
        public Button setupButton;
        public Button changeTemplateButton;
    }
}
