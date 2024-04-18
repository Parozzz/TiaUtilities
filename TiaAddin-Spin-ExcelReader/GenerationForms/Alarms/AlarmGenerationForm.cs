
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.IO.ExcelImporter;
using TiaXmlReader.Localization;
using TiaXmlReader.SimaticML;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.GenerationForms.IO;
using TiaXmlReader.Utility;
using TiaXmlReader.GenerationForms.IO.Config;
using TiaXmlReader.GenerationForms.Alarms;
using TiaXmlReader.Generation.UserAlarms;

namespace TiaXmlReader.GenerationForms.IO
{
    public partial class AlarmGenerationForm : Form
    {
        public const int TOTAL_ROW_COUNT = 1999;

        public const int ADDRESS_COLUMN = 0;
        public const int IO_NAME_COLUMN = 1;
        public const int DB_COLUMN = 2;
        public const int VARIABLE_COLUMN = 3;
        public const int COMMENT_COLUMN = 4;

        private readonly GridHandler<AlarmData> alarmGridHandler;
        private readonly GridHandler<DeviceData> deviceGridHandler;
        

        private readonly AlarmGenerationSettings settings;
        private readonly IOGenerationFormConfigHandler configHandler = null;
        private AlarmConfiguration AlarmConfig { get => settings.Configuration; }

        private string lastFilePath;

        public AlarmGenerationForm()
        {
            InitializeComponent();

            settings = AlarmGenerationSettings.Load();
            settings.Save(); //This could be avoided but is to be sure that all the classes that are created new will be saved to file!

            this.alarmGridHandler = new GridHandler<AlarmData>(this.AlarmDataGridView, settings.GridSettings, () => new AlarmData(), (oldData, newData) => oldData.CopyFrom(newData), null);
            this.deviceGridHandler = new GridHandler<DeviceData>(this.DeviceDataGridView, settings.GridSettings, () => new DeviceData(), (oldData, newData) => oldData.CopyFrom(newData), null);
            //this.configHandler = new IOGenerationFormConfigHandler(this, IOConfig, this.dataGridView);

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
            //var addressColumn = this.dataTable.Rows.Add(TOTAL_ROW_COUNT);

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
                        var ioDataList = new List<IOData>(this.deviceGridHandler.DataSource.GetNotEmptyDataDict().Keys);

                        var ioXmlGenerator = new IOXmlGenerator(this.IOConfig, ioDataList);
                        ioXmlGenerator.GenerateBlocks();
                        ioXmlGenerator.ExportXML(fileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.importExcelToolStripMenuItem.Click += (object sender, EventArgs args) =>
            {
                var excelImportForm = new IOGenerationExcelImportForm(this.ExcelImportConfig, this.settings.GridSettings);

                var dialogResult = excelImportForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    var ioDataList = new List<IOData>();
                    foreach (var importData in excelImportForm.ImportDataEnumerable)
                    {
                        ioDataList.Add(new IOData()
                        {
                            Address = importData.Address,
                            IOName = importData.IOName,
                            Comment = importData.Comment
                        });
                    }

                    var ioDataCounter = 0;
                    foreach (var emptyRowIndex in this.deviceGridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count))
                    {
                        if (ioDataCounter >= ioDataList.Count)
                        {
                            break;
                        }

                        var ioData = ioDataList[ioDataCounter++];
                        this.deviceGridHandler.ChangeRow(emptyRowIndex, ioData);
                    }
                }
            };
            this.preferencesToolStripMenuItem.Click += (object sender, EventArgs args) =>
            {
                var configForm = new ConfigForm("Preferenze");
                configForm.AddColorPickerLine("Bordo cella selezionata")
                    .ApplyColor(settings.GridSettings.SingleSelectedCellBorderColor)
                    .ColorChanged(color => settings.GridSettings.SingleSelectedCellBorderColor = color);

                configForm.AddColorPickerLine("Sfondo cella trascinata")
                    .ApplyColor(settings.GridSettings.DragSelectedCellBorderColor)
                    .ColorChanged(color => settings.GridSettings.DragSelectedCellBorderColor = color);

                configForm.AddColorPickerLine("Triangolo trascinamento")
                    .ApplyColor(settings.GridSettings.SelectedCellTriangleColor)
                    .ColorChanged(color => settings.GridSettings.SelectedCellTriangleColor = color);

                configForm.AddColorPickerLine("Anteprima")
                    .ApplyColor(this.Preferences.PreviewColor)
                    .ColorChanged(color => this.Preferences.PreviewColor = color);

                configForm.StartShowingAtLocation(Cursor.Position);
                configForm.Init();
                configForm.Show(this);

                DeviceDataGridView.Refresh();
            };
            #endregion

