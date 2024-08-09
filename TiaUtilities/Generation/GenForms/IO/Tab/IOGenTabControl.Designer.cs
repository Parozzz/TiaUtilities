using TiaUtilities.Generation.GenForms.Alarm;
using TiaUtilities.Generation.GenForms.Alarm.Controls;
namespace TiaUtilities.Generation.GenForms.IO.Tab
{
    partial class IOGenTabControl
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
            generationTableLayout = new TableLayoutPanel();
            configFlowLayout = new FlowLayoutPanel();
            fcConfigButton = new Button();
            segmentNameConfigButton = new Button();
            generationTableLayout.SuspendLayout();
            configFlowLayout.SuspendLayout();
            SuspendLayout();
            // 
            // generationTableLayout
            // 
            generationTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            generationTableLayout.ColumnCount = 1;
            generationTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            generationTableLayout.Controls.Add(configFlowLayout, 0, 0);
            generationTableLayout.Dock = DockStyle.Fill;
            generationTableLayout.Location = new Point(0, 0);
            generationTableLayout.Name = "generationTableLayout";
            generationTableLayout.RowCount = 2;
            generationTableLayout.RowStyles.Add(new RowStyle());
            generationTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            generationTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            generationTableLayout.Size = new Size(1480, 600);
            generationTableLayout.TabIndex = 0;
            // 
            // configFlowLayout
            // 
            configFlowLayout.AutoSize = true;
            configFlowLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            configFlowLayout.Controls.Add(fcConfigButton);
            configFlowLayout.Controls.Add(segmentNameConfigButton);
            configFlowLayout.Dock = DockStyle.Fill;
            configFlowLayout.Location = new Point(3, 3);
            configFlowLayout.Name = "configFlowLayout";
            configFlowLayout.Size = new Size(1474, 42);
            configFlowLayout.TabIndex = 0;
            // 
            // fcConfigButton
            // 
            fcConfigButton.AutoSize = true;
            fcConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            fcConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            fcConfigButton.Location = new Point(3, 3);
            fcConfigButton.Name = "fcConfigButton";
            fcConfigButton.Padding = new Padding(3);
            fcConfigButton.Size = new Size(49, 36);
            fcConfigButton.TabIndex = 0;
            fcConfigButton.Text = "FC";
            fcConfigButton.UseVisualStyleBackColor = true;
            // 
            // segmentNameConfigButton
            // 
            segmentNameConfigButton.Anchor = AnchorStyles.None;
            segmentNameConfigButton.AutoSize = true;
            segmentNameConfigButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            segmentNameConfigButton.Font = new Font("Microsoft Sans Serif", 12.75F, FontStyle.Bold);
            segmentNameConfigButton.Location = new Point(58, 6);
            segmentNameConfigButton.Margin = new Padding(3, 0, 0, 0);
            segmentNameConfigButton.Name = "segmentNameConfigButton";
            segmentNameConfigButton.Padding = new Padding(8, 0, 8, 0);
            segmentNameConfigButton.Size = new Size(162, 30);
            segmentNameConfigButton.TabIndex = 20;
            segmentNameConfigButton.Text = "Nomi Segmenti";
            segmentNameConfigButton.UseVisualStyleBackColor = true;
            // 
            // IOGenTabControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(generationTableLayout);
            Name = "IOGenTabControl";
            Size = new Size(1480, 600);
            generationTableLayout.ResumeLayout(false);
            generationTableLayout.PerformLayout();
            configFlowLayout.ResumeLayout(false);
            configFlowLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel generationTableLayout;
        public FlowLayoutPanel configFlowLayout;
        public Button fcConfigButton;
        public Button segmentNameConfigButton;
    }
}
