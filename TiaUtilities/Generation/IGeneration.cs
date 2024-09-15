using ClosedXML.Excel;

namespace TiaXmlReader.Generation
{
    public interface IGeneration
    {
        void ImportExcelConfig(IXLWorksheet worksheet);
        void GenerateBlocks();
        void ExportXML(string exportPath);
    }
}
