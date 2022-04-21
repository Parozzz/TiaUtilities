using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using System.Windows.Forms;
using Siemens.Engineering.SW.Tags;

namespace SpinAddin.Utility
{
    public static class Util
    {
        public static Form CreateForm()
        {
            Form form = new Form { Opacity = 0, ShowIcon = false };
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }

        public static void RemoveIDBs(FC fc)
        {
            IEngineeringObject parent = fc;
            while (!(parent is PlcBlockSystemGroup))
            {
                parent = parent.Parent;
            }
            var blockSystemGroup = parent as PlcBlockSystemGroup;

            RemoveIDBs(blockSystemGroup.Blocks, fc.Name);
            foreach (PlcBlockUserGroup group in blockSystemGroup.Groups)
            {
                RemoveIDBs(group, fc.Name);
            }
        }

        private static void RemoveIDBs(PlcBlockComposition blocks, string blockName)
        {
            for (var i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] is InstanceDB dB && dB.InstanceOfName == blockName)
                {
                    try
                    {
                        blocks[i].Delete();
                    }
                    catch
                    {
                        // Ignore failed deletion
                    }
                    i--;
                }
            }
        }

        private static void RemoveIDBs(PlcBlockGroup group, string blockName)
        {
            RemoveIDBs(group.Blocks, blockName);
            foreach (PlcBlockUserGroup blockGroup in group.Groups)
            {
                RemoveIDBs(blockGroup, blockName);
            }
        }

        public static Form GetForegroundWindow()
        {
            // Workaround for Add-In Windows to be shown in foreground of TIA Portal
            var form = new Form { Opacity = 0, ShowIcon = false };
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }

        public static string DesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static void ShowExceptionMessage(Exception ex)
        {
            string message = "Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace;
            string caption = "An exception occoured while executing Spin Addin!";

            MessageBox.Show(Util.CreateForm(), message, caption);
        }
    }

    public static class ExportUtil
    {
        public delegate void ExportDelegate(FileInfo fileInfo, ExportOptions options);

        public static bool Export(ExportDelegate exportDelegate, string filePath)
        {
            return Export(exportDelegate, filePath, ExportOptions.None);
        }

        public static bool Export(ExportDelegate exportDelegate, string filePath, ExportOptions options)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                exportDelegate(new FileInfo(filePath), options);
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
                return false;
            }
            return true;
        }
    }
}