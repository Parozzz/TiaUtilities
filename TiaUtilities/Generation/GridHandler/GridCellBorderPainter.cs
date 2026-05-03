using System.Diagnostics;
using TiaUtilities.Generation.GridHandler.CellPainters;
using static TiaUtilities.Generation.GridHandler.CellPainters.GridCellPaintHandler;

namespace TiaUtilities.Generation.GridHandler
{
    internal class GridCellBorderPainter(GridSettings gridSettings, DataGridView dataGridView) : IGridCellPainter
    {
        private readonly GridSettings gridSettings = gridSettings;
        private readonly DataGridView dataGridView = dataGridView;

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            return new PaintRequest().Border();
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest paintResult, bool backgroundRequested)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            // Verifichiamo se la cella corrente è attualmente selezionata
            bool isSelected = (args.State & DataGridViewElementStates.Selected) != 0;

            var style = args.CellStyle;

            // Evitiamo di sovrascrivere le intestazioni di riga e colonna (se vuoi colorare anche quelle, rimuovi questo controllo)
            if (rowIndex < 0 || columnIndex < 0)
            {
                return;
            }

            if (isSelected)
            {
                int lowestRowIndex = -1;
                int highestRowIndex = -1;

                int lowestColumnIndex = -1;
                int highestColumnIndex = -1;

                foreach (DataGridViewCell cell in this.dataGridView.SelectedCells)
                {
                    if (lowestRowIndex == -1 || cell.RowIndex < lowestRowIndex)
                    {
                        lowestRowIndex = cell.RowIndex;
                    }

                    if (highestRowIndex == -1 || cell.RowIndex > highestRowIndex)
                    {
                        highestRowIndex = cell.RowIndex;
                    }

                    if (lowestColumnIndex == -1 || cell.ColumnIndex < lowestColumnIndex)
                    {
                        lowestColumnIndex = cell.ColumnIndex;
                    }

                    if (highestColumnIndex == -1 || cell.ColumnIndex > highestColumnIndex)
                    {
                        highestColumnIndex = cell.ColumnIndex;
                    }
                }

                // --- L'ALGORITMO DEI VICINI ---
                // Controlliamo se le celle adiacenti esistono e se sono selezionate
                bool topSelected = rowIndex == highestRowIndex;
                bool bottomSelected = rowIndex == lowestRowIndex;
                bool leftSelected = columnIndex == lowestColumnIndex;
                bool rightSelected = columnIndex == highestColumnIndex;

                Debug.WriteLine($"Drawing: {rowIndex}, {columnIndex}. HR: {highestRowIndex}, LR:{lowestRowIndex}, HC: {highestColumnIndex}, LC: {lowestColumnIndex}, T:{topSelected},B:{bottomSelected},L:{leftSelected},R:{rightSelected}");

                // Impostiamo il pennello per il bordo di selezione (es. un bel blu acceso, spessore 2)
                using Pen borderPen = new(gridSettings.SingleSelectedCellBorderColor, 2);

                // 1. LATO SUPERIORE (Disegna se la cella sopra NON è selezionata)
                if (topSelected)
                {
                    graphics.DrawLine(borderPen, bounds.Left, bounds.Top + 0, bounds.Right, bounds.Top + 0);
                }

                // 2. LATO INFERIORE (Disegna se la cella sotto NON è selezionata)
                if (bottomSelected)
                {
                    graphics.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                }

                // 3. LATO SINISTRO (Disegna se la cella a sinistra NON è selezionata)
                if (leftSelected)
                {
                    graphics.DrawLine(borderPen, bounds.Left + 0, bounds.Top, bounds.Left + 0, bounds.Bottom);
                }

                // 4. LATO DESTRO (Disegna se la cella a destra NON è selezionata)
                if (rightSelected)
                {
                    graphics.DrawLine(borderPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
                }

            }
            // (OPZIONALE) Se vuoi mantenere i bordi INTERNI (griglia classica) più leggeri:
            /*
            using (Pen innerPen = new Pen(Color.LightGray, 1))
            {
                if (e.ColumnIndex != colCount - 1) // Disegna linea destra interna
                    e.Graphics.DrawLine(innerPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);

                if (e.RowIndex != rowCount - 1) // Disegna linea inferiore interna
                    e.Graphics.DrawLine(innerPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }
            */
            /*
            var currentCell = this.dataGridView.CurrentCell;
            if (currentCell != null && currentCell.RowIndex == rowIndex && currentCell.ColumnIndex == columnIndex && this.dataGridView.SelectedCells.Count == 1)
            {//I only want to apply the effect when the only selected cell is the current cell.
                //args.PaintBackground(bounds, true);

                //args.Paint(bounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using var borderPen = new Pen(gridSettings.SingleSelectedCellBorderColor, 2);

                //Border
                Rectangle rect = args.CellBounds;
                rect.Width -= 1;
                rect.Height -= 1;
                graphics.DrawRectangle(borderPen, rect);
            }
            */

        }

