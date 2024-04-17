
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

namespace TiaXmlReader.GenerationForms.IO
{
    public partial class IOGenerationForm : Form
    {
        public const int TOTAL_ROW_COUNT = 1999;
        public static readonly Color SORT_ICON_COLOR = Color.Green;
        public static readonly Color SELECTED_CELL_COLOR = Color.DarkGreen;
        public static readonly Color DRAGGED_CELL_BACK_COLOR = Color.PaleGreen;

        public const int ADDRESS_COLUMN = 0;
        public const int IO_NAME_COLUMN = 1;
        public const int DB_COLUMN = 2;
        public const int VARIABLE_COLUMN = 3;
        public const int COMMENT_COLUMN = 4;

        private readonly GridHandler<IOData> gridHandler;

        private readonly IOConfiguration config;
        private readonly IOGenerationExcelImportConfiguration excelImportConfig;
        private readonly IOGenerationFormConfigHandler configHandler;

        private string lastFilePath;

        public IOGenerationForm()
        {
            InitializeComponent();

            this.gridHandler = new GridHandler<IOData>(this.dataGridView, () => new IOData(), (oldData, newData) => oldData.CopyFrom(newData), new IOGenerationComparer());

            var preferenceSave = IOGenerationPreferenceSave.Load();
            this.config = preferenceSave.Configuration;
            this.excelImportConfig = preferenceSave.ExcelImportConfiguration;
            this.configHandler = new IOGenerationFormConfigHandler(this, config, this.dataGridView);

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
                        var ioDataList = new List<IOData>(this.gridHandler.DataSource.GetNotEmptyDataDict().Keys);

                        var ioXmlGenerator = new IOXmlGenerator(this.config, ioDataList);
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
                var excelImportForm = new IOGenerationExcelImportForm(this.excelImportConfig);

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
                    foreach (var emptyRowIndex in this.gridHandler.DataSource.GetFirstEmptyRowIndexes(ioDataList.Count))
                    {
                        if (ioDataCounter >= ioDataList.Count)
                        {
                            break;
                        }

                        var ioData = ioDataList[ioDataCounter++];
                        this.gridHandler.ChangeRow(emptyRowIndex, ioData);
                    }
                }
            };
            #endregion

            #region MemoryType ComboBox
            this.memoryTypeComboBox.DisplayMember = "Text";
            this.memoryTypeComboBox.ValueMember = "Value";

            var memoryTypeItems = new List<object>();
            foreach (IOMemoryTypeEnum memoryType in Enum.GetValues(typeof(IOMemoryTypeEnum)))
            {
                memoryTypeItems.Add(new { Text = memoryType.GetDescription(), Value = memoryType });
            }
            this.memoryTypeComboBox.DataSource = memoryTypeItems;

