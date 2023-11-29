using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using System.Windows.Forms;

namespace FCFBConverter.Utility
{
    public static class Util
    {
        public static bool ExportBlock(PlcBlock block, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                block.Export(new FileInfo(filePath), ExportOptions.None);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception during export:" + Environment.NewLine + ex);
                return false;
            }
            return true;
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
                if (blocks[i] is InstanceDB && ((InstanceDB) blocks[i]).InstanceOfName == blockName)
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
    }
}