
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TiaXmlReader.Localization;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.Configuration;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.AutoSave;

namespace TiaXmlReader.Generation.Alarms.GenerationForm
{
    public partial class AlarmGenerationForm : Form
    {
        private readonly TimedSaveHandler timedSaveHandler;
        private readonly AlarmGenerationSettings settings;
        private readonly GridSettings gridSettings;
        private readonly GridHandler<AlarmConfiguration, DeviceData> deviceGridHandler;
        private readonly GridHandler<AlarmConfiguration, AlarmData> alarmGridHandler;
        private readonly AlarmGenerationFormConfigHandler configHandler;


        private AlarmConfiguration AlarmConfig { get => settings.Configuration; }

        private string lastFilePath;

        public AlarmGenerationForm(TimedSaveHandler autoSaveHandler, AlarmGenerationSettings settings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.timedSaveHandler = autoSaveHandler;
            this.settings = settings;
            this.gridSettings = gridSettings;

            this.deviceGridHandler = new GridHandler<AlarmConfiguration, DeviceData>(this.DeviceDataGridView, gridSettings, AlarmConfig, DeviceData.COLUMN_LIST, null)
            {
                RowCount = 499
            };

            this.alarmGridHandler = new GridHandler<AlarmConfiguration, AlarmData>(this.AlarmDataGridView, gridSettings, AlarmConfig, AlarmData.COLUMN_LIST, null)
            {
                RowCount = 29
            };

            this.configHandler = new AlarmGenerationFormConfigHandler(this, this.AlarmConfig, this.deviceGridHandler, this.alarmGridHandler);


            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.S | Keys.Control:
                    this.ProjectSave();
                    return true;
                case Keys.L | Keys.Control:
                    this.ProjectLoad();
                    return true;
            }

            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            #region TopMenu
            this.saveToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectSave(); };
            this.saveAsToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectSave(true); };
            this.loadToolStripMenuItem.Click += (object sender, EventArgs args) => { this.ProjectLoad(); };
            this.exportXMLToolStripMenuItem.Click += (object sender, EventArgs args) =>
            {
                try
                {
                    var fileDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true,
                        EnsurePathExists = true,
                        EnsureValidNames = true,
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };

                    if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var alarmDataList = new List<AlarmData>(this.alarmGridHandler.DataSource.GetNotEmptyDataDict().Keys);
                        var deviceDataList = new List<DeviceData>(this.deviceGridHandler.DataSource.GetNotEmptyDataDict().Keys);

                        var ioXmlGenerator = new AlarmXmlGenerator(this.AlarmConfig, alarmDataList, deviceDataList);
                        ioXmlGenerator.GenerateBlocks();
                        ioXmlGenerator.ExportXML(fileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.preferencesToolStripMenuItem.Click += (object sender, EventArgs args) => this.gridSettings.ShowConfigForm(this);
            #endregion

            #region PartitionType ComboBox
            this.partitionTypeComboBox.DisplayMember = "Text";
            this.partitionTypeComboBox.ValueMember = "Value";

            var partitionTypeItems = new List<object>();
            foreach (AlarmPartitionType partitionType in Enum.GetValues(typeof(AlarmPartitionType)))
            {
                partitionTypeItems.Add(new { Text = partitionType.GetEnumDescription(), Value = partitionType });
            }
            this.partitionTypeComboBox.DataSource = partitionTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetEnumDescription(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion

            #region DRAG
            this.deviceGridHandler.SetDragPreviewAction(data => { GridUtils.DragPreview(data, this.deviceGridHandler); });
            this.deviceGridHandler.SetDragMouseUpAction(data => { GridUtils.DragMouseUp(data, this.deviceGridHandler); });

            this.alarmGridHandler.SetDragPreviewAction(data => { GridUtils.DragPreview(data, this.alarmGridHandler); });
            this.alarmGridHandler.SetDragMouseUpAction(data => { GridUtils.DragMouseUp(data, this.alarmGridHandler); });
            #endregion 

            //Column initialization before gridHandler.Init()
            #region COLUMNS;
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 80);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.ADDRESS, 160);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            this.alarmGridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 80);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.COIL_ADDRESS, 80);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.SET_COIL_ADDRESS, 80);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 80);
            this.alarmGridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 60, new string[] { "TON", "TOF" });
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 60);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            this.deviceGridHandler?.Init();
            this.alarmGridHandler?.Init();
            this.configHandler?.Init();

            #region AUTO_SAVE
            void eventHandler(object sender, EventArgs args)
            {
                if (!string.IsNullOrEmpty(this.lastFilePath))
                {
                    this.ProjectSave();
                }
            }
            this.Shown += (sender, args) => timedSaveHandler.AddTickEventHandler(eventHandler);
            this.FormClosed += (sender, args) => timedSaveHandler.RemoveTickEventHandler(eventHandler);
            #endregion
        }

        public void ProjectSave(bool saveAs = false)
        {
            var projectSave = new AlarmGenerationProjectSave();
            foreach (var entry in this.deviceGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddDeviceData(entry.Key, entry.Value);
            }

            foreach (var entry in this.alarmGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddAlarmData(entry.Key, entry.Value);
            }
            projectSave.Save(ref lastFilePath, saveAs);

            this.Text = this.Name + ". File: " + lastFilePath;
        }

        public void ProjectLoad()
        {
            var loadedProjectSave = AlarmGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                this.AlarmDataGridView.SuspendLayout();
                this.DeviceDataGridView.SuspendLayout();

                this.alarmGridHandler.DataSource.InitializeData(this.alarmGridHandler.RowCount);
                this.deviceGridHandler.DataSource.InitializeData(this.deviceGridHandler.RowCount);

                foreach (var entry in loadedProjectSave.SaveData.DeviceDataDict)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.deviceGridHandler.RowCount)
                    {
                        var data = this.deviceGridHandler.DataSource[rowIndex];
                        this.deviceGridHandler.DataHandler.MoveValues(entry.Value, data);
                    }
                }

                foreach (var entry in loadedProjectSave.SaveData.AlarmDataDict)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.alarmGridHandler.RowCount)
                    {
                        var data = this.alarmGridHandler.DataSource[rowIndex];
                        this.alarmGridHandler.DataHandler.MoveValues(entry.Value, data);
                    }
                }

                this.AlarmDataGridView.Refresh();
                this.DeviceDataGridView.Refresh();
                this.AlarmDataGridView.ResumeLayout();
                this.DeviceDataGridView.ResumeLayout();

                this.Text = this.Name + ". File: " + lastFilePath;
            }
        }
    }
}