            this.memoryTypeComboBox.SelectionChangeCommitted += (object sender, EventArgs args) => this.UpdateConfigPanel();
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (IOGroupingTypeEnum groupingType in Enum.GetValues(typeof(IOGroupingTypeEnum)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetDescription(), Value = groupingType });
            }
            this.groupingTypeComboBox.DataSource = gropingTypeItems;
            #endregion

            #region CELL_PAINTERS
            this.gridHandler.AddCellPainter(new IOGenerationFormPreviewCellPainter(this.gridHandler.DataSource, this.config));
            #endregion

            #region DRAG
            this.gridHandler.SetDragPreviewAction(data => { IOGenerationUtils.DragPreview(data, this.gridHandler); });
            this.gridHandler.SetDragMouseUpAction(data => { IOGenerationUtils.DragMouseUp(data, this.gridHandler); });
            #endregion

            #region DATA_ASSOCIATION
            this.gridHandler.SetDataAssociation(ADDRESS_COLUMN, ioData => ioData.Address);
            this.gridHandler.SetDataAssociation(IO_NAME_COLUMN, ioData => ioData.IOName);
            this.gridHandler.SetDataAssociation(DB_COLUMN, ioData => ioData.DBName);
            this.gridHandler.SetDataAssociation(VARIABLE_COLUMN, ioData => ioData.Variable);
            this.gridHandler.SetDataAssociation(COMMENT_COLUMN, ioData => ioData.Comment);
            #endregion

            this.gridHandler.Init();
            this.configHandler.Init();

            //Column initialization after gridHandler.Init()
            #region COLUMNS
            var addressColumn = (DataGridViewTextBoxColumn)this.gridHandler.InitColumn(ADDRESS_COLUMN, "Indirizzo", 55);
            addressColumn.MaxInputLength = 10;

            var ioNameColumn = this.gridHandler.InitColumn(IO_NAME_COLUMN, "Nome IO", 80);
            var dbNameColumn = this.gridHandler.InitColumn(DB_COLUMN, "DB", 80);
            var variableColumn = this.gridHandler.InitColumn(VARIABLE_COLUMN, "Variabile", 105);
            var commentColumn = this.gridHandler.InitColumn(COMMENT_COLUMN, "Commento", 0);
            #endregion

            #region SAVE_PREFERENCES_TICK
            var timer = new Timer { Interval = 1000 };
            timer.Start();

            var configSnapshot = Utils.CreatePublicFieldSnapshot(this.config);
            var excelImportConfigSnapshot = Utils.CreatePublicFieldSnapshot(this.excelImportConfig);
            timer.Tick += (sender, e) =>
            {
                //This is done this way because is impossible that fields are changed toghether for multiple configs. So at the first that is different, i create a snapshot and save to file!
                var configEqual = Utils.ComparePublicFieldSnapshot(this.config, configSnapshot);
                if(!configEqual)
                {
                    configSnapshot = Utils.CreatePublicFieldSnapshot(this.config);
                    this.PreferencesSave();
                    return;
                }

                var excelImportConfigEqual = Utils.ComparePublicFieldSnapshot(this.excelImportConfig, excelImportConfigSnapshot);
                if (!excelImportConfigEqual)
                {
                    excelImportConfigSnapshot = Utils.CreatePublicFieldSnapshot(this.excelImportConfig);
                    this.PreferencesSave();
                    return;
                }
            };
            #endregion

            UpdateConfigPanel();
        }

        public void ProjectSave(bool saveAs = false)
        {
            var projectSave = new IOGenerationProjectSave();
            foreach (var entry in gridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddIOData(entry.Key, entry.Value);
            }
            projectSave.Save(ref lastFilePath, saveAs);

            this.Text = this.Name + ". File: " + lastFilePath;
        }

        public void PreferencesSave()
        {
            new IOGenerationPreferenceSave
            {
                Configuration = this.config,
                ExcelImportConfiguration = this.excelImportConfig
            }.Save();
        }

        public void ProjectLoad()
        {
            var loadedProjectSave = IOGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                this.dataGridView.SuspendLayout();
                this.gridHandler.DataSource.InitializeData(this.gridHandler.RowCount);

                foreach (var saveData in loadedProjectSave.SaveDataList)
                {
                    var rowIndex = saveData.RowIndex;
                    if (rowIndex >= 0 && rowIndex <= TOTAL_ROW_COUNT)
                    {
                        saveData.SaveTo(this.gridHandler.DataSource[saveData.RowIndex]);
                    }
                }

                this.dataGridView.Refresh();
                this.dataGridView.ResumeLayout();

                this.Text = this.Name + ". File: " + lastFilePath;
            }
        }
        private void UpdateConfigPanel()
        {
            configButtonPanel.SuspendLayout();

            configButtonPanel.Controls.Clear();

            configButtonPanel.Controls.Add(fcConfigButton);
            if (IOMemoryTypeEnum.DB.Equals(memoryTypeComboBox.SelectedValue))
            {
                configButtonPanel.Controls.Add(dbConfigButton);
            }
            else if (IOMemoryTypeEnum.MERKER.Equals(memoryTypeComboBox.SelectedValue))
            {
                configButtonPanel.Controls.Add(variableTableConfigButton);
            }
            configButtonPanel.Controls.Add(ioTableConfigButton);
            configButtonPanel.Controls.Add(segmentNameConfigButton);

            configButtonPanel.ResumeLayout();
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