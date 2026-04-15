namespace TiaUtilities
{
    partial class QuestionMarkForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestionMarkForm));
            mainPanel = new TableLayoutPanel();
            topLabel = new Label();
            descriptionLabel = new Label();
            bottomPanel = new FlowLayoutPanel();
            madeByLabel = new Label();
            myReposLinkLabel = new LinkLabel();
            softwareGithubLinkLabel = new LinkLabel();
            mainPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.ColumnCount = 1;
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(topLabel, 0, 0);
            mainPanel.Controls.Add(descriptionLabel, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);
            mainPanel.Location = new Point(0, 0);
            mainPanel.Name = "mainPanel";
            mainPanel.RowCount = 3;
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.RowStyles.Add(new RowStyle());
            mainPanel.Size = new Size(557, 140);
            mainPanel.TabIndex = 0;
            // 
            // topLabel
            // 
            topLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            topLabel.AutoSize = true;
            topLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            topLabel.Location = new Point(3, 0);
            topLabel.Name = "topLabel";
            topLabel.Size = new Size(551, 37);
            topLabel.TabIndex = 0;
            topLabel.Text = "TIA Utilities {v}";
            topLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // descriptionLabel
            // 
            descriptionLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            descriptionLabel.AutoSize = true;
            descriptionLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            descriptionLabel.Location = new Point(3, 37);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Padding = new Padding(0, 5, 0, 5);
            descriptionLabel.Size = new Size(551, 73);
            descriptionLabel.TabIndex = 1;
            descriptionLabel.Text = resources.GetString("descriptionLabel.Text");
            // 
            // bottomPanel
            // 
            bottomPanel.Anchor = AnchorStyles.None;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.Controls.Add(madeByLabel);
            bottomPanel.Controls.Add(myReposLinkLabel);
            bottomPanel.Controls.Add(softwareGithubLinkLabel);
            bottomPanel.Location = new Point(129, 113);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(298, 24);
            bottomPanel.TabIndex = 3;
            // 
            // madeByLabel
            // 
            madeByLabel.AutoSize = true;
            madeByLabel.Font = new Font("Microsoft Sans Serif", 14.25F);
            madeByLabel.Location = new Point(0, 0);
            madeByLabel.Margin = new Padding(0);
            madeByLabel.Name = "madeByLabel";
            madeByLabel.Size = new Size(147, 24);
            madeByLabel.TabIndex = 3;
            madeByLabel.Text = "Made with ❤️ by";
            // 
            // myReposLinkLabel
            // 
            myReposLinkLabel.AutoSize = true;
            myReposLinkLabel.Font = new Font("Microsoft Sans Serif", 14.25F);
            myReposLinkLabel.Location = new Point(147, 0);
            myReposLinkLabel.Margin = new Padding(0);
            myReposLinkLabel.Name = "myReposLinkLabel";
            myReposLinkLabel.Size = new Size(76, 24);
            myReposLinkLabel.TabIndex = 4;
            myReposLinkLabel.TabStop = true;
            myReposLinkLabel.Text = "Parozzz";
            // 
            // softwareGithubLinkLabel
            // 
            softwareGithubLinkLabel.AutoSize = true;
            softwareGithubLinkLabel.Font = new Font("Microsoft Sans Serif", 14.25F);
            softwareGithubLinkLabel.Location = new Point(233, 0);
            softwareGithubLinkLabel.Margin = new Padding(10, 0, 0, 0);
            softwareGithubLinkLabel.Name = "softwareGithubLinkLabel";
            softwareGithubLinkLabel.Size = new Size(65, 24);
            softwareGithubLinkLabel.TabIndex = 2;
            softwareGithubLinkLabel.TabStop = true;
            softwareGithubLinkLabel.Text = "Github";
            // 
            // QuestionMarkForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(800, 450);
            Controls.Add(mainPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "QuestionMarkForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "?";
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel mainPanel;
        private Label topLabel;
        private Label descriptionLabel;
        private FlowLayoutPanel bottomPanel;
        private Label madeByLabel;
        private LinkLabel softwareGithubLinkLabel;
        private LinkLabel myReposLinkLabel;
    }
}