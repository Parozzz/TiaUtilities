using SimaticML.Enums;
using TiaUtilities.Generation.GridHandler.Data;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.Placeholders.Data;

namespace TiaUtilities.Generation.Placeholders
{
    public class IOGenPlaceholderHandler(GridDataPreviewer<IOData> previewer, IOMainConfiguration mainConfig, IOTabConfiguration tabConfig) : GenPlaceholderHandler
    {
        public IOData IOData
        {
            set
            {
                AddOrReplace(GenPlaceholders.IO.MEMORY_TYPE, new StringGenPlaceholderData() { Value = value.GetAddressMemoryArea().GetSimaticMLString() });
                AddOrReplace(GenPlaceholders.IO.BIT, new StringGenPlaceholderData() { Value = "" + value.GetAddressBit() });
                AddOrReplace(GenPlaceholders.IO.BYTE, new StringGenPlaceholderData() { Value = "" + value.GetAddressByte() });

                string variable;
                if (string.IsNullOrEmpty(value.Variable))
                {
                    var preview = previewer.RequestPreview(IOData.VARIABLE, value);
                    variable = preview.ComposeDefaultValue();
                }
                else
                {
                    variable = value.Variable;
                }
                AddOrReplace(GenPlaceholders.IO.VARIABLE, new StringGenPlaceholderData() { Value = variable });


                string ioName;
                if (string.IsNullOrEmpty(value.IOName))
                {
                    var preview = previewer.RequestPreview(IOData.IO_NAME, value);
                    ioName = preview.ComposeDefaultValue();
                }
                else
                {
                    ioName = value.IOName;
                }
                AddOrReplace(GenPlaceholders.IO.IONAME, new StringGenPlaceholderData() { Value = this.ParseNotNull(ioName) }); // This one the last (Comment is not useful here!). The io name can contains other placeholders!
                AddOrReplace(GenPlaceholders.IO.COMMENT, new StringGenPlaceholderData() { Value = value.Comment });

                AddOrReplace(GenPlaceholders.IO.CONFIG_DB_NAME, new StringGenPlaceholderData() { Value = mainConfig.DBName });
                AddOrReplace(GenPlaceholders.IO.CONFIG_DB_NUMBER, new StringGenPlaceholderData() { Value = "" + mainConfig.DBNumber });
            }
        }
    }
}
