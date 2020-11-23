using System;
using System.Globalization;

namespace Maincotech.Localization
{
    public interface ILocalizer
    {
        /// <summary>
        /// Specify if the localizer is the translation mode. If true, the localizer will return key.
        /// </summary>
        bool IsInTranslationMode { get; set; }

        /// <summary>
        /// return the localized string
        /// </summary>
        /// <param name="key">The key of the string resource.</param>
        /// <returns>return the localized string</returns>
        string this[string key] { get; }

        bool ContainsKey(string key);

        CultureInfo CurrentCulture { get; }

        void SetLanguage(CultureInfo culture);

        void AddAdditionalLocalizer(ILocalizer localizer);

        event EventHandler<CultureInfo> LanguageChanged;

    }
}