using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.GenerationForms.IO.Data;
using static TiaXmlReader.GenerationForms.IO.IOGenerationCellPaintHandler;

namespace TiaXmlReader.GenerationForms.IO
{
    public class IOGenerationFormPreviewCellPainter : IOGenerationCellPainter
    {
        private readonly DataGridView dataGridView;
        private readonly IOGenerationDataSource dataSource;
        private readonly IOConfiguration config;

        private readonly GenerationPlaceholders placeholders;
        public IOGenerationFormPreviewCellPainter(DataGridView dataGridView, IOGenerationDataSource dataSource, IOConfiguration config)
        {
            this.dataGridView = dataGridView;
            this.dataSource = dataSource;
            this.config = config;

            this.placeholders = new GenerationPlaceholders();
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintRequest = new PaintRequest();

            var columnIndex = args.ColumnIndex;
            var rowIndex = args.RowIndex;

            if (rowIndex < 0 || columnIndex < 1 || columnIndex > 3)
            {
                return paintRequest;
            }

            var generationData = dataSource.GetDataAt(rowIndex);
            if (generationData.IsEmpty())
            {
                return paintRequest;
            }

            paintRequest.data = generationData;
            if (columnIndex == 1)
            {
                return string.IsNullOrEmpty(generationData.IOName) ? paintRequest.Content() : paintRequest;
            } 
            else if(columnIndex == 3)
            {
                return !string.IsNullOrEmpty(generationData.Variable) ? paintRequest.Content() : paintRequest;
            }

            return paintRequest;
        }

        public void PaintCell(DataGridViewCellPaintingEventArgs args, PaintRequest request, bool backgroundRequested)
        {
            if (!request.HasContent())
            {
                return;
            }

            if (backgroundRequested)
            {
                args.PaintBackground(args.ClipBounds, true);
            }

            var columnIndex = args.ColumnIndex;
            var rowIndex = args.RowIndex;
            var bounds = args.CellBounds;
            var graphics = args.Graphics;
            var style = args.CellStyle;

            try
            {
                var ioData = ((IOGenerationData)request.data).CreateIOData();

                placeholders.Clear();
                placeholders.SetIOData(ioData);

                if (columnIndex == 1)
                {
                    var ioName = ioData.IOName;
                    if (string.IsNullOrEmpty(ioName))
                    {
                        var defaultIOName = placeholders.Parse(config.DefaultIoName);
                        if (!string.IsNullOrEmpty(defaultIOName))
                        {
                            var size = TextRenderer.MeasureText(defaultIOName, style.Font);

                            var rec = new RectangleF(bounds.Location, new Size(size.Width, bounds.Height));
                            TextRenderer.DrawText(graphics, defaultIOName, style.Font, Rectangle.Round(rec), Color.DarkGreen, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                        }
                    }
                }
                else if (columnIndex == 3)
                {
                    var variableName = ioData.VariableName;
                    if (variableName == null)
                    {
                        return;
                    }

                    string prefix = string.Empty;
                    if (config.MemoryType == "DB")
                    {
                        prefix = ioData.GetMemoryArea() == SimaticML.SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB;
                    }
                    else if (config.MemoryType == "Merker")
                    {
                        prefix = ioData.GetMemoryArea() == SimaticML.SimaticMemoryArea.INPUT ? config.PrefixInputMerker : config.PrefixOutputMerker;
                    }
                    prefix = placeholders.Parse(prefix);

                    var prefixSize = TextRenderer.MeasureText(prefix, style.Font);
                    var ioNameSize = TextRenderer.MeasureText(variableName, style.Font);

                    var rec = new RectangleF(bounds.Location, new Size(prefixSize.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, prefix, style.Font, Rectangle.Round(rec), Color.DarkGreen, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

                    rec = new RectangleF(new PointF(rec.Location.X + rec.Width, rec.Location.Y), new SizeF(ioNameSize.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, variableName, style.Font, Rectangle.Round(rec), style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + '\n' + ex.StackTrace);
            }


        }
    }
}
