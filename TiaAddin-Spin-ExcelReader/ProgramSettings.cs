using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using TiaXmlReader.Generation.IO.GenerationForm;
using TiaXmlReader.Generation;
using TiaXmlReader.Utility;

namespace TiaXmlReader
{
    public class ProgramSettings
    {
        public const string EXTENSION = "json";
        public const string FILE_NAME = "settings/ProgramSettings." + EXTENSION;

        [JsonProperty] public string lastExcelFileName = "";
        [JsonProperty] public string lastXMLExportPath = "";
        [JsonProperty] public string lastDBDuplicationFileName = "";

        [JsonProperty] public string DBDuplicationNewMemberName = "{replacement1}{replacement2}{replacement3}";
        [JsonProperty] public bool DBDuplicationReplaceDBName;
        [JsonProperty] public uint DBDuplicationStartingNum = 1000;
        [JsonProperty] public string DBDuplicationNewDBName = "{replacement1}{replacement2}{replacement3}";
        [JsonProperty] public string DBDuplicationReplacementList1;
        [JsonProperty] public string DBDuplicationReplacementList2;
        [JsonProperty] public string DBDuplicationReplacementList3;

        [JsonProperty] public AutoSaveHandler.AutoSaveTimeEnum AutoSaveTime = AutoSaveHandler.AutoSaveTimeEnum.MIN_2;
        [JsonProperty] public uint lastTIAVersion = Constants.VERSION;
        [JsonProperty] public string ietfLanguage = SystemVariables.LANG;
        public static string GetFilePath()
        {
            return Directory.GetCurrentDirectory() + @"\" + FILE_NAME;
        }

        public static ProgramSettings Load()
        {
            var filePath = ProgramSettings.GetFilePath();
            return GenerationUtils.Load(ref filePath, EXTENSION, out ProgramSettings settings, showFileDialog: false) ? settings : new ProgramSettings();
        }

        public void Save()
        {
            var filePath = ProgramSettings.GetFilePath();
            GenerationUtils.Save(this, ref filePath, EXTENSION, showFileDialog: false);
        }
    }
}
