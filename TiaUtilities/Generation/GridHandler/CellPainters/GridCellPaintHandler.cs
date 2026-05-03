using TiaUtilities.Generation.GridHandler.Data;
using static TiaUtilities.Generation.GridHandler.CellPainters.GridCellPaintHandler;

namespace TiaUtilities.Generation.GridHandler.CellPainters
{
    public class GridCellPaintHandler(DataGridView dataGridView)
    {
        private readonly DataGridView dataGridView = dataGridView;
        private readonly List<IGridCellPainter> painterList = [];

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

                args.PaintBackground(args.CellBounds, true);
                //args.PaintContent(args.CellBounds);

                var resultDict = new Dictionary<IGridCellPainter, PaintRequest>();
                foreach (var painter in painterList)
                {
                    backgroundDone |= painter.PaintBackground(args, backgroundDone);
                }

                foreach (var painter in painterList)
                {
                    contentDone |= painter.PaintContent(args, backgroundDone);
                }

                if(!contentDone)
                {
                    args.PaintContent(args.CellBounds);
                }

                foreach (var painter in painterList)
                {
                    contentDone |= painter.PaintBorder(args, backgroundDone);
                }

                args.Handled = true;

                /*
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

                    if (!request.IsExecuted() && !request.HasBorder())
                    {
                        backgroundDone |= request.HasBackground();
                        contentDone |= request.HasContent();

                        painter.PaintCell(args, request.Executed(), !backgroundDone);
                    }
                }

                if (!backgroundDone)
                {
                    args.PaintBackground(args.ClipBounds, true);
                }

                if (!contentDone)
                {
                    args.PaintContent(args.ClipBounds);
                }

                args.Handled = true;

                foreach (var entry in resultDict)
                {
                    var painter = entry.Key;
                    var request = entry.Value;

                    if (!request.IsExecuted() && request.HasBorder())
                    {
                        painter.PaintCell(args, request.Executed(), !backgroundDone);
                    }
                }*/
            };
        }

        public class PaintRequest
        {
            private bool background;
            private bool content;
            private bool border;
            private bool executed;
            public object data;
            public GridDataPreview dataPreview;

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

            public PaintRequest Border()
            {
                border = true;
                return this;
            }

            public PaintRequest Executed()
            {
                executed = true;
                return this;
            }

            public bool HasBackground() => background;

            public bool HasContent() => content;

            public bool HasBorder() => border;

            public bool HasNone() => !background && !content && !border;

            public bool IsExecuted() => executed;

        }
    }

    public interface IGridCellPainter
    {
        bool PaintContent(DataGridViewCellPaintingEventArgs args, bool backgroundPainter);
        bool PaintBackground(DataGridViewCellPaintingEventArgs args, bool backgroundPainter);
        bool PaintBorder(DataGridViewCellPaintingEventArgs args, bool backgroundPainter);
    }
}
