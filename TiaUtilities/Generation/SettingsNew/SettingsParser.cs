using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TiaUtilities.Configuration;
using TiaUtilities.Generation.IO;
using TiaUtilities.Generation.Placeholders;
using TiaUtilities.Languages;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.SettingsNew
{
    public static class SettingsParser
    {
        private static readonly TestConfiguration testConfiguration = new();
        public static SettingsForm? CreateTestForm()
        {
            SettingsBindings settingsBindings = new SettingsBindings(testConfiguration)
                .AddSection("Section1")
                .AddGroup("DB")
                .AddValue(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "Mega Description1", SettingsEditorTypeEnum.STRING)
                .AddValue(nameof(TestConfiguration.DBNumber), Locale.GENERICS_NAME, "Mega Description2", SettingsEditorTypeEnum.UINT)
                .AddValue(nameof(TestConfiguration.TestColor), "Color", "Mega Description3", SettingsEditorTypeEnum.COLOR)
                .AddSection("Section2")
                .AddGroup("Object")
                .AddValue(nameof(TestConfiguration.ObjName), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.IONAME}", SettingsEditorTypeEnum.STRING)
                .AddValue(nameof(TestConfiguration.ObjNumber), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.CONFIG_DB_NUMBER}", SettingsEditorTypeEnum.UINT)
                .AddValue(nameof(TestConfiguration.TestBoolean), "Boolean", $"Cool Descr", SettingsEditorTypeEnum.BOOLEAN)
                .AddSection("SectionJson")
                .AddGroup("JSON!")
                .AddValue(nameof(TestConfiguration.TestJson), string.Empty, $"{GenPlaceholders.IO.IONAME}", SettingsEditorTypeEnum.JSON)
                .AddValue(nameof(TestConfiguration.TestEnum), "Enum Fico", $"{GenPlaceholders.IO.IONAME}", SettingsEditorTypeEnum.ENUM)
                .AddSection("SectionJavascript")
                .AddGroup("JAVASCRIPT!")
                .AddValue(nameof(TestConfiguration.TestJavascript), string.Empty, $"{GenPlaceholders.IO.IONAME}", SettingsEditorTypeEnum.JAVASCRIPT);

            var form = new SettingsForm();
            form.ParseBindings(settingsBindings);

            return form;
        }
    }

    public class TestConfiguration : ObservableConfiguration
    {
        [JsonProperty] public string DBName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint DBNumber { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public string ObjName { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public uint ObjNumber { get => this.GetAs<uint>(); set => this.Set(value); }
        [JsonProperty] public Color TestColor { get => this.GetAs<Color>(); set => this.Set(value); }
        [JsonProperty] public bool TestBoolean { get => this.GetAs<bool>(); set => this.Set(value); }
        [JsonProperty] public string TestJson { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public string TestJavascript { get => this.GetAs<string>(); set => this.Set(value); }
        [JsonProperty] public SettingsEditorTypeEnum TestEnum { get => this.GetAs<SettingsEditorTypeEnum>(); set => this.Set(value); }

        public TestConfiguration()
        {
            this.DBName = "TestIO_DB";
            this.DBNumber = 196;

            this.ObjName = "CoolObject";
            this.ObjNumber = 69;

            this.TestColor = Color.YellowGreen;
            this.TestBoolean = true;

            this.TestJson = "{}";
            this.TestJavascript = "";

            this.TestEnum = SettingsEditorTypeEnum.COLOR;
        }
    }
}
