using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;

namespace FCFBConverter.Utility
{
    [XmlRoot]
    [XmlType]
    public class Settings
    {
        public MoveStaticVariables StaticVariables { get; set; }
        public bool RemoveIDBs { get; set; }
        public bool RemoveReturnValue { get; set; }
        public bool OpenBlocksInEditor { get; set; }

        private static readonly string SettingsFilePath;

        static Settings()
        {
            var settingsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TIA Add-Ins", Assembly.GetCallingAssembly().GetName().Name, Assembly.GetCallingAssembly().GetName().Version.ToString());
            var settingsDirectory = Directory.CreateDirectory(settingsDirectoryPath);
            SettingsFilePath = Path.Combine(settingsDirectory.FullName, string.Concat(typeof(Settings).Name, ".xml"));
        }

        public Settings()
        {
            StaticVariables = MoveStaticVariables.Remove;
            RemoveIDBs = false;
            RemoveReturnValue = false;
            OpenBlocksInEditor = false;
        }
        
        public static Settings Load()
        {
            if (File.Exists(SettingsFilePath) == false)
            {
                return new Settings();
            }

            try
            {
                using (FileStream readStream = new FileStream(SettingsFilePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    return serializer.Deserialize(readStream) as Settings;
                }
            }
            catch
            {
                return new Settings();
            }
            
        }

        private void Save()
        {
            try
            {
                using (FileStream writeStream = new FileStream(SettingsFilePath, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writeStream, this);
                }
            }
            catch
            {
                // Ignore file operation. I know that changed settings will be lost
            }
        }
        
        internal void StaticToInOut(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            StaticVariables = MoveStaticVariables.InOut;
            Save();
        }

        internal MenuStatus StaticToInOutStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, RadioButtonActionItemStyle style)
        {
            style.State = StaticVariables == MoveStaticVariables.InOut ? RadioButtonState.Selected : RadioButtonState.Unselected;
            return MenuStatus.Enabled;
        }

        internal void StaticToTemp(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            StaticVariables = MoveStaticVariables.Temp;
            Save();
        }

        internal MenuStatus StaticToTempStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, RadioButtonActionItemStyle style)
        {
            style.State = StaticVariables == MoveStaticVariables.Temp ? RadioButtonState.Selected : RadioButtonState.Unselected;
            return MenuStatus.Enabled;
        }

        internal void RemoveStatic(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            StaticVariables = MoveStaticVariables.Remove;
            Save();
        }

        internal MenuStatus RemoveStaticStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, RadioButtonActionItemStyle style)
        {
            style.State = StaticVariables == MoveStaticVariables.Remove ? RadioButtonState.Selected : RadioButtonState.Unselected;
            return MenuStatus.Enabled;
        }

        internal void RemoveIDB(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            RemoveIDBs = !RemoveIDBs;
            Save();
        }

        internal MenuStatus RemoveIDBStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, CheckBoxActionItemStyle style)
        {
            style.State = RemoveIDBs ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return MenuStatus.Enabled;
        }

        internal void ReturnToOutput(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            RemoveReturnValue = false;
            Save();
        }

        internal MenuStatus ReturnToOutputStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, RadioButtonActionItemStyle style)
        {
            style.State = RemoveReturnValue ? RadioButtonState.Unselected : RadioButtonState.Selected;
            return MenuStatus.Enabled;
        }

        internal void RemoveReturn(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            RemoveReturnValue = true;
            Save();
        }

        internal MenuStatus RemoveReturnStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, RadioButtonActionItemStyle style)
        {
            style.State = RemoveReturnValue ? RadioButtonState.Selected : RadioButtonState.Unselected;
            return MenuStatus.Enabled;
        }
        
        internal void OpenInEditor(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider)
        {
            OpenBlocksInEditor = !OpenBlocksInEditor;
            Save();
        }

        internal MenuStatus OpenInEditorStatus(MenuSelectionProvider<IEngineeringObject> menuSelectionProvider, CheckBoxActionItemStyle style)
        {
            style.State = OpenBlocksInEditor ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            return MenuStatus.Enabled;
        }
    }
}