
using Microsoft.WindowsAPICodePack.Dialogs;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Javascript;
using InfoBox;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation;
using TiaUtilities.Generation.GenForms.Alarm;
using System.Diagnostics;
using TiaXmlReader.Generation.GridHandler.Events;

namespace TiaUtilities.Generation.GenForms.Alarm
{
    public partial class AlarmGenerationForm : Form
    {
        private readonly TimedSaveHandler timedSaveHandler;
        private readonly AlarmGenerationSettings settings;
        private readonly GridSettings gridSettings;
        private AlarmGenerationProjectSave? oldProjectSave;

        private readonly GridHandler<AlarmConfiguration, DeviceData> deviceGridHandler;
        private readonly GridHandler<AlarmConfiguration, AlarmData> alarmGridHandler;
        private readonly AlarmGenerationFormConfigHandler configHandler;

        private AlarmConfiguration AlarmConfig { get => settings.Configuration; }

        private string? lastFilePath;
        private static readonly string[] items = ["TON", "TOF"];

        public AlarmGenerationForm(JavascriptErrorReportThread jsErrorHandlingThread, TimedSaveHandler autoSaveHandler, AlarmGenerationSettings settings, GridSettings gridSettings)
        {
            InitializeComponent();

            this.timedSaveHandler = autoSaveHandler;
            this.settings = settings;
            this.gridSettings = gridSettings;

            this.deviceGridHandler = new GridHandler<AlarmConfiguration, DeviceData>(jsErrorHandlingThread, gridSettings, AlarmConfig) { RowCount = 499 };
            this.alarmGridHandler = new GridHandler<AlarmConfiguration, AlarmData>(jsErrorHandlingThread, gridSettings, AlarmConfig) { RowCount = 64 };

            this.configHandler = new AlarmGenerationFormConfigHandler(this, this.AlarmConfig);


            Init();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                switch (keyData)
                {
                    case Keys.S | Keys.Control:
                        this.ProjectSave();
                        return true; //Return required otherwise will write the letter.
                    case Keys.L | Keys.Control:
                        this.ProjectLoad();
                        return true; //Return required otherwise will write the letter.
                }

                if (this.deviceGridHandler.ProcessCmdKey(ref msg, keyData) || this.alarmGridHandler.ProcessCmdKey(ref msg, keyData))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }


            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Init()
        {
            this.GridsSplitPanel.Panel1.Controls.Add(this.deviceGridHandler.DataGridView);
            this.GridsSplitPanel.Panel2.Controls.Add(this.alarmGridHandler.DataGridView);

            #region FORM
            this.FormClosing += (sender, args) =>
            {
                InformationBoxResult result = InformationBoxResult.None;
                if (this.oldProjectSave == null)
                {
                    result = InformationBox.Show("Do you want to save this project?", title: "Project not saved", buttons: InformationBoxButtons.YesNoCancel);
                }
                else
                {
                    var projectSave = this.CreateProjectSave();
                    if (Utils.AreDifferentObject(projectSave, this.oldProjectSave))
                    {
                        result = InformationBox.Show("Do you want to save this project?", title: "Project different from last save", buttons: InformationBoxButtons.YesNoCancel);
                    }
                }

                if (result == InformationBoxResult.Yes)
                {
                    this.ProjectSave();
                }
                else if (result == InformationBoxResult.Cancel)
                {
                    args.Cancel = true;
                }
            };
            #endregion

            #region TopMenu
            this.saveMenuItem.Click += (sender, args) => { this.ProjectSave(); };
            this.saveAsMenuItem.Click += (sender, args) => { this.ProjectSave(true); };
            this.loadMenuItem.Click += (sender, args) => { this.ProjectLoad(); };
            this.exportXMLMenuItem.Click += (sender, args) =>
            {
                try
                {
                    var fileDialog = new CommonOpenFileDialog
                    {
                        IsFolderPicker = true,
                        EnsurePathExists = true,
                        EnsureValidNames = true,
                    };

                    if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        var alarmDataList = new List<AlarmData>(this.alarmGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys);  //Return CLONED data, otherwise operations on the xml generation will affect the table!
                        var deviceDataList = new List<DeviceData>(this.deviceGridHandler.DataSource.GetNotEmptyClonedDataDict().Keys);  //Return CLONED data, otherwise operations on the xml generation will affect the table!

                        var ioXmlGenerator = new AlarmXmlGenerator(this.AlarmConfig, alarmDataList, deviceDataList);
                        ioXmlGenerator.GenerateBlocks();
                        ioXmlGenerator.ExportXML(fileDialog.FileName);

                        Process.Start("explorer.exe", fileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }
            };
            this.preferencesMenuItem.Click += (sender, args) => this.gridSettings.ShowConfigForm(this);
            #endregion

            #region PartitionType ComboBox
            this.partitionTypeComboBox.DisplayMember = "Text";
            this.partitionTypeComboBox.ValueMember = "Value";

            var partitionTypeItems = new List<object>();
            foreach (AlarmPartitionType partitionType in Enum.GetValues(typeof(AlarmPartitionType)))
            {
                partitionTypeItems.Add(new { Text = partitionType.GetTranslation(), Value = partitionType });
            }
            this.partitionTypeComboBox.DataSource = partitionTypeItems;
            #endregion

            #region GroupingType ComboBox
            this.groupingTypeComboBox.DisplayMember = "Text";
            this.groupingTypeComboBox.ValueMember = "Value";

            var gropingTypeItems = new List<object>();
            foreach (AlarmGroupingType groupingType in Enum.GetValues(typeof(AlarmGroupingType)))
            {
                gropingTypeItems.Add(new { Text = groupingType.GetTranslation(), Value = groupingType });
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
            #region COLUMNS
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.NAME, 125);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.ADDRESS, 160);
            this.deviceGridHandler.AddTextBoxColumn(DeviceData.DESCRIPTION, 0);

            this.alarmGridHandler.AddCheckBoxColumn(AlarmData.ENABLE, 40);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.ALARM_VARIABLE, 250);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.COIL1_ADDRESS, 95);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.COIL2_ADDRESS, 95);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_ADDRESS, 95);
            this.alarmGridHandler.AddComboBoxColumn(AlarmData.TIMER_TYPE, 55, items);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.TIMER_VALUE, 50);
            this.alarmGridHandler.AddTextBoxColumn(AlarmData.DESCRIPTION, 0);
            #endregion

            this.deviceGridHandler.Init();
            this.alarmGridHandler.Init();
            this.configHandler.Init();

            this.alarmGridHandler.Events.CellChange += args =>
            {
                if(args.CellChangeList == null)
                {
                    return;
                }

                foreach(var cellChange in args.CellChangeList)
                {
                    if(AlarmData.ALARM_VARIABLE == cellChange.ColumnIndex)
                    {//If an alarm variable is filled (Before empty and now full) i will automatically set the enable to be true. The opposite removes the enable. QOL
                        if(cellChange.IsOldValueEmptyString() && cellChange.IsNewValueFullString())
                        {
                            alarmGridHandler.DataSource[cellChange.RowIndex].Enable = true;
                        }
                        else if(cellChange.IsOldValueFullString() && cellChange.IsNewValueEmptyString())
                        {
                            alarmGridHandler.DataSource[cellChange.RowIndex].Enable = false;
                        }
                    }
                }
            };

            #region GRIDS_JSCRIPT_EVENTS
            this.deviceGridHandler.Events.ScriptLoad += args => args.Script = settings.DeviceJSScript;
            this.deviceGridHandler.Events.ScriptChanged += args => settings.DeviceJSScript = args.Script;

            this.alarmGridHandler.Events.ScriptLoad += args => args.Script = settings.AlarmJSScript;
            this.alarmGridHandler.Events.ScriptChanged += args => settings.AlarmJSScript = args.Script;
            #endregion

            #region AUTO_SAVE
            void eventHandler(object sender, EventArgs args)
            {
                if (File.Exists(this.lastFilePath))
                {
                    this.ProjectSave();
                }
            }
            this.Shown += (sender, args) => timedSaveHandler.AddTickEventHandler(eventHandler);
            this.FormClosed += (sender, args) => timedSaveHandler.RemoveTickEventHandler(eventHandler);
            #endregion

            this.Shown += (sender, args) =>
            {
                this.alarmGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
                this.deviceGridHandler.DataGridView.AutoResizeColumnHeadersHeight();
            };

            Translate();
        }

        private void Translate()
        {
            this.Text = this.GetFormLocalizatedName();

            this.fileMenuItem.Text = Localization.Get("GENERICS_FILE");
            this.saveMenuItem.Text = Localization.Get("GENERICS_SAVE");
            this.saveAsMenuItem.Text = Localization.Get("GENERICS_SAVE_AS");
            this.loadMenuItem.Text = Localization.Get("GENERICS_LOAD");

            this.importExportMenuItem.Text = Localization.Get("IO_GEN_FORM_IMPEXP");
            this.exportXMLMenuItem.Text = Localization.Get("IO_GEN_FORM_IMPEXP_EXPORT_XML");

            this.preferencesMenuItem.Text = Localization.Get("GRID_PREFERENCES");

            this.configHandler.Translate();
        }

        private AlarmGenerationProjectSave CreateProjectSave()
        {
            var projectSave = new AlarmGenerationProjectSave();

            GenerationUtils.CopyJsonFieldsAndProperties(this.AlarmConfig, projectSave.AlarmConfig);

            foreach (var entry in this.deviceGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddDeviceData(entry.Key, entry.Value);
            }

            foreach (var entry in this.alarmGridHandler.DataSource.GetNotEmptyDataDict())
            {
                projectSave.AddAlarmData(entry.Key, entry.Value);
            }

            return projectSave;
        }

        public void ProjectSave(bool saveAs = false)
        {
            var projectSave = this.CreateProjectSave();

            var saveOK = projectSave.Save(ref lastFilePath, saveAs || !File.Exists(lastFilePath));
            if (!saveOK)
            {
                this.Text = this.GetFormLocalizatedName();
                return;
            }

            this.Text = this.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
            this.oldProjectSave = projectSave;
        }

        public void ProjectLoad()
        {
            var loadedProjectSave = AlarmGenerationProjectSave.Load(ref lastFilePath);
            if (loadedProjectSave != null)
            {
                GenerationUtils.CopyJsonFieldsAndProperties(loadedProjectSave.AlarmConfig, this.AlarmConfig);

                this.alarmGridHandler.DataGridView.SuspendLayout();
                this.deviceGridHandler.DataGridView.SuspendLayout();

                this.alarmGridHandler.DataSource.InitializeData(this.alarmGridHandler.RowCount);
                this.deviceGridHandler.DataSource.InitializeData(this.deviceGridHandler.RowCount);

                foreach (var entry in loadedProjectSave.DeviceData)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.deviceGridHandler.RowCount)
                    {
                        var data = this.deviceGridHandler.DataSource[rowIndex];
                        this.deviceGridHandler.DataHandler.CopyValues(entry.Value, data);
                    }
                }

                foreach (var entry in loadedProjectSave.AlarmData)
                {
                    var rowIndex = entry.Key;
                    if (rowIndex >= 0 && rowIndex <= this.alarmGridHandler.RowCount)
                    {
                        var data = this.alarmGridHandler.DataSource[rowIndex];
                        this.alarmGridHandler.DataHandler.CopyValues(entry.Value, data);
                    }
                }

                this.alarmGridHandler.DataGridView.Refresh();
                this.deviceGridHandler.DataGridView.Refresh();

                this.alarmGridHandler.DataGridView.ResumeLayout();
                this.deviceGridHandler.DataGridView.ResumeLayout();

                this.Text = this.GetFormLocalizatedName() + ". Project File: " + lastFilePath;
                this.oldProjectSave = loadedProjectSave;
            }
        }

        private string GetFormLocalizatedName()
        {
            return Localization.Get("ALARM_GEN_FORM");
        }

        private void PlaceholdersLabel_Click(object sender, EventArgs e)
        {

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