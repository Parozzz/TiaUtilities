using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.Generation.Placeholders.Data;
using SimaticML.Enums;

namespace TiaXmlReader.Generation.Placeholders
{
    public class GenerationPlaceholderHandler
    {
        Dictionary<string, IGenerationPlaceholderData> generationPlaceholdersDict;
        public GenerationPlaceholderHandler()
        {
            generationPlaceholdersDict = new Dictionary<string, IGenerationPlaceholderData>();
        }

        public void Clear()
        {
            generationPlaceholdersDict.Clear();
        }

        public GenerationPlaceholderHandler SetGridData<C>(IGridData<C> gridData, C configuration) where C : IGenerationConfiguration
        {
            if (gridData is DeviceData deviceData)
            {
                return this.SetDeviceData(deviceData);
            }
            else if (gridData is AlarmData alarmData)
            {
                return this.SetAlarmData(alarmData);
            }
            else if (gridData is IOData ioData && configuration is IOConfiguration ioConfig)
            {
                return this.SetIOData(ioData, ioConfig);
            }

            return this;
        }

        public GenerationPlaceholderHandler SetDeviceData(DeviceData consumerData)
        {
            AddOrReplace(GenerationPlaceholders.Alarms.DEVICE_NAME, new StringGenerationPlaceholderData() { Value = consumerData.Name });
            AddOrReplace(GenerationPlaceholders.Alarms.DEVICE_ADDRESS, new StringGenerationPlaceholderData() { Value = consumerData.Address });
            AddOrReplace(GenerationPlaceholders.Alarms.DEVICE_DESCRIPTION, new StringGenerationPlaceholderData() { Value = consumerData.Description });
            return this;
        }

        public GenerationPlaceholderHandler SetAlarmData(AlarmData alarmData)
        {
            AddOrReplace(GenerationPlaceholders.Alarms.ALARM_DESCRIPTION, new StringGenerationPlaceholderData() { Value = alarmData.Description });
            return this;
        }


        public GenerationPlaceholderHandler SetAlarmNum(uint alarmNum, string alarmNumFormat)
        {
            return AddOrReplace(GenerationPlaceholders.Alarms.ALARM_NUM, new UIntGenerationPlaceholderData()
            {
                Value = alarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholderHandler SetStartEndAlarmNum(uint startAlarmNum, uint endAlarmNum, string alarmNumFormat)
        {
            AddOrReplace(GenerationPlaceholders.Alarms.ALARM_NUM_START, new UIntGenerationPlaceholderData()
            {
                Value = startAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });

            return AddOrReplace(GenerationPlaceholders.Alarms.ALARM_NUM_END, new UIntGenerationPlaceholderData()
            {
                Value = endAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholderHandler SetIOData(IOData ioData, IOConfiguration config)
        {
            if(config == null)
            {
                throw new ArgumentNullException("config", "Impossible to set IOData to placeholders without its config.");
            }

            AddOrReplace(GenerationPlaceholders.IO.MEMORY_TYPE, new StringGenerationPlaceholderData() { Value = ioData.GetAddressMemoryArea().GetSimaticMLString() });
            AddOrReplace(GenerationPlaceholders.IO.BIT, new StringGenerationPlaceholderData() { Value = "" + ioData.GetAddressBit() });
            AddOrReplace(GenerationPlaceholders.IO.BYTE, new StringGenerationPlaceholderData() { Value = "" + ioData.GetAddressByte() });
            //AddOrReplace("{db_name}", new StringGenerationPlaceholderData() { Value = ioData.DBName });

            string variable;
            if (string.IsNullOrEmpty(ioData.Variable))
            {
                var preview = ioData.GetPreview(IOData.VARIABLE.ColumnIndex, config);
                variable = preview.ComposeDefaultValue();
            }
            else
            {
                variable = ioData.Variable;
            }
            AddOrReplace(GenerationPlaceholders.IO.VARIABLE, new StringGenerationPlaceholderData() { Value = variable });


            string ioName;
            if (string.IsNullOrEmpty(ioData.IOName))
            {
                var preview = ioData.GetPreview(IOData.IO_NAME.ColumnIndex, config);
                ioName = preview.ComposeDefaultValue();
            }
            else
            {
                ioName = ioData.IOName;
            }
            AddOrReplace(GenerationPlaceholders.IO.IONAME, new StringGenerationPlaceholderData() { Value = this.Parse(ioName) }); // This one the last (Comment is not useful here!). The io name can contains other placeholders!
            AddOrReplace(GenerationPlaceholders.IO.COMMENT, new StringGenerationPlaceholderData() { Value = ioData.Comment });

            AddOrReplace(GenerationPlaceholders.IO.CONFIG_DB_NAME, new StringGenerationPlaceholderData() { Value = config.DBName });
            AddOrReplace(GenerationPlaceholders.IO.CONFIG_DB_NUMBER, new StringGenerationPlaceholderData() { Value = "" + config.DBNumber });

            return this;
        }

        private GenerationPlaceholderHandler AddOrReplace(string placeholder, IGenerationPlaceholderData placeholderData)
        {
            if (generationPlaceholdersDict.Keys.Contains(placeholder))
            {
                generationPlaceholdersDict[placeholder] = placeholderData;
                return this;
            }

            generationPlaceholdersDict.Add(placeholder, placeholderData);
            return this;
        }

        public string Parse(string str)
        {
            if (str == null)
            {
                return str;
            }

            var loopStr = str;
            foreach (KeyValuePair<string, IGenerationPlaceholderData> entry in generationPlaceholdersDict)
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

            foreach (KeyValuePair<string, IGenerationPlaceholderData> entry in generationPlaceholdersDict)
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
