using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.IO_Cad;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Generation.UserAlarms;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation
{
    public interface IGenerationPlaceholderData
    {
        string GetSubstitution();
    }

    public class StringGenerationPlaceholderData : IGenerationPlaceholderData
    {
        public string Value { get; set; }
        public Func<string, string> Function { get; set; }

        public StringGenerationPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function != null ? Function.Invoke(Value) : Value;
        }
    }

    public class UIntGenerationPlaceholderData : IGenerationPlaceholderData
    {
        public uint Value { get; set; }
        public Func<uint, string> Function { get; set; }

        public UIntGenerationPlaceholderData()
        {

        }

        public string GetSubstitution()
        {
            return Function.Invoke(Value);
        }
    }

    public class GenerationPlaceholders
    {
        public const string DEVICE_ADDRESS = "{device_address}";
        public const string DEVICE_DESCRIPTION = "{device_description}";
        public const string ALARM_DESCRIPTION = "{alarm_description}";
        public const string ALARM_NUM = "{alarm_num}";
        public const string ALARM_NUM_START = "{alarm_num_start}";
        public const string ALARM_NUM_END = "{alarm_num_end}";

        Dictionary<string, IGenerationPlaceholderData> generationPlaceholdersDict;
        public GenerationPlaceholders()
        {
            generationPlaceholdersDict = new Dictionary<string, IGenerationPlaceholderData>();
        }

        public void Clear()
        {
            generationPlaceholdersDict.Clear();
        }

        public GenerationPlaceholders SetGridData<C>(IGridData<C> gridData, C configuration) where C : IGenerationConfiguration
        {
            if (gridData is DeviceData deviceData)
            {
                return this.SetDeviceData(deviceData);
            }
            else if (gridData is AlarmData alarmData)
            {
                return this.SetAlarmData(alarmData);
            }
            else if (gridData is IOData ioData)
            {
                return configuration is IOConfiguration ioConfig ? this.SetIOData(ioData, ioConfig) : this.SetIOData(ioData);
            }

            return this;
        }

        public GenerationPlaceholders SetDeviceData(DeviceData consumerData)
        {
            AddOrReplace(DEVICE_ADDRESS, new StringGenerationPlaceholderData() { Value = consumerData.Address });
            AddOrReplace(DEVICE_DESCRIPTION, new StringGenerationPlaceholderData() { Value = consumerData.Description });
            return this;
        }

        public GenerationPlaceholders SetAlarmData(AlarmData alarmData)
        {
            AddOrReplace(ALARM_DESCRIPTION, new StringGenerationPlaceholderData() { Value = alarmData.Description });
            return this;
        }


        public GenerationPlaceholders SetAlarmNum(uint alarmNum, string alarmNumFormat)
        {
            return AddOrReplace(ALARM_NUM, new UIntGenerationPlaceholderData()
            {
                Value = alarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholders SetStartEndAlarmNum(uint startAlarmNum, uint endAlarmNum, string alarmNumFormat)
        {
            AddOrReplace(ALARM_NUM_START, new UIntGenerationPlaceholderData()
            {
                Value = startAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });

            return AddOrReplace(ALARM_NUM_END, new UIntGenerationPlaceholderData()
            {
                Value = endAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholders SetCadData(CadData cadData)
        {
            AddOrReplace("{siemens_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadAddress,
                Function = (value) => CadData.GetMemoryArea(value).GetInitial()
            });

            AddOrReplace("{cad_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadAddress,
                Function = (value) => CadData.GetCadMemoryType(value)
            });

            AddOrReplace("{bit}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadAddress,
                Function = (value) => "" + CadData.GetAddressBit(value)
            });

            AddOrReplace("{byte}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadAddress,
                Function = (value) => "" + CadData.GetAddressByte(value)
            });

            AddOrReplace("{cad_address}", new StringGenerationPlaceholderData() { Value = cadData.CadAddress });
            AddOrReplace("{io_name}", new StringGenerationPlaceholderData() { Value = cadData.IOName });
            AddOrReplace("{db_name}", new StringGenerationPlaceholderData() { Value = cadData.DBName });
            AddOrReplace("{variable_name}", new StringGenerationPlaceholderData() { Value = cadData.VariableName });
            AddOrReplace("{comment1}", new StringGenerationPlaceholderData() { Value = cadData.Comment1 });
            AddOrReplace("{comment2}", new StringGenerationPlaceholderData() { Value = cadData.Comment2 });
            AddOrReplace("{comment3}", new StringGenerationPlaceholderData() { Value = cadData.Comment3 });
            AddOrReplace("{comment4}", new StringGenerationPlaceholderData() { Value = cadData.Comment4 });

            string[] joinedCommentList = { cadData.Comment1, cadData.Comment2, cadData.Comment3, cadData.Comment4 };
            AddOrReplace("{joined_comments}", new StringGenerationPlaceholderData()
            {
                Value = string.Join(" ", joinedCommentList.Where(str => !string.IsNullOrWhiteSpace(str)).ToList())
            });

            AddOrReplace("{mnemonic}", new StringGenerationPlaceholderData() { Value = cadData.Mnemonic });
            AddOrReplace("{wire_num}", new StringGenerationPlaceholderData() { Value = cadData.WireNum });
            AddOrReplace("{page}", new StringGenerationPlaceholderData() { Value = cadData.Page });
            AddOrReplace("{panel}", new StringGenerationPlaceholderData() { Value = cadData.Panel });

            return this;
        }

        public GenerationPlaceholders SetIOData(IOData ioData, IOConfiguration config = null)
        {
            AddOrReplace("{memory_type}", new StringGenerationPlaceholderData() { Value = ioData.GetMemoryArea().GetTIAMnemonic() });
            AddOrReplace("{bit}", new StringGenerationPlaceholderData() { Value = "" + ioData.GetAddressBit() });
            AddOrReplace("{byte}", new StringGenerationPlaceholderData() { Value = "" + ioData.GetAddressByte() });
            AddOrReplace("{db_name}", new StringGenerationPlaceholderData() { Value = ioData.DBName });

            var variable = string.IsNullOrEmpty(ioData.Variable) && config != null ? config.DefaultVariableName : ioData.Variable;
            var ioName = string.IsNullOrEmpty(ioData.IOName) && config != null ? config.DefaultIoName : ioData.IOName;
            AddOrReplace("{variable_name}", new StringGenerationPlaceholderData() { Value = variable });
            AddOrReplace("{io_name}", new StringGenerationPlaceholderData() { Value = this.Parse(ioName) }); // This one the last (Comment is not useful here!). The io name can contains other placeholders!

            AddOrReplace("{comment}", new StringGenerationPlaceholderData() { Value = ioData.Comment });
            return this;
        }

        private GenerationPlaceholders AddOrReplace(string placeholder, IGenerationPlaceholderData placeholderData)
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
