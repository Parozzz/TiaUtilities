using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Engineering.Hmi.Screen;
using System.Windows.Forms;
using SpinAddin.Utility;
using Siemens.Engineering;
using System.IO;
using Siemens.Engineering.SW;

namespace SpinAddIn
{
    internal class HMIScreenHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<Siemens.Engineering.Hmi.Screen.Screen>("Esporta Schermate Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<Siemens.Engineering.Hmi.Screen.Screen>("Esporta Schermate selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<Siemens.Engineering.Hmi.Screen.Screen>("Importa Schermate", ImportDescrete);
            menuRoot.Items.AddActionItem<Siemens.Engineering.Hmi.Screen.Screen>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<Siemens.Engineering.Hmi.Screen.Screen>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (Siemens.Engineering.Hmi.Screen.Screen hmiScreen in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export HMI SCREEN " + hmiScreen.Name,
                    ShowHelp = true,
                    FileName = hmiScreen.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(hmiScreen.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (Siemens.Engineering.Hmi.Screen.Screen hmiScreen in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(hmiScreen.Export, directory + "\\" + hmiScreen.Name + ".xml");
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (Siemens.Engineering.Hmi.Screen.Screen hmiScreen in selectionProvider.GetSelection())
            {
                var hmiScreenFolder = (ScreenFolder) hmiScreen.Parent;

                var fileDialog = new OpenFileDialog
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Multiselect = true
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    foreach (var fileName in fileDialog.FileNames)
                    {
                        ImportHMIScreen(hmiScreenFolder, fileName);
                    }
                }

                break;
            }
        }

        private void ImportAllFolder(MenuSelectionProvider selectionProvider)
        {
            foreach (Siemens.Engineering.Hmi.Screen.Screen hmiScreen in selectionProvider.GetSelection())
            {
                var hmiScreenFolder = (ScreenFolder) hmiScreen.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportHMIScreensFromFolder(hmiScreenFolder, folderDialog.SelectedPath, false);
                }

                break;
            }
        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            foreach (Siemens.Engineering.Hmi.Screen.Screen hmiScreen in selectionProvider.GetSelection())
            {
                var hmiScreenFolder = (ScreenFolder) hmiScreen.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportHMIScreensFromFolder(hmiScreenFolder, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private void ImportHMIScreensFromFolder(ScreenFolder folder, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    this.ImportHMIScreensFromFolder(folder, directoryName, true);
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                ImportHMIScreen(folder, fileName);
            }
        }

        private void ImportHMIScreen(ScreenFolder folder, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    folder.Screens.Import(fileInfo, ImportOptions.Override);
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

    }
}
