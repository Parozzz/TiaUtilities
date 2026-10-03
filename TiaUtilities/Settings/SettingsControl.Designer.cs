using System.Drawing;
using TiaUtilities.CustomControls;
using TiaUtilities.CustomControls.tableColorizable;


namespace TiaUtilities.SettingsStep
{
    partial class SettingsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsControl));
            mainTable = new TableLayoutPanelColorizable();
            selectConfigurationPanel = new FlowLayoutPanel();
            toggleModeButton = new Button();
            selectConfigurationLabel = new Label();
            selectSequenceComboBox = new ComboBoxFilterable();
            searchLabel = new Label();
            searchTextBox = new TextBox();
            sequenceButtonsPanel = new FlowLayoutPanel();
            bottomPanel = new Panel();
            controlsPanel = new TableLayoutPanelColorizable();
            searchPanelControl = new TableLayoutPanelColorizable();
            mainTable.SuspendLayout();
            selectConfigurationPanel.SuspendLayout();
            bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainTable
            // 
            mainTable.AutoSize = true;
            mainTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainTable.ColumnCount = 1;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainTable.Controls.Add(selectConfigurationPanel, 0, 0);
            mainTable.Controls.Add(sequenceButtonsPanel, 0, 1);
            mainTable.Controls.Add(bottomPanel, 0, 2);
            mainTable.Dock = DockStyle.Fill;
            mainTable.Location = new Point(0, 0);
            mainTable.Margin = new Padding(0);
            mainTable.Name = "mainTable";
            mainTable.Padding = new Padding(4, 3, 4, 3);
            mainTable.RowCount = 3;
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle());
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F));
            mainTable.Size = new Size(788, 576);
            mainTable.TabIndex = 0;
            // 
            // selectConfigurationPanel
            // 
            selectConfigurationPanel.AutoSize = true;
            selectConfigurationPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            selectConfigurationPanel.Controls.Add(toggleModeButton);
            selectConfigurationPanel.Controls.Add(selectConfigurationLabel);
            selectConfigurationPanel.Controls.Add(selectSequenceComboBox);
            selectConfigurationPanel.Controls.Add(searchLabel);
            selectConfigurationPanel.Controls.Add(searchTextBox);
            selectConfigurationPanel.Dock = DockStyle.Fill;
            selectConfigurationPanel.Location = new Point(4, 3);
            selectConfigurationPanel.Margin = new Padding(0);
            selectConfigurationPanel.Name = "selectConfigurationPanel";
            selectConfigurationPanel.Padding = new Padding(4, 3, 4, 3);
            selectConfigurationPanel.Size = new Size(780, 31);
            selectConfigurationPanel.TabIndex = 0;
            // 
            // toggleModeButton
            // 
            toggleModeButton.AutoSize = true;
            toggleModeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            toggleModeButton.FlatAppearance.BorderSize = 0;
            toggleModeButton.FlatStyle = FlatStyle.Flat;
            toggleModeButton.Location = new Point(4, 3);
            toggleModeButton.Margin = new Padding(0, 0, 8, 0);
            toggleModeButton.Name = "toggleModeButton";
            toggleModeButton.Size = new Size(39, 25);
            toggleModeButton.TabIndex = 3;
            toggleModeButton.Text = "IMG";
            toggleModeButton.UseVisualStyleBackColor = true;
            // 
            // selectConfigurationLabel
            // 
            selectConfigurationLabel.Anchor = AnchorStyles.None;
            selectConfigurationLabel.AutoSize = true;
            selectConfigurationLabel.Location = new Point(51, 8);
            selectConfigurationLabel.Margin = new Padding(0, 0, 8, 0);
            selectConfigurationLabel.Name = "selectConfigurationLabel";
            selectConfigurationLabel.Size = new Size(138, 15);
            selectConfigurationLabel.TabIndex = 0;
            selectConfigurationLabel.Text = "Seleziona configurazione";
            selectConfigurationLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // selectConfigurationComboBox
            // 
            selectSequenceComboBox.Anchor = AnchorStyles.None;
            selectSequenceComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            selectSequenceComboBox.DropDownHoveredBackColor = Color.LightSeaGreen;
            selectSequenceComboBox.DropDownHoveredForeColor = Color.Black;
            selectSequenceComboBox.FlatStyle = FlatStyle.System;
            selectSequenceComboBox.FormattingEnabled = true;
            selectSequenceComboBox.Location = new Point(197, 3);
            selectSequenceComboBox.Margin = new Padding(0);
            selectSequenceComboBox.Name = "selectSequenceComboBox";
            selectSequenceComboBox.Size = new Size(117, 24);
            selectSequenceComboBox.TabIndex = 0;
            selectSequenceComboBox.TabStop = false;
            // 
            // searchLabel
            // 
            searchLabel.Anchor = AnchorStyles.None;
            searchLabel.AutoSize = true;
            searchLabel.Location = new Point(314, 8);
            searchLabel.Margin = new Padding(0, 0, 8, 0);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(69, 15);
            searchLabel.TabIndex = 1;
            searchLabel.Text = "Cerca valori";
            // 
            // searchTextBox
            // 
            searchTextBox.Anchor = AnchorStyles.None;
            searchTextBox.Location = new Point(391, 4);
            searchTextBox.Margin = new Padding(0);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new Size(192, 23);
            searchTextBox.TabIndex = 2;
            // 
            // sequenceButtonsPanel
            // 
            sequenceButtonsPanel.AutoSize = true;
            sequenceButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            sequenceButtonsPanel.Dock = DockStyle.Fill;
            sequenceButtonsPanel.Location = new Point(7, 36);
            sequenceButtonsPanel.Margin = new Padding(3, 2, 3, 2);
            sequenceButtonsPanel.Name = "sequenceButtonsPanel";
            sequenceButtonsPanel.Size = new Size(774, 1);
            sequenceButtonsPanel.TabIndex = 4;
            // 
            // bottomPanel
            // 
            bottomPanel.AutoScroll = true;
            bottomPanel.AutoSize = true;
            bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomPanel.BackColor = Color.Transparent;
            bottomPanel.Controls.Add(controlsPanel);
            bottomPanel.Controls.Add(searchPanelControl);
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new Point(8, 42);
            bottomPanel.Margin = new Padding(4, 4, 4, 4);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new Size(772, 527);
            bottomPanel.TabIndex = 3;
            // 
            // controlsPanel
            // 
            controlsPanel.AutoSize = true;
            controlsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            controlsPanel.BackColor = Color.Transparent;
            controlsPanel.ColumnCount = 1;
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            controlsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            controlsPanel.Dock = DockStyle.Top;
            controlsPanel.Location = new Point(0, 0);
            controlsPanel.Margin = new Padding(9, 8, 9, 8);
            controlsPanel.Name = "controlsPanel";
            controlsPanel.RowCount = 1;
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            controlsPanel.Size = new Size(772, 0);
            controlsPanel.TabIndex = 0;
            // 
            // searchPanelControl
            // 
            searchPanelControl.AutoSize = true;
            searchPanelControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            searchPanelControl.BackColor = Color.Transparent;
            searchPanelControl.ColumnCount = 1;
            searchPanelControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            searchPanelControl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            searchPanelControl.Dock = DockStyle.Top;
            searchPanelControl.Location = new Point(0, 0);
            searchPanelControl.Margin = new Padding(9, 8, 9, 8);
            searchPanelControl.Name = "searchPanelControl";
            searchPanelControl.RowCount = 1;
            searchPanelControl.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            searchPanelControl.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            searchPanelControl.Size = new Size(772, 0);
            searchPanelControl.TabIndex = 1;
            // 
            // SettingsControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(mainTable);
            Margin = new Padding(3, 2, 3, 2);
            Name = "SettingsControl";
            Size = new Size(788, 576);
            mainTable.ResumeLayout(false);
            mainTable.PerformLayout();
            selectConfigurationPanel.ResumeLayout(false);
            selectConfigurationPanel.PerformLayout();
            bottomPanel.ResumeLayout(false);
            bottomPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanelColorizable mainTable;
        private FlowLayoutPanel selectConfigurationPanel;
        private Label selectConfigurationLabel;
        private ComboBoxFilterable selectSequenceComboBox;
        private Panel bottomPanel;
        private TableLayoutPanelColorizable controlsPanel;
        private Label searchLabel;
        private TextBox searchTextBox;
        private Button toggleModeButton;
        private TableLayoutPanelColorizable searchPanelControl;
        private FlowLayoutPanel sequenceButtonsPanel;
    }
}
