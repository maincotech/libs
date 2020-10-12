using System;
using System.Globalization;

namespace Maincotech.Localization
{
    public interface ILanguageService
    {
        CultureInfo CurrentCulture { get; }

        Resources Resources { get; }

        event EventHandler<CultureInfo> LanguageChanged;

        void SetLanguage(CultureInfo culture);
    }
}
