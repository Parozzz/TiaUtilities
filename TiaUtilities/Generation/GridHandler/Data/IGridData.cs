using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Localization;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Data
{
    public interface IGridData<C> where C : IGenerationConfiguration
    { //CLASS THAT IMPLEMENT THIS MUST HAVE AN EMPTY CONSTRUCTOR!
        GridDataPreview? GetPreview(GridDataColumn column, C config);
        GridDataPreview? GetPreview(int column, C config);
        void Clear();
        bool IsEmpty();
        IReadOnlyList<GridDataColumn> GetColumns();
        GridDataColumn GetColumn(int column);
        object? this[int column] {  get; }
    }
}
