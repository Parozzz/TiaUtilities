using Microsoft.WindowsAPICodePack.Dialogs;
using Svg;
using System.Drawing.Drawing2D;

namespace TiaUtilities
{
    public partial class SvgTestForm : Form
    {
        public record SvgPathRecord(SvgVisualElement Element, SvgElement OriginalElement, GraphicsPath GraphicsPath);

        private SvgDocument? svgDocument;
        private readonly List<SvgPathRecord> visualElementList;

        private SvgPathRecord? selectedPathRecord;

        public SvgTestForm()
        {
            InitializeComponent();

            this.visualElementList = [];
        }

        private void PathTextBox_Click(object sender, EventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog
            {
                EnsurePathExists = true,
                EnsureFileExists = true,
                DefaultExtension = ".svg",
                Filters = { new CommonFileDialogFilter("svg Files", "*.svg") },
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok && fileDialog.FileName is string filePath)
            {
                this.pathTextBox.Text = filePath;
            }
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            if (this.pathTextBox.Text is string filePath && File.Exists(filePath))
            {
                visualElementList.Clear();

                this.svgDocument = SvgDocument.Open<SvgDocument>(filePath);
                DrawSvg();
            }
        }

        private void ProcessNodes(IEnumerable<SvgElement> nodes, SvgPaintServer colorServer, ISvgRenderer renderer)
        {
            foreach (var node in nodes)
            {
                if(node is SvgElement svgElement)
                {
                    svgElement.RenderElement(renderer);

                    if(svgElement is SvgVisualElement svgVisualElement)
                    {
                        var graphicsPath = svgVisualElement.Path(renderer);
                        visualElementList.Add(new(svgVisualElement, svgVisualElement.DeepCopy(), graphicsPath));
                    }
                }

                ProcessNodes(node.Descendants(), colorServer, renderer);
            }
        }

        private void svgPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(this.svgDocument == null)
            {
                return;
            }

            bool requireRedraw = false;

            var x = e.X;
            var y = e.Y;

            SvgPathRecord? clickedPathRecord = null;

            foreach(var record in visualElementList)
            {
                if(record.GraphicsPath.IsVisible(x, y))
                {
                    clickedPathRecord = record;
                    break;
                }
            }

            var oldPathRecord = this.selectedPathRecord;
            this.selectedPathRecord = clickedPathRecord;

            this.svgDocument.ApplyRecursive(svgElement =>
            {
                if (this.selectedPathRecord != null && svgElement == this.selectedPathRecord.Element)
                {
                    svgElement.Stroke = new SvgColourServer(Color.Yellow);
                    svgElement.StrokeWidth = 3;

                    requireRedraw = true;
                }
                else if(oldPathRecord != null && svgElement == oldPathRecord.Element)
                {
                    svgElement.Stroke = oldPathRecord.OriginalElement.Stroke;
                    svgElement.StrokeWidth = oldPathRecord.OriginalElement.StrokeWidth;
                }
            });

            if (requireRedraw)
            {
                DrawSvg();
            }
        }

        private void DrawSvg()
        {
            if(this.svgDocument == null)
            {
                return;
            }    

            var graphics = this.svgPictureBox.CreateGraphics();
            graphics.Clear(Color.White);

            // Recursively change all nodes.
            var rendered = SvgRenderer.FromGraphics(graphics);
            ProcessNodes(this.svgDocument.Descendants(), new SvgColourServer(Color.DarkGreen), rendered);
        }
    }

    public class TestSvgEventCaller : ISvgEventCaller
    {
        private Dictionary<string, Action> FDynamicRPCs = [];

        public void RegisterAction(string rpcID, Action action)
        {
            FDynamicRPCs.Add(rpcID, action);
        }

        public void RegisterAction<T1>(string rpcID, Action<T1> action)
        {

        }

        public void RegisterAction<T1, T2>(string rpcID, Action<T1, T2> action)
        {

        }

        public void RegisterAction<T1, T2, T3>(string rpcID, Action<T1, T2, T3> action)
        {

        }

        public void RegisterAction<T1, T2, T3, T4>(string rpcID, Action<T1, T2, T3, T4> action)
        {

        }

        public void RegisterAction<T1, T2, T3, T4, T5>(string rpcID, Action<T1, T2, T3, T4, T5> action)
        {

        }

        public void RegisterAction<T1, T2, T3, T4, T5, T6>(string rpcID, Action<T1, T2, T3, T4, T5, T6> action)
        {

        }

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7>(string rpcID, Action<T1, T2, T3, T4, T5, T6, T7> action)
        {

        }

        public void RegisterAction<T1, T2, T3, T4, T5, T6, T7, T8>(string rpcID, Action<T1, T2, T3, T4, T5, T6, T7, T8> action)
        {

        }

        public void UnregisterAction(string rpcID)
        {

        }
    }
}
