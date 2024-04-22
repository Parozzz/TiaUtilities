using System;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.IO;
using static TiaXmlReader.GenerationForms.GridHandler.GridCellPaintHandler;
using TiaXmlReader.GenerationForms.GridHandler;
using TiaXmlReader.SimaticML.Enums;
using TiaXmlReader.Utility;

namespace TiaXmlReader.GenerationForms.IO
{
    public class GridCellPreview<C, T> : IGridCellPainter where C : IGenerationConfiguration where T : IGridData<C>
    {
        private readonly GridDataSource<C, T> dataSource;
        private readonly C config;
        private readonly GridSettings settings;

        private readonly GenerationPlaceholders placeholders;

        public GridCellPreview(GridDataSource<C, T> dataSource, C config, GridSettings settings)
        {
            this.dataSource = dataSource;
            this.config = config;
            this.settings = settings;

            this.placeholders = new GenerationPlaceholders();
        }

        public PaintRequest PaintCellRequest(DataGridViewCellPaintingEventArgs args)
        {
            var paintRequest = new PaintRequest();

            try
            {
                var columnIndex = args.ColumnIndex;
                var rowIndex = args.RowIndex;

                if (rowIndex < 0 || rowIndex >= dataSource.Count)
                {
                    return paintRequest;
                }

                var gridData = dataSource[rowIndex];

                var preview = gridData.GetPreview(columnIndex, this.config);
                if (preview == null)
                {
                    return paintRequest;
                }

                paintRequest.dataPreview = preview;
                paintRequest.data = gridData;
                return paintRequest.Content();
            }
            catch(Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
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

            var bounds = args.CellBounds;
            var graphics = args.Graphics;
            var style = args.CellStyle;

            try
            {
                //var ioData = ((IOData)request.data).Clone(); //I do not want to operate on the IOData that is been used in the table, otherwise things changes!
                var gridData = (T)request.data;
                var previewData = request.dataPreview;

                //ioData.LoadDefaults(config);

                placeholders.SetGridData(gridData, this.config);

                var isValueDefault = string.IsNullOrEmpty(previewData.Value);
                var value = placeholders.Parse(isValueDefault ? previewData.DefaultValue : previewData.Value);

                RectangleF rec = new RectangleF();

                var hasPrefix = !string.IsNullOrEmpty(previewData.Prefix);
                if (hasPrefix)
                {
                    var parsedPrefix = placeholders.Parse(previewData.Prefix);
                    var prefixMeasuredText = TextRenderer.MeasureText(parsedPrefix, style.Font);

                    rec = new RectangleF(bounds.Location, new Size(prefixMeasuredText.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, parsedPrefix, style.Font, Rectangle.Round(rec), settings.PreviewColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);
                }

                var valueMeasuredText = TextRenderer.MeasureText(value, style.Font);
                rec = hasPrefix
                    ? new RectangleF(new PointF(rec.Location.X + rec.Width - 3, rec.Location.Y), new SizeF(valueMeasuredText.Width, bounds.Height))
                    : new RectangleF(bounds.Location, new Size(valueMeasuredText.Width, bounds.Height));

                var color = isValueDefault ? settings.PreviewColor : style.ForeColor;
                TextRenderer.DrawText(graphics, value, style.Font, Rectangle.Round(rec), color, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                /*
                if (columnIndex == IOData.IO_NAME)
                {
                    var isIONameDefault = (ioData.IOName == config.DefaultIoName);
                    var parsedIOName = placeholders.Parse(ioData.IOName);

                    var size = TextRenderer.MeasureText(parsedIOName, style.Font);

                    var rec = new RectangleF(bounds.Location, new Size(size.Width, bounds.Height));

                    var color = isIONameDefault ? preferences.PreviewColor : style.ForeColor;
                    TextRenderer.DrawText(graphics, parsedIOName, style.Font, Rectangle.Round(rec), color, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                }
                else if (columnIndex == IOData.VARIABLE)
                {
                    string parsedPrefix = string.Empty;
                    if (config.MemoryType == IOMemoryTypeEnum.DB)
                    {
                        parsedPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB;
                    }
                    else if (config.MemoryType == IOMemoryTypeEnum.MERKER)
                    {
                        parsedPrefix = ioData.GetMemoryArea() == SimaticMemoryArea.INPUT? config.PrefixInputMerker : config.PrefixOutputMerker;
                    }
                    parsedPrefix = placeholders.Parse(parsedPrefix);

                    var prefixTextSize = TextRenderer.MeasureText(parsedPrefix, style.Font);

                    var rec = new RectangleF(bounds.Location, new Size(prefixTextSize.Width, bounds.Height));
                    TextRenderer.DrawText(graphics, parsedPrefix, style.Font, Rectangle.Round(rec), preferences.PreviewColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);

                    bool isVariableDefault = (ioData.Variable == config.DefaultVariableName);
                    var parsedVariableName = placeholders.Parse(ioData.Variable);

                    var variableNameTextSize = TextRenderer.MeasureText(parsedVariableName, style.Font);
                    rec = new RectangleF(new PointF(rec.Location.X + rec.Width - 3, rec.Location.Y), new SizeF(variableNameTextSize.Width, bounds.Height));

                    var color = isVariableDefault ? preferences.PreviewColor : style.ForeColor;
                    TextRenderer.DrawText(graphics, parsedVariableName, style.Font, Rectangle.Round(rec), color, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl);
                }*/
            }
            catch (Exception ex)
            {
                Utils.ShowExceptionMessage(ex);
            }


        }
    }
}
