using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Units;
using SpinAddin.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace SpinAddIn
{
    public class PlcSoftwareHandler : SpinAddinMenuRegistrationService
    {
        private readonly PlcBlockHandler blockHandler;
        private readonly PlcTagTableHandler tagTableHandler;
        private readonly PLCUDTHandler udtHandler;
        public PlcSoftwareHandler(PlcBlockHandler blockHandler, PlcTagTableHandler tagTableHandler, PLCUDTHandler udtHandler)
        {
            this.blockHandler = blockHandler;
            this.tagTableHandler = tagTableHandler;
            this.udtHandler = udtHandler;
        }

        public void Register(ContextMenuAddInRoot menuRoot)
        {
            //Siemens.Engineering.HW.Device
            menuRoot.Items.AddActionItem<Device>("Esporta blocchi del progetto", ExportProjectBlocks);
            menuRoot.Items.AddActionItem<Device>("Esporta TagTables del progetto", ExportProjectTagTables);
            menuRoot.Items.AddActionItem<Device>("Esporta UDT del progetto", ExportProjectUDTs);
        }

        private void ExportProjectBlocks(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcSoftware software in selectionProvider.GetSelection())
            {
                blockHandler.ExportGroup(mainDirectory, software.BlockGroup, true);
            }
        }

        private void ExportProjectTagTables(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcSoftware software in selectionProvider.GetSelection())
            {
                tagTableHandler.ExportGroup(mainDirectory, software.TagTableGroup, true);
            }
        }

        private void ExportProjectUDTs(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (PlcSoftware software in selectionProvider.GetSelection())
            {
                udtHandler.ExportGroup(mainDirectory, software.TypeGroup, true);
            }
        }

    }
}
