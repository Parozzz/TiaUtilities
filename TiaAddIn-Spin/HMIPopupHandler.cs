﻿
/* Modifica senza merge dal progetto 'AddIn_V16'
Prima:
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Collections.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
*/

/* Modifica senza merge dal progetto 'AddIn_V17'
Prima:
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Collections.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
*/

/* Modifica senza merge dal progetto 'AddIn_V18'
Prima:
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Collections.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
*/

/* Modifica senza merge dal progetto 'AddIn_V19'
Prima:
using Siemens.Engineering.AddIn.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.Hmi.Screen;
using Siemens.Collections.SW;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
*/
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
/* Modifica senza merge dal progetto 'AddIn_V16'
Prima:
using System.Windows.Forms;
using SpinAddin.Utility;
using Siemens.Engineering;
Dopo:
using System.Linq;
using System.Text;
*/

/* Modifica senza merge dal progetto 'AddIn_V17'
Prima:
using System.Windows.Forms;
using SpinAddin.Utility;
using Siemens.Engineering;
Dopo:
using System.Linq;
using System.Text;
*/

/* Modifica senza merge dal progetto 'AddIn_V18'
Prima:
using System.Windows.Forms;
using SpinAddin.Utility;
using Siemens.Engineering;
Dopo:
using System.Linq;
using System.Text;
*/

/* Modifica senza merge dal progetto 'AddIn_V19'
Prima:
using System.Windows.Forms;
using SpinAddin.Utility;
using Siemens.Engineering;
Dopo:
using System.Linq;
using System.Text;
*/
using SpinAddin.Utility;
using System;

/* Modifica senza merge dal progetto 'AddIn_V16'
Prima:
using System.Threading.Tasks;
Dopo:
using System.IO;
using System.Threading.Tasks;
*/

/* Modifica senza merge dal progetto 'AddIn_V17'
Prima:
using System.Threading.Tasks;
Dopo:
using System.IO;
using System.Threading.Tasks;
*/

/* Modifica senza merge dal progetto 'AddIn_V18'
Prima:
using System.Threading.Tasks;
Dopo:
using System.IO;
using System.Threading.Tasks;
*/

/* Modifica senza merge dal progetto 'AddIn_V19'
Prima:
using System.Threading.Tasks;
Dopo:
using System.IO;
using System.Threading.Tasks;
*/
using System.IO;
using System.Windows.Forms;

namespace SpinAddIn
{
    public class HMIPopupHandler : SpinAddinMenuRegistrationService
    {
        public void Register(ContextMenuAddInRoot menuRoot)
        {
            menuRoot.Items.AddActionItem<ScreenPopup>("Esporta Popup Singolarmente", ExportDescrete);
            menuRoot.Items.AddActionItem<ScreenPopup>("Esporta Popup selez. su Cartella", ExportAllToFolder);
            menuRoot.Items.AddActionItem<ScreenPopup>("Importa Popup", ImportDescrete);
            menuRoot.Items.AddActionItem<ScreenPopup>("Importa Cartella", ImportAllFolder);
            menuRoot.Items.AddActionItem<ScreenPopup>("Importa Cartella (comprese sottocartelle)", ImportAllSubFolders);
        }

        private void ExportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (ScreenPopup hmiPopup in selectionProvider.GetSelection())
            {
                var fileDialog = new OpenFileDialog()
                {
                    Filter = "xml files (*.xml)|*.xml",
                    CheckFileExists = false,
                    CheckPathExists = false,
                    Multiselect = false,
                    Title = "Export HMI SCREEN " + hmiPopup.Name,
                    ShowHelp = true,
                    FileName = hmiPopup.Name
                };
                if (fileDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    ExportUtil.Export(hmiPopup.Export, fileDialog.FileName);
                }
            }
        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (ScreenPopup hmiPopup in selectionProvider.GetSelection())
                {
                    ExportUtil.Export(hmiPopup.Export, directory + "\\" + hmiPopup.Name + ".xml");
                }
            }
        }


        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            foreach (ScreenPopup hmiPopup in selectionProvider.GetSelection())
            {
                var hmiPopupFolder = (ScreenPopupFolder)hmiPopup.Parent;

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
                        ImportHMIPopup(hmiPopupFolder, fileName);
                    }
                }

                break;
            }
        }

        private void ImportAllFolder(MenuSelectionProvider selectionProvider)
        {
            foreach (ScreenPopup hmiPopup in selectionProvider.GetSelection())
            {
                var hmiPopupFolder = (ScreenPopupFolder)hmiPopup.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportHMIPopupFromFolder(hmiPopupFolder, folderDialog.SelectedPath, false);
                }

                break;
            }
        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            foreach (ScreenPopup hmiPopup in selectionProvider.GetSelection())
            {
                var hmiPopupFolder = (ScreenPopupFolder)hmiPopup.Parent;

                var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
                {
                    this.ImportHMIPopupFromFolder(hmiPopupFolder, folderDialog.SelectedPath, true);
                }

                break;
            }
        }

        private void ImportHMIPopupFromFolder(ScreenPopupFolder popupFolder, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    this.ImportHMIPopupFromFolder(popupFolder, directoryName, true);
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                ImportHMIPopup(popupFolder, fileName);
            }
        }

        private void ImportHMIPopup(ScreenPopupFolder popupFolder, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
                    var fileInfo = new FileInfo(fileName);
                    popupFolder.ScreenPopups.Import(fileInfo, ImportOptions.Override);
                }
                catch (Exception ex)
                {
                    Util.ShowExceptionMessage(ex);
                }
            }
        }

    }
}
