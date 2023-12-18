using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.AlarmGeneration;
using TiaXmlReader.Generation.IO_Cad;
using TiaXmlReader.SimaticML;

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
        Dictionary<string, IGenerationPlaceholderData> generationPlaceholdersDict;
        public GenerationPlaceholders()
        {
            generationPlaceholdersDict = new Dictionary<string, IGenerationPlaceholderData>();
        }

        public GenerationPlaceholders SetConsumerData(UserData consumerData)
        {
            AddOrReplace("{user_name}", new StringGenerationPlaceholderData()
            {
                Value = consumerData.Name
            });

            AddOrReplace("{user_description}", new StringGenerationPlaceholderData()
            {
                Value = consumerData.Description
            });

            return this;
        }

        public GenerationPlaceholders SetAlarmData(AlarmData alarmData)
        {
            AddOrReplace("{alarm_description}", new StringGenerationPlaceholderData()
            {
                Value = alarmData.Description
            });

            return this;
        }


        public GenerationPlaceholders SetAlarmNum(uint alarmNum, string alarmNumFormat)
        {
            return AddOrReplace("{alarm_num}", new UIntGenerationPlaceholderData()
            {
                Value = alarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholders SetStartEndAlarmNum(uint startAlarmNum, uint endAlarmNum, string alarmNumFormat)
        {
            AddOrReplace("{alarm_num_start}", new UIntGenerationPlaceholderData()
            {
                Value = startAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });

            return AddOrReplace("{alarm_num_end}", new UIntGenerationPlaceholderData()
            {
                Value = endAlarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholders SetCadData(CadData cadData)
        {
            AddOrReplace("{cad_address}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Address
            });

            AddOrReplace("{siemens_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Address,
                Function = (value) => CadData.GetSiemensMemoryType(value).GetInitial()
            });

            AddOrReplace("{cad_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Address,
                Function = (value) => CadData.GetCadMemoryType(value)
            });

            AddOrReplace("{bit_address}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Address,
                Function = (value) => "" + CadData.GetAddressBit(value)
            });

            AddOrReplace("{byte_address}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Address,
                Function = (value) => "" + CadData.GetAddressByte(value)
            });

            AddOrReplace("{cad_comment1}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Comment1
            });

            AddOrReplace("{cad_comment2}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Comment2
            });

            AddOrReplace("{cad_comment3}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Comment3
            });

            AddOrReplace("{cad_comment4}", new StringGenerationPlaceholderData()
            {
                Value = cadData.Comment4
            });

            AddOrReplace("{cad_page}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadPage
            });

            AddOrReplace("{cad_panel}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadPanel
            });

            AddOrReplace("{cad_type}", new StringGenerationPlaceholderData()
            {
                Value = cadData.CadType
            });

            return this;
        }

        public GenerationPlaceholders SetIOData(IOData ioData)
        {
            AddOrReplace("{siemens_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = ioData.GetMemoryArea().GetTIAMnemonic()
            });

            AddOrReplace("{bit_address}", new StringGenerationPlaceholderData()
            {
                Value = "" + ioData.GetAddressBit()
            });

            AddOrReplace("{byte_address}", new StringGenerationPlaceholderData()
            {
                Value = "" + ioData.GetAddressByte()
            });

            AddOrReplace("{io_tag_name}", new StringGenerationPlaceholderData()
            {
                Value = ioData.IOName
            });

            AddOrReplace("{db_name}", new StringGenerationPlaceholderData()
            {
                Value = ioData.DBName
            });

            AddOrReplace("{variable_name}", new StringGenerationPlaceholderData()
            {
                Value = ioData.VariableName
            });

            AddOrReplace("{comment}", new StringGenerationPlaceholderData()
            {
                Value = ioData.Comment
            });

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
            var loopStr = str;
            foreach (KeyValuePair<string, IGenerationPlaceholderData> entry in generationPlaceholdersDict)
            {
                var placeholder = entry.Key;
                var placeholderData = entry.Value;

                if (string.IsNullOrEmpty(placeholder))
                {
                    continue;
                }

                loopStr = loopStr.Replace(placeholder, placeholderData.GetSubstitution());
            }

            //{mnemonic} {bit_address} {byte_address} {cad_address} {cad_comment1} {cad_comment2} {cad_comment3} {cad_comment4} {cad_page} {cad_panel} {cad_type}
            return loopStr;
        }

        public string ParseFullOr(string str, string or)
        {
            return string.IsNullOrEmpty(str) ? this.Parse(or) : this.Parse(str);
        }
    }

}
