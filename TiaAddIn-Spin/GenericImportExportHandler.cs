using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using SpinAddin.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SpinAddIn
{

    public class ImportData<GROUP>
    {
        public readonly GROUP group;
        public readonly FileInfo fileInfo;
        public readonly ImportOptions importOptions;
        public readonly SWImportOptions swImportOptions;
        public ImportData(GROUP group, FileInfo fileInfo, ImportOptions importOptions, SWImportOptions swImportOptions)
        {
            this.group = group;
            this.fileInfo = fileInfo;
            this.importOptions = importOptions;
            this.swImportOptions = swImportOptions;
        }
    }

    public class GenericImportExportHandler<OBJ, GROUP> : SpinAddinMenuRegistrationService where OBJ : IEngineeringObject where GROUP : IEngineeringObject
    {
        private readonly PropertyInfo objNameProperty;
        private readonly PropertyInfo objParentProperty;
        private readonly MethodInfo objExportMethod;
        private readonly PropertyInfo objIsConsistent;

        private readonly PropertyInfo groupNameProperty;
        private readonly PropertyInfo groupItemCompositionProperty;
        private readonly PropertyInfo groupCompositionProperty;

        private readonly string multipleDescriptiveName;
        private readonly Predicate<ImportData<GROUP>> importPredicate;
        
        public GenericImportExportHandler( string multipleDescriptiveName,
            string groupItemCompositionPropertyName, string groupCompositionPropertyName,
            Predicate<ImportData<GROUP>> importPredicate)
        {
            this.multipleDescriptiveName = multipleDescriptiveName;
            this.importPredicate = importPredicate;

            var objType = typeof(OBJ);
            this.objNameProperty = objType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
            this.objParentProperty = objType.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public);
            this.objExportMethod = objType.GetMethod("Export", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(FileInfo), typeof(ExportOptions) }, null);
            this.objIsConsistent = objType.GetProperty("IsConsistent", BindingFlags.Instance | BindingFlags.Public);

            var groupType = typeof(GROUP);
            this.groupNameProperty = groupType.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
            this.groupItemCompositionProperty = groupType.GetProperty(groupItemCompositionPropertyName, BindingFlags.Instance | BindingFlags.Public);
            this.groupCompositionProperty = groupType.GetProperty(groupCompositionPropertyName, BindingFlags.Instance | BindingFlags.Public);

        }

        public string GetObjName(OBJ obj)
        {
            return (string)objNameProperty.GetValue(obj);
        }

        public GROUP GetObjParent(OBJ obj)
        {
            return (GROUP)objParentProperty.GetValue(obj);
        }

        public bool IsObjConsistent(OBJ obj)
        {
            return objIsConsistent == null ? true : (bool) objIsConsistent.GetValue(obj);
        }

        public IEnumerable<GROUP> GetAllGroups(GROUP group)
        {
            return (IEnumerable<GROUP>) groupCompositionProperty.GetValue(group);
        }


        public IEnumerable<OBJ> GetAllObjInGroup(GROUP group)
        {
            return (IEnumerable<OBJ>) groupItemCompositionProperty.GetValue(group);
        }

        public string GetGroupName(GROUP group)
        {
            return (string)groupNameProperty.GetValue(group);
        }

        public void Register(ContextMenuAddInRoot menuRoot)
        {
            if (Util.IsItalian())
            {
                menuRoot.Items.AddActionItem<OBJ>($"Copia tutti i nomi dei {multipleDescriptiveName} selezionati", CopyAllSelectedNamesToClipboard);
                menuRoot.Items.AddActionItem<OBJ>($"Sostituisci tutti i nomi dei {multipleDescriptiveName} selezionati", ChangeAllSelectedNames);
                if (typeof(OBJ).IsAssignableFrom(typeof(PlcBlock)))
                {

                    menuRoot.Items.AddActionItem<OBJ>($"Modifica numeri dei {multipleDescriptiveName} sequenzialmente", ChangeBlockNumberSequentially);
                    menuRoot.Items.AddActionItem<OBJ>($"Imposta {multipleDescriptiveName} come \"Ottimizzato\"", ChangeAllBlockToOptimized);
                    menuRoot.Items.AddActionItem<OBJ>($"Imposta {multipleDescriptiveName} come \"Non Ottimizzato\"", ChangeAllBlockToStandard);
                }

                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} da file selezionati", ImportDescrete);
                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} dalle cartelle selezionata", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Importa {multipleDescriptiveName} dalla cartella selezionata (Incluse sottocartelle)", ImportAllSubFolders);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName}", ImportDescrete);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName} da cartella", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<OBJ>($"Importa {multipleDescriptiveName} da cartella (comprese sotto-cartelle)", ImportAllSubFolders);

                menuRoot.Items.AddActionItem<GROUP>($"Esporta {multipleDescriptiveName} nella cartella selezionata", ExportOnlySelectedFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Esporta {multipleDescriptiveName} nella cartella selezionata (Comprese sotto-cartelle)", ExportAllFolders);
                menuRoot.Items.AddActionItem<OBJ>($"Esporta {multipleDescriptiveName} selez. su cartella", ExportAllToFolder);
            }
            else
            {
                menuRoot.Items.AddActionItem<OBJ>($"Copy the names of all selected {multipleDescriptiveName}", CopyAllSelectedNamesToClipboard);
                menuRoot.Items.AddActionItem<OBJ>($"Replace all the names of selected {multipleDescriptiveName}", ChangeAllSelectedNames);
                if (typeof(OBJ).IsAssignableFrom(typeof(PlcBlock)))
                {
                    menuRoot.Items.AddActionItem<OBJ>($"Change number sequentially for {multipleDescriptiveName}", ChangeBlockNumberSequentially);
                    menuRoot.Items.AddActionItem<OBJ>($"Set \"Optimized block access\" to all", ChangeAllBlockToOptimized);
                    menuRoot.Items.AddActionItem<OBJ>($"Remove \"Optimized block access\" to all", ChangeAllBlockToStandard);
                }

                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected file(s)", ImportDescrete);
                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected folder(s)", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Import {multipleDescriptiveName} from selected folder (Including sub-folders)", ImportAllSubFolders);
                menuRoot.Items.AddActionItem<OBJ>($"Import {multipleDescriptiveName}", ImportDescrete);
                menuRoot.Items.AddActionItem<OBJ>($"Import all {multipleDescriptiveName} from folder", ImportOnlyTopFolder);
                menuRoot.Items.AddActionItem<OBJ>($"Import all {multipleDescriptiveName} from folder (Including sub-folder)", ImportAllSubFolders);

                menuRoot.Items.AddActionItem<GROUP>($"Export all {multipleDescriptiveName} inside selected folder", ExportOnlySelectedFolder);
                menuRoot.Items.AddActionItem<GROUP>($"Export all {multipleDescriptiveName} inside selected folder (Including sub-folders)", ExportAllFolders);
                menuRoot.Items.AddActionItem<OBJ>($"Export selected {multipleDescriptiveName} in folder", ExportAllToFolder);
            }

        }

        private void ExportOnlySelectedFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (GROUP group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, false);
            }
        }

        private void ExportAllFolders(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) != DialogResult.OK)
            {
                return;
            }

            var mainDirectory = folderDialog.SelectedPath;
            foreach (GROUP group in selectionProvider.GetSelection())
            {
                ExportGroup(mainDirectory, group, true);
            }
        }

        public void ExportGroup(string mainDirectory, GROUP group, bool withSubfolders)
        {
            var directory = mainDirectory + "\\" + this.GetGroupName(group);
            foreach (OBJ obj in this.GetAllObjInGroup(group))
            {
                var exportOK = ExportSingle(obj, directory + "\\" + this.GetObjName(obj) + ".xml");
                if (!exportOK)
                {
                    return;
                }
            }

            if (withSubfolders)
            {
                foreach (GROUP subGroup in this.GetAllGroups(group))
                {
                    ExportGroup(directory, subGroup, true);
                }
            }

        }

        private void ExportAllToFolder(MenuSelectionProvider selectionProvider)
        {
            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                var directory = folderDialog.SelectedPath;
                foreach (OBJ obj in selectionProvider.GetSelection())
                {
                    var exportOK = ExportSingle(obj, directory + "\\" + this.GetObjName(obj) + ".xml");
                    if (!exportOK)
                    {
                        return;
                    }
                }
            }
        }

        private void CopyAllSelectedNamesToClipboard(MenuSelectionProvider selectionProvider)
        {
            try
            {
                var names = new List<string>();
                foreach (OBJ obj in selectionProvider.GetSelection())
                {
                    PropertyInfo propertyInfo = null;

                    var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var property in properties)
                    {
                        if (property.Name == "Name" || property.Name == "name")
                        {
                            propertyInfo = property;
                            break;
                        }
                    }

                    if (propertyInfo == null)
                    {
                        continue;
                    }

                    var name = (string)propertyInfo.GetValue(obj);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }

                var namesText = String.Join("\r\n", names) + "\r\n";
                Clipboard.SetText(namesText, TextDataFormat.Text);
            }
            catch (Exception)
            {
            }
        }

        private void ChangeAllSelectedNames(MenuSelectionProvider selectionProvider)
        {
            string placeholder = "{actual_name}";
            try
            {
                string str = placeholder;
                var result = Util.ShowInputDialog(ref str);
                if (result == DialogResult.OK)
                {
                    foreach (OBJ obj in selectionProvider.GetSelection())
                    {
                        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            if (property.Name == "Name" || property.Name == "name")
                            {
                                var name = (string)property.GetValue(obj);
                                var newName = str.Replace("{actual_name}", name);
                                property.SetValue(obj, newName);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void ChangeBlockNumberSequentially(MenuSelectionProvider selectionProvider)
        {
            try
            {
                string str = "100";
                var result = Util.ShowInputDialog(ref str);

                if (result == DialogResult.OK)
                {
                    if (!Int32.TryParse(str, out Int32 blockNum))
                    {
                        return;
                    }

                    foreach (PlcBlock block in selectionProvider.GetSelection().Select(obj => obj is PlcBlock).Cast<PlcBlock>())
                    {
                        block.AutoNumber = false;
                        block.Number = blockNum;
                        blockNum++;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void ChangeAllBlockToOptimized(MenuSelectionProvider selectionProvider)
        {
            try
            {
                foreach (PlcBlock block in selectionProvider.GetSelection().Select(obj => obj is PlcBlock).Cast<PlcBlock>())
                {
                    block.MemoryLayout = MemoryLayout.Optimized;
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void ChangeAllBlockToStandard(MenuSelectionProvider selectionProvider)
        {
            try
            {
                foreach (PlcBlock block in selectionProvider.GetSelection().Select(obj => obj is PlcBlock).Cast<PlcBlock>())
                {
                    block.MemoryLayout = MemoryLayout.Standard;
                }
            }
            catch (Exception ex)
            {
                Util.ShowExceptionMessage(ex);
            }
        }

        private void ImportDescrete(MenuSelectionProvider selectionProvider)
        {
            var selection = selectionProvider.GetSelection();

            var selectedGroup = selection.Where(group => group is GROUP).Cast<GROUP>().SingleOrDefault();
            if (selectedGroup == null)
            {
                selectedGroup = selectionProvider.GetSelection()
                    .Where(obj => obj is OBJ)
                    .Cast<OBJ>()
                    .Select(this.GetObjParent)
                    .First();
            }

            if (selectedGroup == null)
            {
                return;
            }

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
                    if (!ImportBlock(selectedGroup, fileName))
                    {
                        return;
                    }
                }
            }

        }
        private void ImportOnlyTopFolder(MenuSelectionProvider selectionProvider)
        {
            ImportFolderGeneric(selectionProvider, false);
        }

        private void ImportAllSubFolders(MenuSelectionProvider selectionProvider)
        {
            ImportFolderGeneric(selectionProvider, true);
        }

        private void ImportFolderGeneric(MenuSelectionProvider selectionProvider, bool includeSubFolders)
        {
            GROUP group = default(GROUP);

            group = selectionProvider.GetSelection().Select(obj => obj is GROUP).Cast<GROUP>().FirstOrDefault();
            if (group == null)
            {
                var firstObj = selectionProvider.GetSelection().Select(obj => obj is OBJ).Cast<OBJ>().FirstOrDefault();
                if (firstObj == null)
                {
                    return;
                }

                group = this.GetObjParent(firstObj);
            }

            if (group == null)
            {
                return;
            }

            var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog(Util.CreateForm()) == DialogResult.OK)
            {
                this.ImportObjectsFromFolder(group, folderDialog.SelectedPath, includeSubFolders);
            }
        }

        public bool ImportObjectsFromFolder(GROUP group, string folderName, bool searchSubDirectories)
        {
            if (searchSubDirectories)
            {
                foreach (var directoryName in Directory.GetDirectories(folderName))
                {
                    if (!this.ImportObjectsFromFolder(group, directoryName, true))
                    {
                        return false;
                    }
                }
            }

            foreach (var fileName in Directory.GetFiles(folderName))
            {
                if (!ImportBlock(group, fileName))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExportSingle(OBJ obj, string filePath)
        {//return TRUE = continue other export, FALSE = block all other export.
            try
            {
                if (!this.IsObjConsistent(obj))
                {
                    string message = Util.IsItalian() ? "Per favore, compila il blocco prima di esportarlo." : "Please compile the block before exporting";
                    string caption = Util.IsItalian() ? "Blocco non consistente" : "Incosistent block";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    return true;
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                objExportMethod.Invoke(obj, new object[] { new FileInfo(filePath), ExportOptions.WithReadOnly });
                return true;
            }
            catch (Exception ex)
            {

                return Util.ShowExceptionMessage(ex, useInnerException: true); //If it return true it means that the error is ignored and will continue.
            }
        }

        private bool ImportBlock(GROUP group, string fileName)
        {
            if (fileName.ToLower().EndsWith(".xml"))
            {
                try
                {
#if TIA_V19 || TIA_V20 || TIA_V21
                    var importData = new ImportData<GROUP>(group,
                        new FileInfo(fileName),
                        ImportOptions.Override | ImportOptions.ActivateInactiveCultures,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
#else
                    var importData = new ImportData<GROUP>(group,
                        new FileInfo(fileName),
                        ImportOptions.Override,
                        SWImportOptions.IgnoreMissingReferencedObjects |
                        SWImportOptions.IgnoreStructuralChanges |
                        SWImportOptions.IgnoreUnitAttributes);
#endif

                    return importPredicate(importData);
                }
                catch (Exception ex)
                {
                    return Util.ShowExceptionMessage(ex); //Return false if the message box is cancelled
                }
            }

            return false;
        }

    }
}
