using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Generation.GridHandler.Events
{
    public delegate void GridPreSortEventHandler(GridPreSortEventArgs args);
    public delegate void GridPostSortEventHandler(GridPostSortEventArgs args);

    public class GridPreSortEventArgs(SortOrder oldSortOrder, SortOrder sortOrder, int column)
    {
        public SortOrder OldSortOrder { get; init; } = oldSortOrder;
        public int Column {  get; init; } = column;
        public SortOrder SortOrder { get; set; } = sortOrder;
        public bool Handled { get; set; } = false;
    }

    public class GridPostSortEventArgs(SortOrder oldSortOrder, SortOrder sortOrder, int column)
    {
        public SortOrder OldSortOrder { get; init; } = oldSortOrder;
        public int Column { get; init; } = column;
        public SortOrder SortOrder { get; init; } = sortOrder;
    }
}
