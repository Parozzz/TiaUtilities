﻿using TiaUtilities.Configuration;

namespace TiaUtilities.Generation.Configuration
{
    public interface IConfigObject
    {
        Control? GetControl();
    }

    public interface IConfigLine : IConfigObject
    {
        bool IsLabelOnTop();
        ObservableObject<string?> GetLabelText();
        Font? GetLabelFont();

        int GetHeight();
        bool IsControlNoAdapt();

        void TrasferToAllConfigurations();
    }

    public interface IConfigGroup : IConfigObject
    {
        ConfigForm GetConfigForm();
        C Add<C>(C configObject) where C : IConfigObject;
    }
}
