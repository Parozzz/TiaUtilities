using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaXmlReader.Generation
{
    public interface IGeneration
    {
        void ImportExcelConfig(IXLWorksheet worksheet);
        void GenerateBlocks();
        void ExportXML(string exportPath);
    }
}
