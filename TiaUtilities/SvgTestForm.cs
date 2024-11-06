using Microsoft.WindowsAPICodePack.Dialogs;
using Svg;
using System.Drawing.Drawing2D;

namespace TiaUtilities
{
    public partial class SvgTestForm : Form
    {
        public record SvgPathRecord(SvgPath VvgPath, GraphicsPath GraphicsPath);

        private readonly List<SvgPathRecord> pathRecordList;

        public SvgTestForm()
        {
            InitializeComponent();

            this.pathRecordList = [];
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
                var svgDoc = SvgDocument.Open<SvgDocument>(filePath);

                // Recursively change all nodes.
                ProcessNodes(svgDoc.Descendants(), new SvgColourServer(Color.DarkGreen));

                var graphics = this.svgPictureBox.CreateGraphics();
                graphics.Clear(Color.White);

                var rendered = SvgRenderer.FromGraphics(graphics);

                pathRecordList.Clear();
                this.RenderNodes(svgDoc.Descendants(), rendered);
                //svgDoc.Draw(graphics);
                //var bitmap = svgDoc.Draw();
            }
        }

        private void ProcessNodes(IEnumerable<SvgElement> nodes, SvgPaintServer colorServer)
        {
            int IDCounter = 1;
            foreach (var node in nodes)
            {

                if (node.Fill != SvgPaintServer.None) node.Fill = colorServer;
                if (node.Color != SvgPaintServer.None) node.Color = colorServer;
                //if (node.StopColor != SvgPaintServer.None) node.StopColor = colorServer;
                if (node.Stroke != SvgPaintServer.None) node.Stroke = colorServer;

                //node.ID = $"path_{IDCounter++}";

                ProcessNodes(node.Descendants(), colorServer);
            }
        }

        private void RenderNodes(IEnumerable<SvgElement> nodes, ISvgRenderer renderer)
        {
            foreach (var node in nodes)
            {
                if (node is SvgPath svgPath)
                {
                    svgPath.RenderElement(renderer);

                    var path = svgPath.Path(renderer);
                    pathRecordList.Add(new(svgPath, path));
                }

                RenderNodes(node.Descendants(), renderer);
            }
        }

        private void svgPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var x = e.X;
            var y = e.Y;

            foreach(var record in pathRecordList)
            {
                if(record.GraphicsPath.IsVisible(x, y))
                {
                    Console.WriteLine("Visible!");
                }
            }
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
