using SpinXmlReader.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation
{
    public class GenerationPlaceholders
    {
        private string consumerName;
        private string dbName;

        private string alarmNumFormat;
        private uint alarmNum;

        //CAD 
        private string address;
        private string comment1;
        private string comment2;
        private string comment3;
        private string comment4;
        private string cadPage;
        private string cadPanel;
        private string cadType;

        public GenerationPlaceholders()
        {

        }

        public GenerationPlaceholders SetConsumerData(ConsumerData consumerData)
        {
            return this.SetConsumerName(consumerData.GetName()).SetDBName(consumerData.GetDbName());
        }

        public GenerationPlaceholders SetConsumerName(string consumerName)
        {
            this.consumerName = consumerName;
            return this;
        }

        public GenerationPlaceholders SetDBName(string dbName)
        {
            this.dbName = dbName;
            return this;
        }

        public GenerationPlaceholders SetAlarmNumFormat(string alarmNumFormat)
        {
            this.alarmNumFormat = alarmNumFormat;
            return this;
        }

        public GenerationPlaceholders SetAlarmNum(uint alarmNum)
        {
            this.alarmNum = alarmNum;
            return this;
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
            this.address = address;
            return this;
        }

        public GenerationPlaceholders SetComment1(string comment1)
        {
            this.comment1 = comment1;
            return this;
        }

        public GenerationPlaceholders SetComment2(string comment2)
        {
            this.comment2 = comment2;
            return this;
        }

        public GenerationPlaceholders SetComment3(string comment3)
        {
            this.comment3 = comment3;
            return this;
        }

        public GenerationPlaceholders SetComment4(string comment4)
        {
            this.comment4 = comment4;
            return this;
        }

        public GenerationPlaceholders SetCadPage(string cadPage)
        {
            this.cadPage = cadPage;
            return this;
        }

        public GenerationPlaceholders SetCadPanel(string cadPanel)
        {
            this.cadPanel = cadPanel;
            return this;
        }
        public GenerationPlaceholders SetCadType(string cadType)
        {
            this.cadType = cadType;
            return this;
        }

        public string Parse(string str)
        {
            var dict = new Dictionary<string, string>()
            {
                { "{nome_db}", dbName },
                { "{nome_utenza}", consumerName },
                { "{num_allarme}", alarmNum.ToString(alarmNumFormat) },
                { "{bit_address}", "" + CadData.GetAddressBit(address) },
                { "{byte_address}", "" + CadData.GetAddressByte(address) },
                { "{cad_address}", address },
                { "{cad_comment1}", comment1 },
                { "{cad_comment2}", comment2 },
                { "{cad_comment3}", comment3 },
                { "{cad_comment4}", comment4 },
                { "{cad_page}", cadPage },
                { "{cad_panel}", cadPanel },
                { "{cad_type}", cadType }
            };

            var loopStr = str;
            foreach(KeyValuePair<string, string> entry in dict)
            {
                var placeholder = entry.Key;
                var substitution = entry.Value;

                if(string.IsNullOrEmpty(placeholder) || string.IsNullOrEmpty(substitution))
                {
                    continue;
                }

                loopStr.Replace(placeholder, substitution);
            }

            //{mnemonic} {bit_address} {byte_address} {cad_address} {cad_comment1} {cad_comment2} {cad_comment3} {cad_comment4} {cad_page} {cad_panel} {cad_type}
            return loopStr;
        }
    }
}
