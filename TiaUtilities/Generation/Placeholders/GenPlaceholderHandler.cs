using TiaUtilities.Generation.Placeholders;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.Placeholders.Data;
using TiaXmlReader.Utility.Extensions;

namespace TiaXmlReader.Generation.Placeholders
{
    public class GenPlaceholderHandler
    {
        protected readonly Dictionary<string, IGenPlaceholderData> placeholdersDict = [];

        public void Clear()
        {
            placeholdersDict.Clear();
        }

        public string TabName { set => AddOrReplace(GenPlaceholders.Generation.TAB_NAME, new StringGenPlaceholderData() { Value = value }); }

        public IGridData GridData
        {
            set
            {
                if (this is IOGenPlaceholderHandler ioPlaceholderHandler)
                {
                    if (value is IOData ioData)
                    {
                        ioPlaceholderHandler.IOData = ioData;
                    }
                }
                else if (this is AlarmGenPlaceholdersHandler alarmPlaceholderHandler)
                {
                    if (value is DeviceData deviceData)
                    {
                        alarmPlaceholderHandler.DeviceData = deviceData;
                    }
                    else if (value is AlarmData alarmData)
                    {
                        alarmPlaceholderHandler.AlarmData = alarmData;
                    }
                }
            }
        }

        protected void AddOrReplace(string placeholder, IGenPlaceholderData placeholderData)
        {
            placeholdersDict.Compute(placeholder, placeholderData);
        }

        public string ParseNotNull(string? str)
        {
            var parsedStr = this.Parse(str);
            return parsedStr ?? "";
        }

        public string? Parse(string? str)
        {
            if (str == null)
            {
                return str;
            }

            var loopStr = str;
            foreach (KeyValuePair<string, IGenPlaceholderData> entry in placeholdersDict)
            {
                var placeholder = entry.Key;
                var placeholderData = entry.Value;

                if (string.IsNullOrEmpty(placeholder))
                {
                    continue;
                }

                if (loopStr == null)
                {
                    return null;
                }

                loopStr = loopStr.Replace(placeholder, placeholderData.GetSubstitution());
            }

            //{mnemonic} {bit_address} {byte_address} {cad_address} {cad_comment1} {cad_comment2} {cad_comment3} {cad_comment4} {cad_page} {cad_panel} {cad_type}
            return loopStr;
        }

        public bool ParseWithResult(ref string str)
        {
            bool anyFound = false;

            foreach (KeyValuePair<string, IGenPlaceholderData> entry in placeholdersDict)
            {
                var placeholder = entry.Key;
                var placeholderData = entry.Value;

                if (string.IsNullOrEmpty(placeholder))
                {
                    continue;
                }

                if (str.Contains(placeholder))
                {
                    anyFound = true;
                }

                str = str.Replace(placeholder, placeholderData.GetSubstitution());
            }

            return anyFound;
        }
    }

}
