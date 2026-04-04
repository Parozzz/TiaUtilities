using TiaUtilities.SettingsNew.Editors;

namespace TiaUtilities.SettingsNew.Bindings
{
    public record SettingsValueListStringTag(List<string> List);
    public record SettingsValueListSignedTag(List<long> List);
    public record SettingsValueListUnsignedTag(List<ulong> List);

    public class SettingsValueBinding(SettingsSectionBinding sectionBinding,
        string propertyName, string name, string description,
        SettingsEditorTypeEnum editorType,
        Object? tag = null)
    {
        public SettingsSectionBinding SectionBinding { get; init; } = sectionBinding;
        public string PropertyName { get; init; } = propertyName;
        public string Name { get; init; } = name;
        public string Description { get; init; } = description;
        public SettingsEditorTypeEnum EditorType { get; init; } = editorType;
        public Object? Tag { get; init; } = tag;

        public bool HasPlaceholderDotMark { get; set; } = false;
    }
}
