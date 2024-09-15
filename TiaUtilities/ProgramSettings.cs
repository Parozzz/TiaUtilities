using Newtonsoft.Json;
using TiaUtilities;
using TiaXmlReader.Generation;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Languages;
using TiaXmlReader.Utility;

namespace TiaXmlReader
{
    public class ProgramSettings
    {
        [JsonProperty] public string lastDBDuplicationFileName = "";

        [JsonProperty] public string DBDuplicationNewMemberName = "{replacement1}{replacement2}{replacement3}";
        [JsonProperty] public bool DBDuplicationReplaceDBName;
        [JsonProperty] public uint DBDuplicationStartingNum = 1000;
        [JsonProperty] public string DBDuplicationNewDBName = "{replacement1}{replacement2}{replacement3}";
        [JsonProperty] public string DBDuplicationReplacementList1;
        [JsonProperty] public string DBDuplicationReplacementList2;
        [JsonProperty] public string DBDuplicationReplacementList3;

        [JsonProperty] public int TimedSaveTime = 2; //Seconds
        [JsonProperty] public uint lastTIAVersion = Constants.VERSION;
        [JsonProperty] public string ietfLanguage = LocaleVariables.LANG;

        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + $@"/settings/ProgramSettings.{Constants.SAVE_FILE_EXTENSION}";
        }

        public void Save()
        {
            SavesLoader.CreateFileAndSave(this, ProgramSettings.GetFilePath(), Constants.SAVE_FILE_EXTENSION); //To create file if not exist!
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
