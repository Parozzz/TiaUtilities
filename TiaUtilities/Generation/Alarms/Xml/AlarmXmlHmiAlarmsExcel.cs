using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Languages;

namespace TiaUtilities.Generation.Alarms.XmlGenerator
{
    public class AlarmXmlHmiAlarmsExcel
    {
        private const int COLUMN_ID             = 1;
        private const int COLUMN_NAME           = 2;
        private const int COLUMN_CLASS          = 3;
        private const int COLUMN_ALARM_TEXT     = 4;
        private const int COLUMN_FIELD_INFO     = 5;
        private const int COLUMN_TRIGGER_TAG    = 6;
        private const int COLUMN_TRIGGER_BIT    = 7;
        private const int COLUMN_PARAM_1        = 8;
        private const int COLUMN_PARAM_2        = 9;
        private const int COLUMN_PARAM_3        = 10;
        private const int COLUMN_PARAM_4        = 11;
        private const int COLUMN_PARAM_5        = 12;
        private const int COLUMN_PARAM_6        = 13;
        private const int COLUMN_PARAM_7        = 14;
        private const int COLUMN_PARAM_8        = 15;
        private const int COLUMN_PARAM_9        = 16;
        private const int COLUMN_PARAM_10       = 17;

        private const string CELL_NO_VALUE = "<No value>";

        private readonly XLWorkbook workbook;
        private readonly IXLWorksheet discreteAlarmWorksheet;

        private int rowIndex = 1;

        public AlarmXmlHmiAlarmsExcel()
        {
            this.workbook = new XLWorkbook();
            this.discreteAlarmWorksheet = this.workbook.Worksheets.Add("DiscreteAlarms");

            this.InitFirstRow();
        }

        private void InitFirstRow()
        {
            this.discreteAlarmWorksheet.Cell(1, COLUMN_ID).Value = "ID";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_NAME).Value = "Name";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_CLASS).Value = "Class";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_ALARM_TEXT).Value = $"Alarm text [{LocaleVariables.CULTURE.IetfLanguageTag}], Alarm text 1";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_FIELD_INFO).Value = $"FieldInfo [Alarm text 1]";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_TRIGGER_TAG).Value = "Trigger tag";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_TRIGGER_BIT).Value = "Trigger bit";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_1).Value = "Alarm parameter 1";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_2).Value = "Alarm parameter 2";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_3).Value = "Alarm parameter 3";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_4).Value = "Alarm parameter 4";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_5).Value = "Alarm parameter 5";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_6).Value = "Alarm parameter 6";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_7).Value = "Alarm parameter 7";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_8).Value = "Alarm parameter 8";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_9).Value = "Alarm parameter 9";
            this.discreteAlarmWorksheet.Cell(1, COLUMN_PARAM_10).Value = "Alarm parameter 10";

            rowIndex = 2;
        }

        public void AddData(AlarmXmlItem item)
        {
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_ID).Value = item.HmiID;
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_NAME).Value = item.HmiAlarmName;
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_CLASS).Value = item.HmiAlarmClass;
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_ALARM_TEXT).Value = item.HmiAlarmText;
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_TRIGGER_TAG).Value = item.HmiTriggerTag;
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_TRIGGER_BIT).Value = item.HmiTriggerBit;

            for(int i = 0; i < 10; i++)
            {
                this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_PARAM_1 + i).Value = CELL_NO_VALUE;
            }

            int refId = 0;
            int parameterNumber = 1;

            var fieldStr = "";
            foreach(var field in item.HmiFields)
            {
                fieldStr = fieldStr + field.GetAsString(refId, parameterNumber) + '\n';

                this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_PARAM_1 + parameterNumber - 1).Value = field.Tag;

                refId++;
                parameterNumber++;
            }
            this.discreteAlarmWorksheet.Cell(rowIndex, COLUMN_FIELD_INFO).Value = fieldStr;
        }

        public void SaveAs(string filePath)
        {
            this.workbook.SaveAs(filePath);
        }
    }
}
