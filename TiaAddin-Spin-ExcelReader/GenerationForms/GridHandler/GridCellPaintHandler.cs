using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static TiaXmlReader.GenerationForms.GridHandler.GridCellPaintHandler;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.GenerationForms.IO;

namespace TiaXmlReader.GenerationForms.GridHandler
{
    public class GridCellPaintHandler
    {
        private readonly DataGridView dataGridView;
        private readonly List<IGridCellPainter> painterList;

        public GridCellPaintHandler(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            this.painterList = new List<IGridCellPainter>();
        }

        public void AddPainter(IGridCellPainter painter)
        {
            painterList.Add(painter);
        }

        public void AddPainterRange(ICollection<IGridCellPainter> painterCollection)
        {
            foreach(var painter in painterCollection)
            {
                this.AddPainter(painter);
            }
        }

        public void Init()
        {
            this.dataGridView.CellPainting += (object sender, DataGridViewCellPaintingEventArgs args) =>
            {

                bool backgroundDone = false, contentDone = false;

                var resultDict = new Dictionary<IGridCellPainter, PaintRequest>();
                foreach (var painter in painterList)
                {
                    var request = painter.PaintCellRequest(args);
                    if(!request.HasNone())
                    {
                        resultDict.Add(painter, request);
                    }
                }

                foreach(var entry in resultDict)
                {
                    var painter = entry.Key;
                    var request = entry.Value;

                    if(request.HasBackground())
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

                if(backgroundDone || contentDone)
                {
                    args.Handled = true;

                    if(!contentDone)
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
