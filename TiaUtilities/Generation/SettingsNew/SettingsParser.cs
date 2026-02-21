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
using TiaUtilities.Generation.SettingsNew.Bindings;
using TiaUtilities.Languages;
using TiaUtilities.Utility.Extensions;

namespace TiaUtilities.Generation.SettingsNew
{
    public static class SettingsParser
    {
        private static readonly TestConfiguration testConfiguration = new();
        public static SettingsForm CreateTestForm()
        {
            SettingsBindings settingsBindings = new SettingsBindings()
                .MacroSection(() => "Macro1", () => testConfiguration)

                .Section("Section1")
                .AddUInt(nameof(TestConfiguration.DBNumber), Locale.GENERICS_NAME, "Name again!?")

                .Section("DB", "Very very long Db Description! Will it work?")
                .AddString(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "Very very long Db Description! Will it work? Very very long Db Description! Will it work? Very very long Db Description! Will it work? Mega Description1")
                .AddUInt(nameof(TestConfiguration.DBNumber), Locale.GENERICS_NAME, "Mega Description2")

                .Section("AnotherCoolSectionWithACoolName", "And what if it has also a very very very long description? That would be cool!")
                .AddColor(nameof(TestConfiguration.TestColor), "Color", "Mega Description3")
                .AddColor(nameof(TestConfiguration.TestColor), "Color2", "SameOld same old?")
                .AddString(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "DB NAME AGAIN!!")

                .Section("Object", "Object??")
                .AddString(nameof(TestConfiguration.ObjName), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.IONAME}")
                .AddUInt(nameof(TestConfiguration.ObjNumber), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.CONFIG_DB_NUMBER}")
                .AddBool(nameof(TestConfiguration.TestBoolean), "Boolean", $"Cool Descr")

                .Section("JSON!", "JSON!")
                .AddJSON(nameof(TestConfiguration.TestJson), string.Empty, $"{GenPlaceholders.IO.IONAME}")
                .AddEnum(nameof(TestConfiguration.TestEnum), "Enum Fico", $"{GenPlaceholders.IO.IONAME}")

                .Section("JAVASCRIPT!", "JAVASCRIPT??!!")
                .AddJavascript(nameof(TestConfiguration.TestJavascript), string.Empty, $"{GenPlaceholders.IO.IONAME}")

                .MacroSection(() => "Macro2", () => testConfiguration)
                .Section("Section1")
                .AddUInt(nameof(TestConfiguration.DBNumber), Locale.GENERICS_NAME, "Name again!?")

                .Section("DB", "Very very long Db Description! Will it work?")
                .AddString(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "Mega Description1")
                .AddUInt(nameof(TestConfiguration.DBNumber), Locale.GENERICS_NAME, "Mega Description2")

                .Section("AnotherCoolSectionWithACoolName", "And what if it has also a very very very long description? That would be cool!")
                .AddColor(nameof(TestConfiguration.TestColor), "Color", "Mega Description3")
                .AddColor(nameof(TestConfiguration.TestColor), "Color2", "SameOld same old?")
                .AddString(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "DB NAME AGAIN!!")

                .Section("AnotherCoolSectionWithACoolName", "And what if it has also a very very very long description? That would be cool!")
                .AddColor(nameof(TestConfiguration.TestColor), "Color", "Mega Description3")
                .AddColor(nameof(TestConfiguration.TestColor), "Color2", "SameOld same old?")
                .AddString(nameof(TestConfiguration.DBName), Locale.GENERICS_NAME, "DB NAME AGAIN!!")

                .Section("Object", "Object??")
                .AddString(nameof(TestConfiguration.ObjName), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.IONAME}")
                .AddUInt(nameof(TestConfiguration.ObjNumber), Locale.GENERICS_NAME, $"{GenPlaceholders.IO.CONFIG_DB_NUMBER}")
                .AddBool(nameof(TestConfiguration.TestBoolean), "Boolean", $"Cool Descr");

            return new SettingsForm(settingsBindings);
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
