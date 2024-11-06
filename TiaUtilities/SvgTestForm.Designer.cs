namespace TiaUtilities
{
    partial class SvgTestForm
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
            pathTextBox = new TextBox();
            drawButton = new Button();
            svgPictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)svgPictureBox).BeginInit();
            SuspendLayout();
            // 
            // pathTextBox
            // 
            pathTextBox.Location = new Point(37, 12);
            pathTextBox.Name = "pathTextBox";
            pathTextBox.Size = new Size(673, 23);
            pathTextBox.TabIndex = 0;
            pathTextBox.Click += PathTextBox_Click;
            // 
            // drawButton
            // 
            drawButton.Location = new Point(262, 41);
            drawButton.Name = "drawButton";
            drawButton.Size = new Size(199, 23);
            drawButton.TabIndex = 1;
            drawButton.Text = "Draw";
            drawButton.UseVisualStyleBackColor = true;
            drawButton.Click += DrawButton_Click;
            // 
            // svgPictureBox
            // 
            svgPictureBox.Location = new Point(37, 90);
            svgPictureBox.Name = "svgPictureBox";
            svgPictureBox.Size = new Size(718, 333);
            svgPictureBox.TabIndex = 2;
            svgPictureBox.TabStop = false;
            svgPictureBox.MouseClick += svgPictureBox_MouseClick;
            // 
            // SvgTestForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(svgPictureBox);
            Controls.Add(drawButton);
            Controls.Add(pathTextBox);
            Name = "SvgTestForm";
            Text = "SvgTestForm";
            ((System.ComponentModel.ISupportInitialize)svgPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox pathTextBox;
        private Button drawButton;
        private PictureBox svgPictureBox;
    }
}