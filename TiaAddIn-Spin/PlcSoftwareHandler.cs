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
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.HW.Features;
using System.Collections.Generic;
using Siemens.Engineering.SW.WatchAndForceTables;
using Siemens.Engineering.SW.TechnologicalObjects;

namespace SpinAddIn
{
    public class PlcSoftwareHandler : SpinAddinMenuRegistrationService
    {
        private readonly GenericImportExportHandler<PlcBlock, PlcBlockGroup> blockHandler;
        private readonly GenericImportExportHandler<PlcTagTable, PlcTagTableGroup> tagTableHandler;
        private readonly GenericImportExportHandler<PlcType, PlcTypeGroup> udtHandler;
        private readonly GenericImportExportHandler<PlcWatchTable, PlcWatchAndForceTableGroup> watchTableHandler;
        public PlcSoftwareHandler(
            GenericImportExportHandler<PlcBlock, PlcBlockGroup> blockHandler,
            GenericImportExportHandler<PlcTagTable, PlcTagTableGroup> tagTableHandler,
            GenericImportExportHandler<PlcType, PlcTypeGroup> udtHandler,
            GenericImportExportHandler<PlcWatchTable, PlcWatchAndForceTableGroup> watchTableHandler)
        {
            this.blockHandler = blockHandler;
            this.tagTableHandler = tagTableHandler;
            this.udtHandler = udtHandler;
            this.watchTableHandler = watchTableHandler;
        }

        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<DeviceItem>("Importa", ImportPlcSoftwareAll);

            menuRoot.Items.AddActionItem<DeviceItem>("Esporta", ExportPlcSoftwareAll);
            menuRoot.Items.AddActionItem<DeviceItem>("Esporta Blocchi del PLC", ExportPlcSoftwareBlocks);
            menuRoot.Items.AddActionItem<DeviceItem>("Esporta TagTables del PLC", ExportPlcSoftwareTagTables);
            menuRoot.Items.AddActionItem<DeviceItem>("Esporta UDT del PLC", ExportPlcSoftwareUDTs);
            menuRoot.Items.AddActionItem<DeviceItem>("Esporta WatchTables del PLC", ExportPlcSoftwareWatchTables);
        }

        private List<PlcSoftware> GetPlcSoftwareCollection(MenuSelectionProvider selectionProvider)
        {
            var list = new List<PlcSoftware>();
            foreach (DeviceItem deviceItem in selectionProvider.GetSelection())
            {
                var software = deviceItem.GetService<SoftwareContainer>()?.Software;
                if (!(software is PlcSoftware))
                {
                    Util.ShowMessage("Invalid object for export", "Invalid Object");
                    break;
                }

                list.Add((PlcSoftware) software);
            }
            return list;
        }

        private void ImportPlcSoftwareAll(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                blockHandler.ImportObjectsFromFolder(plcSoftware.BlockGroup, mainDirectory, true);
                tagTableHandler.ImportObjectsFromFolder(plcSoftware.TagTableGroup, mainDirectory, true);
                udtHandler.ImportObjectsFromFolder(plcSoftware.TypeGroup, mainDirectory, true);
                watchTableHandler.ImportObjectsFromFolder(plcSoftware.WatchAndForceTableGroup, mainDirectory, true);
            }
        }

        private void ExportPlcSoftwareAll(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                var softwareDirectory = Directory.CreateDirectory(mainDirectory + "\\" + plcSoftware.Name).FullName;
                blockHandler.ExportGroup(softwareDirectory, plcSoftware.BlockGroup, true);
                tagTableHandler.ExportGroup(softwareDirectory, plcSoftware.TagTableGroup, true);
                udtHandler.ExportGroup(softwareDirectory, plcSoftware.TypeGroup, true);
                watchTableHandler.ExportGroup(softwareDirectory, plcSoftware.WatchAndForceTableGroup, true);
            }
        }

        private void ExportPlcSoftwareBlocks(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                blockHandler.ExportGroup(mainDirectory, plcSoftware.BlockGroup, true);
            }
        }

        private void ExportPlcSoftwareTagTables(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                tagTableHandler.ExportGroup(mainDirectory, plcSoftware.TagTableGroup, true);
            }
        }

        private void ExportPlcSoftwareUDTs(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                udtHandler.ExportGroup(mainDirectory, plcSoftware.TypeGroup, true);
            }
        }

        private void ExportPlcSoftwareWatchTables(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (var plcSoftware in this.GetPlcSoftwareCollection(selectionProvider))
            {
                watchTableHandler.ExportGroup(mainDirectory, plcSoftware.WatchAndForceTableGroup, true);
            }
        }
    }
}
