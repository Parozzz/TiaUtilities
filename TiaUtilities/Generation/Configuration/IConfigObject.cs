namespace TiaXmlReader.Generation.Configuration
{
    public interface IConfigObject
    {
        Control? GetControl();
    }

    public interface IConfigLine : IConfigObject
    {
        bool IsLabelOnTop();
        string? GetLabelText();
        Font? GetLabelFont();

        int GetHeight();
        bool IsControlNoAdapt();
    }

    public interface IConfigGroup : IConfigObject
    {
        ConfigForm GetConfigForm();
        C Add<C>(C configObject) where C : IConfigObject;
    }
}
