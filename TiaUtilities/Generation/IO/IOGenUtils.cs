using SimaticML;
using SimaticML.Enums;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.GridHandler.Events;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.SettingsNew.Bindings;

namespace TiaUtilities.Generation.IO
{
    public static class IOGenUtils
    {
        public static void DragPreview<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = gridHandler.DataGridView;

            var startingCellValue = dataGridView.Rows[eventArgs.StartingRow]?.Cells[eventArgs.DraggedColumn].Value;
            if (eventArgs.DraggedColumn == IOData.ADDRESS)
            {
                var tagAddress = SimaticTagAddress.FromAddress(startingCellValue?.ToString());
                if (tagAddress != null)
                {
                    eventArgs.TooltipString = (eventArgs.DraggingDown
                                ? tagAddress.NextBit(SimaticDataType.BYTE, eventArgs.SelectedRowCount - 1)
                                : tagAddress.PreviousBit(SimaticDataType.BYTE, eventArgs.SelectedRowCount - 1)).GetAddress();
                }
            }
            else
            {
                GridUtils.DragPreview(eventArgs, gridHandler);
            }
        }

        public static void DragDone<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : IGridData
        {
            var dataGridView = gridHandler.DataGridView;
            if (eventArgs.DraggedColumn == IOData.ADDRESS)
            {
                var startString = dataGridView.Rows[eventArgs.StartingRow]?.Cells[eventArgs.DraggedColumn].Value?.ToString();
                if (string.IsNullOrEmpty(startString))
                {
                    return;
                }

                var tagAddress = SimaticTagAddress.FromAddress(startString);
                if (tagAddress == null)  //If is not a valid address, i won't care about doing any stuff.
                {
                    return;
                }

                var cellChangeList = new List<GridCellChange>();

                var rowIndexEnumeration = Enumerable.Range(eventArgs.TopSelectedRow, (int)eventArgs.SelectedRowCount);
                if (!eventArgs.DraggingDown)
                {
                    rowIndexEnumeration = rowIndexEnumeration.Reverse();
                }

                foreach (var rowIndex in rowIndexEnumeration)
                {
                    var cellChange = new GridCellChange(0, rowIndex) { NewValue = tagAddress.GetAddress() };
                    cellChangeList.Add(cellChange);

                    var _ = eventArgs.DraggingDown ? tagAddress.NextBit(SimaticDataType.BYTE) : tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                }

                gridHandler.ChangeCells(cellChangeList);
            }
            else
            {
                GridUtils.DragDone(eventArgs, gridHandler);
            }
        }

        public static void AddMainConfigBindings(SettingsBindings settingsBindings, IOMainConfiguration mainConfig)
        {
            /*
              
                          {
                var comboBox = this.groupingTypeComboBox;
                comboBox.SelectedValue = mainConfig.GroupingType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.GroupingType = (IOGroupingTypeEnum)(comboBox.SelectedValue ?? default(IOGroupingTypeEnum));
            }

            {
                var comboBox = this.memoryTypeComboBox;
                comboBox.SelectedValue = mainConfig.MemoryType;
                comboBox.SelectionChangeCommitted += (sender, args) => mainConfig.MemoryType = (IOMemoryTypeEnum)(comboBox.SelectedValue ?? default(IOMemoryTypeEnum));
            }

             */
            settingsBindings
                .MacroSection("IO Gen", true, mainConfig, MainForm.Settings.PresetIOMainConfiguration)

                .Section(Locale.GENERICS_CONFIGURATION)
                .AddEnum(nameof(IOMainConfiguration.GroupingType), Locale.IO_SETTINGS_GROUPING_TYPE, Locale.IO_SETTINGS_GROUPING_TYPE_DESC)
                .AddEnum(nameof(IOMainConfiguration.MemoryType), Locale.IO_SETTINGS_MEMORY_TYPE, Locale.IO_SETTINGS_MEMORY_TYPE_DESC)

                .Section(Locale.IO_GEN_CONFIG_IO_TABLE)
                .AddString(nameof(IOMainConfiguration.IOTableName), Locale.GENERICS_NAME, Locale.IO_SETTINGS_IO_TABLE_NAME_DESC)
                .AddString(nameof(IOMainConfiguration.IOTableSplitEvery), Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY, Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY_DESC)
                .AddString(nameof(IOMainConfiguration.DefaultIoName), Locale.IO_SETTINGS_IO_TABLE_DEFAULT_NAME, Locale.IO_SETTINGS_IO_TABLE_DEFAULT_NAME_DESC)

                .Section(Locale.IO_GEN_CONFIG_ALIAS_DB)
                .AddString(nameof(IOMainConfiguration.DBName), Locale.GENERICS_NAME, Locale.IO_SETTINGS_ALIAS_DB_NAME_DESC.Replace("<placeholder>", GenPlaceholders.IO.CONFIG_DB_NAME))
                .AddUInt(nameof(IOMainConfiguration.DBNumber), Locale.GENERICS_NUMBER, Locale.IO_SETTINGS_ALIAS_DB_NUMBER_DESC.Replace("<placeholder>", GenPlaceholders.IO.CONFIG_DB_NAME))
                .AddString(nameof(IOMainConfiguration.DefaultDBInputVariable), Locale.IO_SETTINGS_ALIAS_DB_INPUT_DEFAULT, "")
                .AddString(nameof(IOMainConfiguration.DefaultDBOutputVariable), Locale.IO_SETTINGS_ALIAS_DB_OUTPUT_DEFAULT, "")


                .Section(Locale.IO_GEN_CONFIG_ALIAS_TABLE)
                .AddString(nameof(IOMainConfiguration.VariableTableName), Locale.GENERICS_NAME, Locale.IO_SETTINGS_ALIAS_TABLE_NAME_DESC)
                .AddUInt(nameof(IOMainConfiguration.VariableTableSplitEvery), Locale.IO_SETTINGS_ALIAS_TABLE_SPLIT_EVERY, Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY_DESC)
                .AddUInt(nameof(IOMainConfiguration.VariableTableInputStartAddress), Locale.IO_SETTINGS_ALIAS_TABLE_INPUT_START_NUMBER, "")
                .AddUInt(nameof(IOMainConfiguration.DefaultMerkerInputVariable), Locale.IO_SETTINGS_ALIAS_TABLE_INPUT_DEFAULT_NAME, "")
                .AddUInt(nameof(IOMainConfiguration.VariableTableOutputStartAddress), Locale.IO_SETTINGS_ALIAS_TABLE_OUTPUT_START_NUMBER, "")
                .AddUInt(nameof(IOMainConfiguration.DefaultMerkerOutputVariable), Locale.IO_SETTINGS_ALIAS_TABLE_OUTPUT_DEFAULT_NAME, "");
        }

