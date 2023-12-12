using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GenerationPlaceholders SetConsumerData(ConsumerData consumerData)
        {
            return this.SetConsumerName(consumerData.GetName()).SetDBName(consumerData.GetDbName());
        }

        public GenerationPlaceholders SetConsumerName(string consumerName)
        {
            return AddOrReplace("{nome_utenza}", new StringGenerationPlaceholderData()
            {
                Value = consumerName
            });
        }

        public GenerationPlaceholders SetDBName(string dbName)
        {
            return AddOrReplace("{nome_db}", new StringGenerationPlaceholderData()
            {
                Value = dbName
            });
        }

        public GenerationPlaceholders SetAlarmNum(uint alarmNum, string alarmNumFormat)
        {
            return AddOrReplace("{num_allarme}", new UIntGenerationPlaceholderData()
            {
                Value = alarmNum,
                Function = (value) => value.ToString(alarmNumFormat)
            });
        }

        public GenerationPlaceholders SetCadData(CadData cadData)
        {
            return this.SetAddress(cadData.Address)
                .SetComment1(cadData.Comment1)
                .SetComment2(cadData.Comment2)
                .SetComment3(cadData.Comment3)
                .SetComment4(cadData.Comment4)
                .SetCadPage(cadData.CadPage)
                .SetCadPanel(cadData.CadPanel)
                .SetCadPage(cadData.CadPage);
        }

        public GenerationPlaceholders SetAddress(string address)
        {
            AddOrReplace("{cad_address}", new StringGenerationPlaceholderData()
            {
                Value = address
            });
            AddOrReplace("{siemens_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = address,
                Function = (value) => CadData.GetSiemensMemoryType(address).GetInitial()
            });
            AddOrReplace("{cad_memory_type}", new StringGenerationPlaceholderData()
            {
                Value = address,
                Function = (value) => CadData.GetCadMemoryType(address)
            });
            AddOrReplace("{bit_address}", new StringGenerationPlaceholderData() 
            { 
                Value = address, 
                Function = (value) => "" + CadData.GetAddressBit(address) 
            });
            return AddOrReplace("{byte_address}", new StringGenerationPlaceholderData()
            {
                Value = address,
                Function = (value) => "" + CadData.GetAddressByte(address)
            });
        }

        public GenerationPlaceholders SetComment1(string comment1)
        {
            return AddOrReplace("{cad_comment1}", new StringGenerationPlaceholderData()
            {
                Value = comment1
            });
        }

        public GenerationPlaceholders SetComment2(string comment2)
        {
            return AddOrReplace("{cad_comment2}", new StringGenerationPlaceholderData()
            {
                Value = comment2
            });
        }

        public GenerationPlaceholders SetComment3(string comment3)
        {
            return AddOrReplace("{cad_comment3}", new StringGenerationPlaceholderData()
            {
                Value = comment3
            });
        }

        public GenerationPlaceholders SetComment4(string comment4)
        {
            return AddOrReplace("{cad_comment4}", new StringGenerationPlaceholderData()
            {
                Value = comment4
            });
        }

        public GenerationPlaceholders SetCadPage(string cadPage)
        {
            return AddOrReplace("{cad_page}", new StringGenerationPlaceholderData()
            {
                Value = cadPage
            });
        }

        public GenerationPlaceholders SetCadPanel(string cadPanel)
        {
            return AddOrReplace("{cad_panel}", new StringGenerationPlaceholderData()
            {
                Value = cadPanel
            });
        }
        public GenerationPlaceholders SetCadType(string cadType)
        {
            return AddOrReplace("{cad_type}", new StringGenerationPlaceholderData() 
            { 
                Value = cadType 
            });
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
                Value = ioData.IOTagName
            });

            AddOrReplace("{io_tag_comment}", new StringGenerationPlaceholderData()
            {
                Value = ioData.IOTagComment
            });

            AddOrReplace("{support_variable_comment}", new StringGenerationPlaceholderData()
            {
                Value = ioData.VariableComment
            });

            return this;

        }

        private GenerationPlaceholders AddOrReplace(string placeholder, IGenerationPlaceholderData placeholderData)
        {
            if(generationPlaceholdersDict.Keys.Contains(placeholder))
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
            foreach(KeyValuePair<string, IGenerationPlaceholderData> entry in generationPlaceholdersDict)
            {
                var placeholder = entry.Key;
                var placeholderData = entry.Value;

                if(string.IsNullOrEmpty(placeholder))
                {
                    continue;
                }

                loopStr =  loopStr.Replace(placeholder, placeholderData.GetSubstitution());
            }

            //{mnemonic} {bit_address} {byte_address} {cad_address} {cad_comment1} {cad_comment2} {cad_comment3} {cad_comment4} {cad_page} {cad_panel} {cad_type}
            return loopStr;
        }
    }

}
