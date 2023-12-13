using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpinXmlReader;
using System;
using System.IO;

namespace TiaXmlReader
{
    public class SaveData
    {
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

        [JsonProperty] public uint lastTIAVersion = Constants.VERSION;

        public static bool Exists()
        {
            return File.Exists(Directory.GetCurrentDirectory() + @"\saveData.json");
        }

        public static SaveData Load()
        {
            if (!Exists())
            {
                return new SaveData();
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var sr = new StreamReader(Directory.GetCurrentDirectory() + @"\saveData.json"))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<SaveData>(reader);
                }
            }
        }

        public void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\saveData.json"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    lastExcelFileName = lastExcelFileName.Replace("\\", "/");
                    lastXMLExportPath = lastXMLExportPath.Replace("\\", "/");

                    serializer.Serialize(writer, this);
                    writer.Flush();
                }
            }
        }
    }
}
