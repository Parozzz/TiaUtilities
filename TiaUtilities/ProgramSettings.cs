using Newtonsoft.Json;
using TiaXmlReader.Generation;
using TiaXmlReader.Utility;
using TiaXmlReader.AutoSave;
using TiaXmlReader.Generation.GridHandler;
using TiaXmlReader.Languages;
using TiaUtilities.Generation.GenForms.Alarm;
using TiaUtilities.Generation.GenForms.IO;

namespace TiaXmlReader
{
    public class ProgramSettings : ISettingsAutoSave
    {
        public const string EXTENSION = "json";
        public const string FILE_NAME = "settings/ProgramSettings." + EXTENSION;

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
        [JsonProperty] public string ietfLanguage = LocalizationVariables.LANG;

        [JsonProperty] public GridSettings GridSettings { get; set; } = new GridSettings();

        public static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"/" + FILE_NAME;
        }

        public static ProgramSettings Load()
        {
            var filePath = ProgramSettings.GetFilePath();
            return GenerationUtils.Deserialize<ProgramSettings>(ref filePath, EXTENSION, showFileDialog: false) ?? new ProgramSettings();
        }

        public bool Save()
        {
            var filePath = ProgramSettings.GetFilePath();
            if(!File.Exists(filePath))
            {
                var fileStream = File.Create(filePath);
                fileStream.Close();
            }

            return GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog: false);
        }
    }
}
