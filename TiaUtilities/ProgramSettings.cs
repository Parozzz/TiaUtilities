using Newtonsoft.Json;
using TiaUtilities;
using TiaUtilities.Configuration;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
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

            this.GridSettings = new GridSettings();
        }

        public override bool IsDirty() => base.IsDirty() || this.GridSettings.IsDirty();
        public override void Wash()
        {
            base.Wash();
            this.GridSettings.Wash();
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
