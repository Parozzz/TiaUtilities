using SimaticML;
using SimaticML.Enums;
using TiaUtilities.Generation.GridHandler;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.IO.Configurations;
using TiaUtilities.Generation.IO.Data;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.SettingsStep;
using TiaUtilities.Utility;
using static TiaUtilities.SettingsStep.ControlFactory.SettingsFactoryGeneralOptions;

namespace TiaUtilities.Generation.IO
{
    public static class IOGenUtils
    {
        public static void DragPreview<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : GridData
        {
            var startingCellValue = gridHandler.DataSource[eventArgs.StartingRow][eventArgs.DraggedColumn];
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

        public static void DragDone<T>(GridExcelDragEventArgs eventArgs, GridHandler<T> gridHandler) where T : GridData
        {
            if (eventArgs.SelectedRowCount <= 0 || eventArgs.TopSelectedRow < 0)
            {
                return;
            }

            if (eventArgs.DraggedColumn == IOData.ADDRESS)
            {
                var startString = "" + gridHandler.DataSource[eventArgs.StartingRow][eventArgs.DraggedColumn];
                if (string.IsNullOrEmpty(startString))
                {
                    return;
                }

                var tagAddress = SimaticTagAddress.FromAddress(startString);
                if (tagAddress == null)  //If is not a valid address, i won't care about doing any stuff.
                {
                    return;
                }


                var req = gridHandler.DataChangedHandler.Join();

                try
                {
                    var rowIndexEnumeration = Enumerable.Range(eventArgs.TopSelectedRow, (int)eventArgs.SelectedRowCount);
                    if (!eventArgs.DraggingDown)
                    {
                        rowIndexEnumeration = rowIndexEnumeration.Reverse();
                    }

                    foreach (var rowIndex in rowIndexEnumeration)
                    {
                        gridHandler.DataSource[rowIndex][IOData.ADDRESS] = tagAddress.GetAddress();
                        var _ = eventArgs.DraggingDown ?
                            tagAddress.NextBit(SimaticDataType.BYTE) :
                            tagAddress.PreviousBit(SimaticDataType.BYTE); //Increase at the end. The first value is valid!
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowExceptionMessage(ex);
                }

                gridHandler.DataChangedHandler.End(req);
            }
            else
            {
                GridUtils.DragDone(eventArgs, gridHandler);
            }
        }

        public static List<SettingsSequencePanelDescriptor> CreateGlobalSettingsDescriptors()
        {
            var genericStepDescriptor = new SettingsSequencePanelDescriptor(Locale.GENERICS_CONFIGURATION)
            .CreateBinder<IOMainConfiguration>()
            .StartGroup(Locale.GENERICS_CONFIGURATION)
                .Add(x => x.GroupingType, Locale.IO_SETTINGS_GROUPING_TYPE, Locale.IO_SETTINGS_GROUPING_TYPE_DESC)
                .Add(x => x.MemoryType, Locale.IO_SETTINGS_MEMORY_TYPE, Locale.IO_SETTINGS_MEMORY_TYPE_DESC)
            .End();

            var ioTableStepDescriptor = new SettingsSequencePanelDescriptor(Locale.IO_GEN_CONFIG_IO_TABLE)
                .CreateBinder<IOMainConfiguration>()
                .StartGroup(Locale.IO_GEN_CONFIG_IO_TABLE)
                    .Add(x => x.IOTableName, Locale.GENERICS_NAME, Locale.IO_SETTINGS_IO_TABLE_NAME_DESC, options: new() { SupportPlaceholders = true })
                    .Add(x => x.IOTableSplitEvery, Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY, Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY_DESC)
                    .Add(x => x.DefaultIoName, Locale.IO_SETTINGS_IO_TABLE_DEFAULT_NAME, Locale.IO_SETTINGS_IO_TABLE_DEFAULT_NAME_DESC, options: new() { SupportPlaceholders = true })
                .End();

            var aliasDbStepDescriptor = new SettingsSequencePanelDescriptor(Locale.IO_GEN_CONFIG_ALIAS_DB/*, enabledFunc: () => mainConfig.MemoryType == IOMemoryTypeEnum.DB*/)
                .CreateBinder<IOMainConfiguration>()
                .StartGroup(Locale.IO_GEN_CONFIG_ALIAS_DB)
                    .Add(x => x.DBName, Locale.GENERICS_NAME, Locale.IO_SETTINGS_ALIAS_DB_NAME_DESC.Replace("<placeholder>", GenPlaceholders.IO.CONFIG_DB_NAME), options: new() { SupportPlaceholders = true })
                    .Add(x => x.DBNumber, Locale.GENERICS_NUMBER, Locale.IO_SETTINGS_ALIAS_DB_NUMBER_DESC.Replace("<placeholder>", GenPlaceholders.IO.CONFIG_DB_NAME))
                    .Add(x => x.DefaultDBInputVariable, Locale.IO_SETTINGS_ALIAS_DB_INPUT_DEFAULT, "", options: new() { SupportPlaceholders = true })
                    .Add(x => x.DefaultDBOutputVariable, Locale.IO_SETTINGS_ALIAS_DB_OUTPUT_DEFAULT, "", options: new() { SupportPlaceholders = true })
                .End();

            var aliasTableStepDescriptor = new SettingsSequencePanelDescriptor(Locale.IO_GEN_CONFIG_ALIAS_TABLE/*, enabledFunc: () => mainConfig.MemoryType == IOMemoryTypeEnum.MERKER*/)
                .CreateBinder<IOMainConfiguration>()
                .StartGroup(Locale.IO_GEN_CONFIG_ALIAS_TABLE)
                    .Add(x => x.VariableTableName, Locale.GENERICS_NAME, Locale.IO_SETTINGS_ALIAS_TABLE_NAME_DESC, options: new() { SupportPlaceholders = true })
                    .Add(x => x.VariableTableSplitEvery, Locale.IO_SETTINGS_ALIAS_TABLE_SPLIT_EVERY, Locale.IO_SETTINGS_IO_TABLE_SPLIT_EVERY_DESC)
                    .Add(x => x.VariableTableInputStartAddress, Locale.IO_SETTINGS_ALIAS_TABLE_INPUT_START_NUMBER, "")
                    .Add(x => x.DefaultMerkerInputVariable, Locale.IO_SETTINGS_ALIAS_TABLE_INPUT_DEFAULT_NAME, "", options: new() { SupportPlaceholders = true })
                    .Add(x => x.VariableTableOutputStartAddress, Locale.IO_SETTINGS_ALIAS_TABLE_OUTPUT_START_NUMBER, "")
                    .Add(x => x.DefaultMerkerOutputVariable, Locale.IO_SETTINGS_ALIAS_TABLE_OUTPUT_DEFAULT_NAME, "", options: new() { SupportPlaceholders = true })
                .End();

            return [genericStepDescriptor, ioTableStepDescriptor, aliasDbStepDescriptor, aliasTableStepDescriptor];
        }

        public static List<SettingsSequencePanelDescriptor> CreateTabSettingsDescriptors()
        {
            var fcStepDescriptor = new SettingsSequencePanelDescriptor(Locale.IO_GEN_CONFIG_FC)
            .CreateBinder<IOTabConfiguration>()
            .StartGroup(Locale.IO_GEN_CONFIG_FC)
                .Add(x => x.FCBlockName, Locale.GENERICS_NAME, "", options: new() { SupportPlaceholders = true })
                .Add(x => x.FCBlockNumber, Locale.GENERICS_NUMBER, "")
            .End();

            var segmentStepDescriptor = new SettingsSequencePanelDescriptor(Locale.IO_GEN_CONFIG_SEGMENT)
                .CreateBinder<IOTabConfiguration>()
                .StartGroup(Locale.IO_GEN_CONFIG_SEGMENT)
                    .Add(x => x.SegmentNameBitGrouping, Locale.IO_SETTINGS_SEGMENT_BIT_GROUPING, "", options: new() { SupportPlaceholders = true })
                    .Add(x => x.SegmentNameByteGrouping, Locale.IO_SETTINGS_SEGMENT_BYTE_GROUPING, "", options: new() { SupportPlaceholders = true })
                .End();

            return [fcStepDescriptor, segmentStepDescriptor];
        }

        public static List<SettingsSequencePanelDescriptor> CreateExcelSettingsDescriptors()
        {
            var descriptor = new SettingsSequencePanelDescriptor(Locale.GENERICS_ADDRESS)
                .CreateBinder<IOExcelImportConfiguration>()
                .StartGroup(Locale.GENERICS_ADDRESS)
                    .Add(x => x.AddressCellConfig, Locale.GENERICS_ADDRESS, description: Locale.IO_SETTINGS_EXCELIMPORT_ADDRESS_DESC)
                    .Add(x => x.IONameCellConfig, Locale.IO_SETTINGS_EXCELIMPORT_IO_NAME, description: Locale.IO_SETTINGS_EXCELIMPORT_IO_NAME_DESC, options: new() { StringEditor = StringCustomEditor.JS })
                    .Add(x => x.CommentCellConfig, Locale.GENERICS_COMMENT, description: Locale.IO_SETTINGS_EXCELIMPORT_COMMENT_DESC)
                    .Add(x => x.StartingRow, Locale.IO_SETTINGS_EXCELIMPORT_STARTING_ROW, description: Locale.IO_SETTINGS_EXCELIMPORT_STARTING_ROW_DESC)
                    .Add(x => x.IgnoreRowExpressionConfig, Locale.IO_SETTINGS_EXCELIMPORT_EXPRESSION, description: Locale.IO_SETTINGS_EXCELIMPORT_EXPRESSION_DESC, options: new() { StringEditor = StringCustomEditor.JS })
                .End();

            return [descriptor];
        }
    }
}
