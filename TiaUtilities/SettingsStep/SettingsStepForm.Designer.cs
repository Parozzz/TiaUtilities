using TiaUtilities.CustomControls;
using TiaUtilities.SettingsStep.CustomControls;

namespace TiaUtilities.SettingsNew
{
    partial class SettingsStepForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsStepForm));
            mainTable = new TableLayoutPanelNoScrollbarsColorizable();
            selectContainerFlowPanel = new FlowLayoutPanel();
            selectContainerLabel = new Label();
            selectContainerComboBox = new ComboBoxFilterable();
            stepFlowPanel = new FlowLayoutPanel();
            bottomPanel = new Panel();
            buttonNext = new Button();
            buttonPrevious = new Button();
            mainTable.SuspendLayout();
            selectContainerFlowPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTable.ColumnCount = 1;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.Controls.Add(selectContainerFlowPanel, 0, 0);
            mainTable.Controls.Add(stepFlowPanel, 0, 1);
            mainTable.Controls.Add(bottomPanel, 0, 3);
            mainTable.Dock = DockStyle.Fill;
            mainTable.Location = new Point(0, 0);
            mainTable.Margin = new Padding(0);
            mainTable.Name = "mainTable";
            mainTable.Padding = new Padding(5);
            mainTable.RowCount = 4;
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.Size = new Size(782, 853);
            mainTable.TabIndex = 0;
            // 
            // selectContainerFlowPanel
            // 
            selectContainerFlowPanel.AutoSize = true;
            selectContainerFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectContainerFlowPanel.Controls.Add(selectContainerLabel);
            selectContainerFlowPanel.Controls.Add(selectContainerComboBox);
            selectContainerFlowPanel.Dock = DockStyle.Fill;
            selectContainerFlowPanel.Location = new Point(5, 5);
            selectContainerFlowPanel.Margin = new Padding(0);
            selectContainerFlowPanel.Name = "selectContainerFlowPanel";
            selectContainerFlowPanel.Padding = new Padding(5);
            selectContainerFlowPanel.Size = new Size(772, 38);
            selectContainerFlowPanel.TabIndex = 0;
            // 
            // selectContainerLabel
            // 
            selectContainerLabel.AutoSize = true;
            selectContainerLabel.Dock = DockStyle.Left;
            selectContainerLabel.Location = new Point(5, 5);
            selectContainerLabel.Margin = new Padding(0);
            selectContainerLabel.Name = "selectContainerLabel";
            selectContainerLabel.Size = new Size(176, 28);
            selectContainerLabel.TabIndex = 0;
            selectContainerLabel.Text = "Seleziona configurazione";
            selectContainerLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectContainerComboBox
            // 
            selectContainerComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            selectContainerComboBox.DropDownHoveredBackColor = Color.LightSeaGreen;
            selectContainerComboBox.DropDownHoveredForeColor = Color.Black;
            selectContainerComboBox.FlatStyle = FlatStyle.System;
            selectContainerComboBox.FormattingEnabled = true;
            selectContainerComboBox.Location = new Point(181, 5);
            selectContainerComboBox.Margin = new Padding(0);
            selectContainerComboBox.Name = "selectContainerComboBox";
            selectContainerComboBox.Size = new Size(151, 28);
            selectContainerComboBox.TabIndex = 0;
            selectContainerComboBox.TabStop = false;
            // 
            // stepFlowPanel
            // 
            stepFlowPanel.AutoScroll = true;
            stepFlowPanel.AutoSize = true;
            stepFlowPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            stepFlowPanel.Dock = DockStyle.Fill;
            stepFlowPanel.Location = new Point(5, 43);
            stepFlowPanel.Margin = new Padding(0);
            stepFlowPanel.Name = "stepFlowPanel";
            stepFlowPanel.Size = new Size(772, 1);
            stepFlowPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            bottomPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.Controls.Add(buttonNext);
            bottomPanel.Controls.Add(buttonPrevious);
            bottomPanel.Location = new Point(8, 805);
            bottomPanel.MinimumSize = new Size(0, 40);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(766, 40);
            bottomPanel.TabIndex = 1;
            // 
            // buttonNext
            // 
            buttonNext.AutoSize = true;
            buttonNext.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonNext.BackgroundImage = Properties.Resources.next_8462980;
            buttonNext.BackgroundImageLayout = ImageLayout.Zoom;
            buttonNext.Dock = DockStyle.Right;
            buttonNext.FlatStyle = FlatStyle.Flat;
            buttonNext.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonNext.Location = new Point(686, 0);
            buttonNext.MinimumSize = new Size(80, 0);
            buttonNext.Name = "buttonNext";
            buttonNext.Size = new Size(80, 40);
            buttonNext.TabIndex = 1;
            buttonNext.UseVisualStyleBackColor = true;
            // 
            // buttonPrevious
            // 
            buttonPrevious.AutoSize = true;
            buttonPrevious.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonPrevious.BackgroundImage = Properties.Resources.previous_8463010;
            buttonPrevious.BackgroundImageLayout = ImageLayout.Zoom;
            buttonPrevious.Dock = DockStyle.Left;
            buttonPrevious.FlatStyle = FlatStyle.Flat;
            buttonPrevious.Location = new Point(0, 0);
            buttonPrevious.Margin = new Padding(0);
            buttonPrevious.MinimumSize = new Size(80, 0);
            buttonPrevious.Name = "buttonPrevious";
            buttonPrevious.Size = new Size(80, 40);
            buttonPrevious.TabIndex = 0;
            buttonPrevious.UseVisualStyleBackColor = true;
            // 
            // SettingsStepForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 853);
            Controls.Add(mainTable);
            Name = "SettingsStepForm";
            Text = "Settings";
            TopMost = true;
            mainTable.ResumeLayout(false);
            mainTable.PerformLayout();
            selectContainerFlowPanel.ResumeLayout(false);
            selectContainerFlowPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanelNoScrollbarsColorizable mainTable;
        private Panel bottomPanel;
        private Button buttonNext;
        private Button buttonPrevious;
        private FlowLayoutPanel selectContainerFlowPanel;
        private Label selectContainerLabel;
        private ComboBoxFilterable selectContainerComboBox;
        private FlowLayoutPanel stepFlowPanel;
    }
}