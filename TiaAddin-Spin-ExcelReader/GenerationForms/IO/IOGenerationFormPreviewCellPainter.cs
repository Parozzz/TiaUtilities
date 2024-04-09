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

            if (rowIndex < 0 || columnIndex < IOGenerationForm.IO_NAME_COLUMN || columnIndex > IOGenerationForm.VARIABLE_COLUMN)
            {
                return paintRequest;
            }

            var ioData = dataSource.GetDataAt(rowIndex);
            if (ioData.IsEmpty() || string.IsNullOrEmpty(ioData.Address))
            {
                return paintRequest;
            }

            paintRequest.data = ioData;
            switch (columnIndex)
            {
                case IOGenerationForm.IO_NAME_COLUMN:
                    if (!string.IsNullOrEmpty(ioData.IOName)) //I want to preview the io name if is not set yet!
                    {
                        return paintRequest;
                    }

                    return string.IsNullOrEmpty(ioData.IOName) ? paintRequest.Content() : paintRequest;
                case IOGenerationForm.VARIABLE_COLUMN:
                    if (string.IsNullOrEmpty(ioData.Variable) && string.IsNullOrEmpty(config.DefaultVariableName))
                    {
                        return paintRequest;
                    }

                    return paintRequest.Content();
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
                var ioData = ((IOData)request.data).Clone(); //I do not want to operate on the IOData that is been used in the table, otherwise things changes!
                ioData.LoadDefaults(config);

                placeholders.Clear();
                placeholders.SetIOData(ioData);

                if (columnIndex == IOGenerationForm.IO_NAME_COLUMN)
                {
                    var isIONameDefault = (ioData.IOName == config.DefaultIoName);
                    var parsedIOName = placeholders.Parse(ioData.IOName);

                    var size = TextRenderer.MeasureText(parsedIOName, style.Font);

                    var rec = new RectangleF(bounds.Location, new Size(size.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, parsedIOName, style.Font, Rectangle.Round(rec), isIONameDefault ? Color.DarkGreen : style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
                else if (columnIndex == IOGenerationForm.VARIABLE_COLUMN)
                {
                    string parsedPrefix = string.Empty;
                    if (config.MemoryType == IOMemoryTypeEnum.DB)
                    {
                        parsedPrefix = ioData.GetMemoryArea() == SimaticML.SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB;
                    }
                    else if (config.MemoryType == IOMemoryTypeEnum.MERKER)
                    {
                        parsedPrefix = ioData.GetMemoryArea() == SimaticML.SimaticMemoryArea.INPUT ? config.PrefixInputMerker : config.PrefixOutputMerker;
                    }
                    parsedPrefix = placeholders.Parse(parsedPrefix);

                    var prefixTextSize = TextRenderer.MeasureText(parsedPrefix, style.Font);

                    var rec = new RectangleF(bounds.Location, new Size(prefixTextSize.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, parsedPrefix, style.Font, Rectangle.Round(rec), Color.DarkGreen, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);

                    bool isVariableDefault = (ioData.Variable == config.DefaultVariableName);
                    var parsedVariableName = placeholders.Parse(ioData.Variable);

                    var variableNameTextSize = TextRenderer.MeasureText(parsedVariableName, style.Font);
                    rec = new RectangleF(new PointF(rec.Location.X + rec.Width - 3, rec.Location.Y), new SizeF(variableNameTextSize.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, parsedVariableName, style.Font, Rectangle.Round(rec), isVariableDefault ? Color.DarkGreen : style.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + '\n' + ex.StackTrace);
            }


        }
    }
}
