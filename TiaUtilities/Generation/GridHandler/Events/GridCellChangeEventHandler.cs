﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;
using TiaXmlReader.GenerationForms;

namespace TiaXmlReader.Generation.GridHandler.Events
{
    public delegate void GridCellChangeEventHandler(GridCellChangeEventArgs args);

    public class GridCellChangeEventArgs : EventArgs
    {
        public List<GridCellChange>? CellChangeList {  get; set; }
        public bool IsUndo { get; set; }
    }
}