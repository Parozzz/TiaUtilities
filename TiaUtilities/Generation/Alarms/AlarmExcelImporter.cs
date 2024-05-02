using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Localization;
using TiaXmlReader.Generation.Alarms;

namespace TiaXmlReader.Generation.Alarms
{
    public class AlarmExcelImporter
    {
        private readonly AlarmConfiguration config;
        private readonly List<AlarmData> alarmDataList;
        private readonly List<DeviceData> userDataList;

        public AlarmExcelImporter()
        {
            this.config = new AlarmConfiguration();
            this.alarmDataList = new List<AlarmData>();
            this.userDataList = new List<DeviceData>();
        }

        public AlarmConfiguration GetConfiguration()
        {
            return config;
        }

        public List<AlarmData> GetAlarmDataList()
        {
            return alarmDataList;
        }

        public List<DeviceData> GetDeviceDataList()
        {
            return userDataList;
        }

        public void ImportExcelConfig(IXLWorksheet worksheet)
        {
            config.FCBlockName = worksheet.Cell("C5").Value.ToString();
            config.FCBlockNumber = (uint)worksheet.Cell("C6").Value.GetNumber();
            config.CoilFirst = worksheet.Cell("C7").Value.ToString().ToLower() == "true";

            if (!LocalizationHelper.TryGetEnumByTranslation(worksheet.Cell("C9").Value.ToString(), out AlarmPartitionType partitionType))
            {
                MessageBox.Show("Partition type is invalid.", "Invalid configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.PartitionType = partitionType;

            if (!LocalizationHelper.TryGetEnumByTranslation(worksheet.Cell("C10").Value.ToString(), out AlarmGroupingType groupingType))
            {
                MessageBox.Show("Grouping type is invalid.", "Invalid configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.GroupingType = groupingType;

            config.StartingAlarmNum = (uint)worksheet.Cell("C13").Value.GetNumber();
            config.AlarmNumFormat = worksheet.Cell("C14").Value.ToString();
            config.AntiSlipNumber = (uint)worksheet.Cell("C15").Value.GetNumber();
            config.SkipNumberAfterGroup = (uint)worksheet.Cell("C16").Value.GetNumber();
            config.GenerateEmptyAlarmAntiSlip = worksheet.Cell("C17").Value.ToString().ToLower() == "true";
            config.EmptyAlarmAtEnd = (uint)worksheet.Cell("C18").Value.GetNumber();
            config.EmptyAlarmContactAddress = worksheet.Cell("C19").Value.ToString();

            config.DefaultCoilAddress = worksheet.Cell("C22").Value.ToString();
            config.DefaultSetCoilAddress = worksheet.Cell("C23").Value.ToString();
            config.DefaultTimerAddress = worksheet.Cell("C24").Value.ToString();
            config.DefaultTimerType = worksheet.Cell("C25").Value.ToString();
            config.DefaultTimerValue = worksheet.Cell("C26").Value.ToString();

            config.AlarmAddressPrefix = worksheet.Cell("C29").Value.ToString();
            config.CoilAddressPrefix = worksheet.Cell("C30").Value.ToString();
            config.SetCoilAddressPrefix = worksheet.Cell("C31").Value.ToString();
            config.TimerAddressPrefix = worksheet.Cell("C32").Value.ToString();

            config.OneEachSegmentName = worksheet.Cell("C35").Value.ToString();
            config.OneEachEmptyAlarmSegmentName = worksheet.Cell("C36").Value.ToString();
            config.GroupSegmentName = worksheet.Cell("C37").Value.ToString();
            config.GroupEmptyAlarmSegmentName = worksheet.Cell("C38").Value.ToString();

            alarmDataList.Clear();

            uint variablesCellIndex = 5;
            while (true)
            {
                var userAddress = worksheet.Cell("H" + variablesCellIndex).Value;
                var coilAddress = worksheet.Cell("I" + variablesCellIndex).Value;
                var setCoilAddress = worksheet.Cell("J" + variablesCellIndex).Value;
                var timerAddress = worksheet.Cell("K" + variablesCellIndex).Value;
                var timerType = worksheet.Cell("L" + variablesCellIndex).Value;
                var timerValue = worksheet.Cell("M" + variablesCellIndex).Value;
                var description = worksheet.Cell("N" + variablesCellIndex).Value;
                var enable = worksheet.Cell("O" + variablesCellIndex).Value;
                variablesCellIndex++;

                if (!userAddress.IsText)
                {
                    break;
                }

                var alarmData = new AlarmData()
                {
                    AlarmVariable = userAddress.ToString(),
                    CoilAddress = coilAddress.ToString(),
                    SetCoilAddress = setCoilAddress.ToString(),
                    TimerAddress = timerAddress.ToString(),
                    TimerType = timerType.ToString(),
                    TimerValue = timerValue.ToString(),
                    Description = description.ToString(),
                    Enable = bool.TryParse(enable.ToString(), out bool enableResult) && enableResult,
                };
                alarmDataList.Add(alarmData);
            }

            userDataList.Clear();

            var consumerCellIndex = 5;
            while (true)
            {
                var userName = worksheet.Cell("E" + consumerCellIndex).Value;
                var userDescription = worksheet.Cell("F" + consumerCellIndex).Value;
                consumerCellIndex++;

                if (!userName.IsText)
                {
                    break;
                }

                var consumerData = new DeviceData()
                {
                    Address = userName.ToString(),
                    Description = userDescription.ToString(),
                };
                userDataList.Add(consumerData);
            }
        }
    }
}
