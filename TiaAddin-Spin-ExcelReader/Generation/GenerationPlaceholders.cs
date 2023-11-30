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


        public string Parse(string str)
        {
            return str.Replace("{nome_db}", dbName).Replace("{nome_utenza}", consumerName).Replace("{num_allarme}", alarmNum.ToString(alarmNumFormat));
        }
    }
}