/*
this.dataGridView.SortCompare += (sender, args) =>
{
    if (args.Column.Index == 0)
    {
        //This is required to avoid the values to go bottom and top when sorting. I want the empty lines always at the bottom!.
        var sortOrderMultiplier = dataGridView.SortOrder == SortOrder.Ascending ? -1 : 1;

        var cell1Address = SimaticTagAddress.FromAddress(args.CellValue1?.ToString());
        var cell2Address = SimaticTagAddress.FromAddress(args.CellValue2?.ToString());
        if (cell1Address == null)
        {
            args.SortResult = -1 * sortOrderMultiplier;
        }
        else if (cell2Address == null)
        {
            args.SortResult = 1 * sortOrderMultiplier;
        }
        else
        {
            args.SortResult = cell1Address.CompareTo(cell2Address);
        }

        args.Handled = true;
    }
};

dataGridView.MouseDoubleClick += (sender, args) =>
{
    var hitTest = dataGridView.HitTest(args.X, args.Y);
    if (hitTest.Type == DataGridViewHitTestType.Cell)
    {
        var cell = dataGridView.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex];
        dataGridView.CurrentCell = cell;
        dataGridView.BeginEdit(true);
    }
};

dataGridView.CellPainting += (sender, args) =>
{
    if (args.RowIndex >= 0 && args.ColumnIndex >= 0)
    {
        if (dataGridView.Rows[args.RowIndex].Cells[args.ColumnIndex].Selected == true)
        {
            args.Paint(args.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
            using (Pen p = new Pen(Color.Red, 3))
            {
                Rectangle rect = args.CellBounds;
                rect.Width -= 2;
                rect.Height -= 2;
                args.Graphics.DrawRectangle(p, rect);
            }
            args.Handled = true;
        }
    }

    e.PaintBackground(e.CellBounds, true);  
    e.PaintContent(e.CellBounds);  
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))  
    {  
        Point[] pt = new Point[] { new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 10), new Point(e.CellBounds.Right - 1, e.CellBounds.Bottom - 1), new Point(e.CellBounds.Right - 10, e.CellBounds.Bottom - 1) };  
        e.Graphics.FillPolygon(brush, pt);  
    }  
    e.Handled = true;  
};
*/