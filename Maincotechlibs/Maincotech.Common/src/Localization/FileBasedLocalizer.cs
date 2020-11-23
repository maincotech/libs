using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Maincotech.Localization
{
    public class FileBasedLocalizer : ILocalizer
    {
        private CultureInfo _currentCulture;
        private string _resourcesFolder;
        private IDictionary<string, string> _keyValues = new Dictionary<string, string>();

        private List<ILocalizer> _additionalLocalizers = new List<ILocalizer>();

        public event EventHandler<CultureInfo> LanguageChanged;

        public FileBasedLocalizer(string resourcesFolder)
        {
            _currentCulture = CultureInfo.CurrentCulture;
            _resourcesFolder = resourcesFolder;
            InitResources();
        }

        public FileBasedLocalizer(string resourcesFolder, CultureInfo culture)
        {
            _currentCulture = culture;
            _resourcesFolder = resourcesFolder;
            InitResources();
        }

        public string this[string key]
        {
            get
            {
                if (IsInTranslationMode)
                {
                    return key;
                }
                if (_keyValues.ContainsKey(key))
                {
                    return _keyValues[key];
                }
                foreach (var localizer in _additionalLocalizers)
                {
                    if (localizer.ContainsKey(key))
                    {
                        return localizer[key];
                    }
                }
                return key;
            }
        }

        public bool IsInTranslationMode { get; set; }

        public CultureInfo CurrentCulture => _currentCulture;

        public void AddAdditionalLocalizer(ILocalizer localizer)
        {
            _additionalLocalizers.Add(localizer);
        }

        public bool ContainsKey(string key)
        {
            return _keyValues.ContainsKey(key);
        }

        public void SetLanguage(CultureInfo culture)
        {
            _currentCulture = culture;
            InitResources();

            if (_additionalLocalizers.Count > 0)
            {
                foreach (var localizer in _additionalLocalizers)
                {
                    localizer.SetLanguage(culture);
                }
            }
            LanguageChanged?.Invoke(this, _currentCulture);
        }

        private void InitResources()
        {
            _keyValues.Clear();
            var dir = new DirectoryInfo(_resourcesFolder);
            var files = dir.GetFiles($"*{_currentCulture.Name}.json");
            var list = new List<KeyValuePair<string, string>>();
            foreach (var item in files)
            {
                var values = JsonSerializer.Deserialize<IDictionary<string, string>>(File.ReadAllText(item.FullName, Encoding.UTF8));
                list = list.Concat(values).ToList();
            }
            _keyValues = list.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}