        public static void AddTabConfigSettings(SettingsBindings settingsBindings, 
            Func<string> nameFunc, Func<bool> isVisibileFunc, 
            Func<IOTabConfiguration?> tabConfigFunc, Func<Dictionary<string, ObservableConfiguration>> tabDictFunc)
        {
            settingsBindings
                .MacroSection(nameFunc, isVisibileFunc, tabConfigFunc, MainForm.Settings.PresetIOTabConfiguration, tabDictFunc)

                .Section(Locale.IO_GEN_CONFIG_FC)
                .AddString(nameof(IOTabConfiguration.FCBlockName), Locale.GENERICS_NAME, "")
                .AddString(nameof(IOTabConfiguration.FCBlockNumber), Locale.GENERICS_NUMBER, "")

                .Section(Locale.IO_GEN_CONFIG_SEGMENT)
                .AddString(nameof(IOTabConfiguration.SegmentNameBitGrouping), Locale.IO_SETTINGS_SEGMENT_BIT_GROUPING, "")
                .AddString(nameof(IOTabConfiguration.SegmentNameByteGrouping), Locale.IO_SETTINGS_SEGMENT_BYTE_GROUPING, "");
        }

        public static void AddExcelImporterSettingsBindings(SettingsBindings settingsBindings, IOExcelImportConfiguration excelImportConfig)
        {
            settingsBindings
                .MacroSection(Locale.IO_SETTINGS_EXCELIMPORT, true, excelImportConfig, MainForm.Settings.PresetIOExcelImportConfiguration)

                .Section(Locale.GENERICS_ADDRESS)
                .AddString(nameof(IOExcelImportConfiguration.AddressCellConfig), description: Locale.IO_SETTINGS_EXCELIMPORT_ADDRESS_DESC)

                .Section(Locale.IO_SETTINGS_EXCELIMPORT_IO_NAME)
                .AddString(nameof(IOExcelImportConfiguration.IONameCellConfig), description: Locale.IO_SETTINGS_EXCELIMPORT_IO_NAME_DESC)

                .Section(Locale.GENERICS_COMMENT)
                .AddString(nameof(IOExcelImportConfiguration.CommentCellConfig), description: Locale.IO_SETTINGS_EXCELIMPORT_COMMENT_DESC)

                .Section(Locale.IO_SETTINGS_EXCELIMPORT_STARTING_ROW)
                .AddString(nameof(IOExcelImportConfiguration.StartingRow), description: Locale.IO_SETTINGS_EXCELIMPORT_STARTING_ROW_DESC)

                .Section(Locale.IO_SETTINGS_EXCELIMPORT_EXPRESSION)
                .AddJavascript(nameof(IOExcelImportConfiguration.IgnoreRowExpressionConfig), description: Locale.IO_SETTINGS_EXCELIMPORT_EXPRESSION_DESC);
        }
    }
}
