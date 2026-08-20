using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.Alarms.Xml
{
    public class AlarmXmlBeta80Excel
    {
        private readonly XLWorkbook beta80Excel;
        private readonly IXLWorksheet beta80Sheet;
        private int beta80AlarmNum = 1;
        private int beta80ExcelRowIndex = 2; //Starts from second row since the first is the headers.

        public AlarmXmlBeta80Excel()
        {
            this.beta80Excel = new();

            this.beta80Sheet = this.beta80Excel.Worksheets.Add("Allarmi");
            //CD_ERROR	DSC_ERROR	ERROR_TYPE	CD_GROUP	CD_CONVEYOR
            this.beta80Sheet.Cell(1, 1).Value = "CD_ERROR";
            this.beta80Sheet.Cell(1, 2).Value = "DSC_ERROR";
            this.beta80Sheet.Cell(1, 3).Value = "ERROR_TYPE";
            this.beta80Sheet.Cell(1, 4).Value = "CD_GROUP";
            this.beta80Sheet.Cell(1, 5).Value = "CD_CONVEYOR";
        }

        public void AddItems(List<AlarmXmlItem> alarmItems)
        {
            foreach (var item in alarmItems)
            {
                if (!string.IsNullOrEmpty(item.HmiAlarmText))
                {
                    this.beta80Sheet.Cell(beta80ExcelRowIndex, 1).Value = beta80AlarmNum;
                    this.beta80Sheet.Cell(beta80ExcelRowIndex, 2).Value = item.HmiAlarmText;
                    this.beta80Sheet.Cell(beta80ExcelRowIndex, 3).Value = 4;
                    this.beta80Sheet.Cell(beta80ExcelRowIndex, 4).Value = item.TabName.Replace("Z", "");
                    this.beta80Sheet.Cell(beta80ExcelRowIndex, 5).Value = 1;
                }

                beta80AlarmNum++;
            }
        }

        public void Save(string exportPath)
        {
            var beta80Path = $"{exportPath}/Beta80Alarms.xlsx";
            this.beta80Excel.SaveAs(beta80Path);
        }
    }
}
