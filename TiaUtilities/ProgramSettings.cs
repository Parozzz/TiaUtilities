using Newtonsoft.Json;
using TiaUtilities;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.Alarms;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.IO.Module.ExcelImporter;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.Alarms;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaXmlReader
{
    public class ProgramSettings : ObservableConfiguration
    {
        [JsonProperty] public string LastDBDuplicationFileName { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public string DBDuplicationNewMemberName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public bool DBDuplicationReplaceDBName { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty] public uint DBDuplicationStartingNum { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string DBDuplicationNewDBName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DBDuplicationReplacementList1 { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DBDuplicationReplacementList2 { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string DBDuplicationReplacementList3 { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public int AutoSaveTime { get => this.GetAs<int>(); set => this.Set(value); }
        [JsonProperty] public uint TIAVersion { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string IetfLanguage { get => this.GetAs<string>(); set => this.Set(value); }

        [JsonProperty] public GridSettings GridSettings { get => this.GetAs<GridSettings>(); set => this.Set(value); }

        [JsonProperty] public IOMainConfiguration PresetIOMainConfiguration { get => this.GetAs<IOMainConfiguration>(); set => this.Set(value); }
        [JsonProperty] public IOTabConfiguration PresetIOTabConfiguration { get => this.GetAs<IOTabConfiguration>(); set => this.Set(value); }
        [JsonProperty] public IOExcelImportConfiguration PresetIOExcelImportConfiguration { get => this.GetAs<IOExcelImportConfiguration>(); set => this.Set(value); }

        [JsonProperty] public AlarmMainConfiguration PresetAlarmMainConfiguration { get => this.GetAs<AlarmMainConfiguration>(); set => this.Set(value); }
        [JsonProperty] public AlarmTabConfiguration PresetAlarmTabConfiguration { get => this.GetAs<AlarmTabConfiguration>(); set => this.Set(value); }
        [JsonProperty] public AlarmTemplateConfiguration PresetTemplateConfiguration { get => this.GetAs<AlarmTemplateConfiguration>(); set => this.Set(value); }

        public ProgramSettings()
        {
            this.LastDBDuplicationFileName = "";

            this.DBDuplicationNewMemberName = "{replacement1}{replacement2}{replacement3}";
            this.DBDuplicationReplaceDBName = false;
            this.DBDuplicationStartingNum = 1000;
            this.DBDuplicationNewDBName = "{replacement1}{replacement2}{replacement3}";
            this.DBDuplicationReplacementList1 = "";
            this.DBDuplicationReplacementList2 = "";
            this.DBDuplicationReplacementList3 = "";

            this.AutoSaveTime = 60; //Seconds
            this.TIAVersion = Constants.VERSION;
            this.IetfLanguage = LocaleVariables.LANG;

            this.GridSettings = new();

            this.PresetIOMainConfiguration = new();
            this.PresetIOTabConfiguration = new();
            this.PresetIOExcelImportConfiguration = new();

            this.PresetAlarmMainConfiguration = new();
            this.PresetAlarmTabConfiguration = new();
            this.PresetTemplateConfiguration = new();
        }

        public static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + $@"/settings/ProgramSettings.{Constants.SAVE_FILE_EXTENSION}";
        }

        public void Save()
        {
            SavesLoader.CreateFileAndSave(this, ProgramSettings.GetFilePath(), Constants.SAVE_FILE_EXTENSION); //To create file if not exist!
            this.Wash();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var equals = GenUtils.CompareJsonFieldsAndProperties(this, obj, out _);
            return equals;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
