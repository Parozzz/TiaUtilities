﻿using TiaUtilities.Generation.GridHandler.Data;
using static TiaUtilities.Generation.GridHandler.CellPainters.GridCellPaintHandler;

namespace TiaUtilities.Generation.GridHandler.CellPainters
{
    public class GridCellPaintHandler
    {
        private readonly DataGridView dataGridView;
        private readonly List<IGridCellPainter> painterList;

        public GridCellPaintHandler(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            painterList = new List<IGridCellPainter>();
        }

        public void AddPainter(IGridCellPainter painter)
        {
            painterList.Add(painter);
        }

        public void AddPainterRange(ICollection<IGridCellPainter> painterCollection)
        {
            foreach (var painter in painterCollection)
            {
                AddPainter(painter);
            }
        }

        public void Init()
        {
            dataGridView.CellPainting += (sender, args) =>
            {

                bool backgroundDone = false, contentDone = false;

                var resultDict = new Dictionary<IGridCellPainter, PaintRequest>();
                foreach (var painter in painterList)
                {
                    var request = painter.PaintCellRequest(args);
                    if (!request.HasNone())
                    {
                        resultDict.Add(painter, request);
                    }
                }

                foreach (var entry in resultDict)
                {
                    var painter = entry.Key;
                    var request = entry.Value;

                    if (request.HasBackground())
                    {
                        backgroundDone |= request.HasBackground();
                        contentDone |= request.HasContent();

                        painter.PaintCell(args, request.Executed(), true);
                    }
                }

                foreach (var entry in resultDict)
                {
                    var painter = entry.Key;
                    var request = entry.Value;

                    if (!request.IsExecuted())
                    {
                        backgroundDone |= request.HasBackground();
                        contentDone |= request.HasContent();

                        painter.PaintCell(args, request.Executed(), !backgroundDone);
                    }
                }

                if (backgroundDone || contentDone)
                {
                    args.Handled = true;

                    if (!contentDone)
                    {
                        args.PaintContent(args.ClipBounds);
                    }
                }
            };
        }

        public class PaintRequest
        {
            private bool background;
            private bool content;
            private bool executed;
            public object data;
            public Preview dataPreview;

            public PaintRequest Background()
            {
                background = true;
                return this;
            }

            public PaintRequest Content()
            {
                content = true;
                return this;
            }

            public PaintRequest Executed()
            {
                executed = true;
                return this;
            }

            public bool HasBackground() => background;

            public bool HasContent() => content;

            public bool HasNone() => !background && !content;

            public bool IsExecuted() => executed;

        }
    }

    public interface IGridCellPainter
    {
        PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args);

        void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest paintResult, bool backgroundRequested);
    }
}