        public bool PaintContent(DataGridViewCellPaintingEventArgs args, bool backgroundPainter) => false;

        public bool PaintBackground(DataGridViewCellPaintingEventArgs args, bool backgroundPainter) => false;

        public bool PaintBorder(DataGridViewCellPaintingEventArgs args, bool backgroundPainter)
        {
            var bounds = args.CellBounds;
            var graphics = args.Graphics;

            var rowIndex = args.RowIndex;
            var columnIndex = args.ColumnIndex;

            // Verifichiamo se la cella corrente è attualmente selezionata
            bool isSelected = (args.State & DataGridViewElementStates.Selected) != 0;

            var style = args.CellStyle;

            // Evitiamo di sovrascrivere le intestazioni di riga e colonna (se vuoi colorare anche quelle, rimuovi questo controllo)
            if (!isSelected || rowIndex < 0 || columnIndex < 0)
            {
                return false;
            }
            int lowestRowIndex = -1;
            int highestRowIndex = -1;

            int lowestColumnIndex = -1;
            int highestColumnIndex = -1;

            foreach (DataGridViewCell cell in this.dataGridView.SelectedCells)
            {
                if (lowestRowIndex == -1 || cell.RowIndex < lowestRowIndex)
                {
                    lowestRowIndex = cell.RowIndex;
                }

                if (highestRowIndex == -1 || cell.RowIndex > highestRowIndex)
                {
                    highestRowIndex = cell.RowIndex;
                }

                if (lowestColumnIndex == -1 || cell.ColumnIndex < lowestColumnIndex)
                {
                    lowestColumnIndex = cell.ColumnIndex;
                }

                if (highestColumnIndex == -1 || cell.ColumnIndex > highestColumnIndex)
                {
                    highestColumnIndex = cell.ColumnIndex;
                }
            }

            /*bool topSelected = rowIndex == highestRowIndex;
            bool bottomSelected = rowIndex == lowestRowIndex;
            bool leftSelected = columnIndex == lowestColumnIndex;
            bool rightSelected = columnIndex == highestColumnIndex;*/

            //Debug.WriteLine($"Drawing: {rowIndex}, {columnIndex}. HR: {highestRowIndex}, LR:{lowestRowIndex}, HC: {highestColumnIndex}, LC: {lowestColumnIndex}, T:{topSelected},B:{bottomSelected},L:{leftSelected},R:{rightSelected}");
            /*
            using Pen borderPen = new(gridSettings.SingleSelectedCellBorderColor, 2);
            foreach (DataGridViewCell cell in this.dataGridView.SelectedCells)
            {
                var cellRowIndex = cell.RowIndex;
                var cellColumnIndex = cell.ColumnIndex;

                bool topSelected = cellRowIndex == highestRowIndex;
                bool bottomSelected = cellRowIndex == lowestRowIndex;
                bool leftSelected = cellColumnIndex == lowestColumnIndex;
                bool rightSelected = cellColumnIndex == highestColumnIndex;

                if (topSelected)
                {
                    graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top + 0, cellBounds.Right, cellBounds.Top + 0);
                }

                if (bottomSelected)
                {
                    graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right, cellBounds.Bottom - 1);
                }

                if (leftSelected)
                {
                    graphics.DrawLine(borderPen, cellBounds.Left + 0, cellBounds.Top, cellBounds.Left + 0, cellBounds.Bottom);
                }

                if (rightSelected)
                {
                    graphics.DrawLine(borderPen, cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom);
                }
            }

            */
            return true;
        }
    }
}