            #region PartitionType ComboBox
            this.partitionTypeComboBox.DisplayMember = "Text";
            this.partitionTypeComboBox.ValueMember = "Value";
            
            var partitionTypeItems = new List<object>();
            foreach (AlarmPartitionType partitionType in Enum.GetValues(typeof(AlarmPartitionType)))
            {
                partitionTypeItems.Add(new { Text = partitionType.GetDescription(), Value = partitionType });
            }
            this.partitionTypeComboBox.DataSource = partitionTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetDescription(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion

            #region CELL_PAINTERS
            this.deviceGridHandler.AddCellPainter(new IOGenerationFormPreviewCellPainter(this.deviceGridHandler.DataSource, this.IOConfig, this.Preferences));
            #endregion

            #region DRAG
            this.deviceGridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.deviceGridHandler); });
            this.deviceGridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.deviceGridHandler); });
            #endregion 

            #region DATA_ASSOCIATION
            this.deviceGridHandler.SetDataAssociation(ADDRESS_COLUMN, ioData => ioData.Address);
            this.deviceGridHandler.SetDataAssociation(IO_NAME_COLUMN, ioData => ioData.IOName);
            this.deviceGridHandler.SetDataAssociation(DB_COLUMN, ioData => ioData.DBName);
            this.deviceGridHandler.SetDataAssociation(VARIABLE_COLUMN, ioData => ioData.Variable);
            this.deviceGridHandler.SetDataAssociation(COMMENT_COLUMN, ioData => ioData.Comment);
            #endregion

            #region PREFERENCES
            #endregion

            this.deviceGridHandler.Init();
            this.configHandler.Init();

            //Column initialization after gridHandler.Init()
            #region COLUMNS
            var addressColumn = (DataGridViewTextBoxColumn)this.deviceGridHandler.InitColumn(ADDRESS_COLUMN, "Indirizzo", 55);
            addressColumn.MaxInputLength = 10;

            var ioNameColumn = this.deviceGridHandler.InitColumn(IO_NAME_COLUMN, "Nome IO", 80);
            var dbNameColumn = this.deviceGridHandler.InitColumn(DB_COLUMN, "DB", 80);
            var variableColumn = this.deviceGridHandler.InitColumn(VARIABLE_COLUMN, "Variabile", 105);
            var commentColumn = this.deviceGridHandler.InitColumn(COMMENT_COLUMN, "Commento", 0);
            #endregion

            #region PROGRAM_SAVE_TICK
            var timer = new Timer { Interval = 1000 };
            timer.Start();

            var alarmConfigSnapshot = Utils.CreatePublicFieldSnapshot(this.AlarmConfig);
            var gridSettingsSnapshot = Utils.CreatePublicFieldSnapshot(this.settings.GridSettings);
            timer.Tick += (sender, e) =>
            {
                //This is done this way because is impossible that fields are changed toghether for multiple configs. So at the first that is different, i create a snapshot and save to file!
                var configEqual = Utils.ComparePublicFieldSnapshot(this.AlarmConfig, alarmConfigSnapshot);
                if(!configEqual)
                {
                    alarmConfigSnapshot = Utils.CreatePublicFieldSnapshot(this.AlarmConfig);
                    this.settings.Save();
                    return;
                }

                var gridSettingsEqual = Utils.ComparePublicFieldSnapshot(this.settings.GridSettings, gridSettingsSnapshot);
                if (!gridSettingsEqual)
                {
                    gridSettingsSnapshot = Utils.CreatePublicFieldSnapshot(this.settings.GridSettings);
                    this.settings.Save();
                    return;
                }
            };
            #endregion
        }

        public void ProjectSave(bool saveAs = false)
        {
            /*
            var projectSave = new IOGenerationProjectSave();
            foreach (var entry in deviceGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddIOData(entry.Key, entry.Value);
            }
            projectSave.Save(ref lastFilePath, saveAs);

            this.Text = this.Name + ". File: " + lastFilePath;*/
        }

        public void ProjectLoad()
        {
            /*
            var loadedProjectSave = IOGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                this.DeviceDataGridView.SuspendLayout();
                this.deviceGridHandler.DataSource.InitializeData(this.deviceGridHandler.RowCount);

                foreach (var saveData in loadedProjectSave.SaveDataList)
                {
                    var rowIndex = saveData.RowIndex;
                    if (rowIndex >= 0 && rowIndex <= TOTAL_ROW_COUNT)
                    {
                        saveData.SaveTo(this.deviceGridHandler.DataSource[saveData.RowIndex]);
                    }
                }

                this.DeviceDataGridView.Refresh();
                this.DeviceDataGridView.ResumeLayout();

                this.Text = this.Name + ". File: " + lastFilePath;
            }*/
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