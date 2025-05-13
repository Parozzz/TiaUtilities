using Newtonsoft.Json;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.Placeholders.Data;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.Placeholders
{
    public class GenPlaceholderHandler
    {
        protected readonly Dictionary<string, IGenPlaceholderData> placeholdersDict = [];

        public void Clear()
        {
            placeholdersDict.Clear();
        }

        public string TabName { set => AddOrReplace(TiaUtilities.Generation.Placeholders.Generation.TAB_NAME, new StringGenPlaceholderData() { Value = value }); }

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

        public void LoadJSONObject(string json)
        {
            try
            {
                var placeholderDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (placeholderDict == null)
                {
                    return;
                }

                foreach (var item in placeholderDict)
                {
                    var placeholder = item.Key;
                    if (!placeholder.StartsWith('{'))
                    {
                        placeholder = '{' + placeholder;
                    }
                    if (!placeholder.EndsWith('}'))
                    {
                        placeholder = placeholder + '}';
                    }

                    var value = item.Value;
                    if (value == null)
                    {
                        continue;
                    }

                    var stringValue = value.ToString();
                    if (stringValue == null)
                    {
                        continue;
                    }

                    this.AddOrReplace(placeholder, new StringGenPlaceholderData() { Value = stringValue });
                }
            }
            catch (Exception)
            {

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

            var localStr = str;

            while (true)
            {
                //Parse until no more placeholders are found!
                var result = this.ParseWithResult(ref localStr);
                if (!result)
                {
                    break;
                }
            }

            //{mnemonic} {bit_address} {byte_address} {cad_address} {cad_comment1} {cad_comment2} {cad_comment3} {cad_comment4} {cad_page} {cad_panel} {cad_type}
            return localStr;
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
                    str = str.Replace(placeholder, placeholderData.GetSubstitution());
                }

            }

            return anyFound;
        }
    }

}
