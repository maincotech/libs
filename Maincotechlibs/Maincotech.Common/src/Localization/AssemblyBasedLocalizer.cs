using Maincotech.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Maincotech.Localization
{
    public class AssemblyBasedLocalizer : ILocalizer
    {
        private readonly ILogger _Logger = AppRuntimeContext.Current.GetLogger<AssemblyBasedLocalizer>();

        private CultureInfo _currentCulture;
        private readonly Assembly _resourcesAssembly;
        public virtual string ResourcesPatten => @"^.*Resources\..*{0}\.json";
        protected IDictionary<string, string> _keyValues = new Dictionary<string, string>();
        private ConcurrentDictionary<Type, ILocalizer> _additionalLocalizers = new ConcurrentDictionary<Type, ILocalizer>();

        public event EventHandler<CultureInfo> LanguageChanged;

        public AssemblyBasedLocalizer(Assembly assembly, CultureInfo culture)
        {
            _currentCulture = culture;
            _resourcesAssembly = assembly;
            InitResources();
        }

        public AssemblyBasedLocalizer(Assembly assembly)
        {
            _resourcesAssembly = assembly;
            _currentCulture = CultureInfo.CurrentCulture;
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
                foreach (var localizer in _additionalLocalizers.Values)
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

        public virtual void SetLanguage(CultureInfo culture)
        {
            _currentCulture = culture;
            InitResources();
            if (_additionalLocalizers.Count > 0)
            {
                foreach (var localizer in _additionalLocalizers.Values)
                {
                    localizer.SetLanguage(culture);
                }
            }
            LanguageChanged?.Invoke(this, _currentCulture);
        }

        public bool ContainsKey(string key)
        {
            return _keyValues.ContainsKey(key);
        }

        private void InitResources()
        {
            _keyValues.Clear();

            var availableResources = _resourcesAssembly
               .GetManifestResourceNames()
               .Select(x => Regex.Match(x, string.Format(ResourcesPatten, _currentCulture.Name)))
               .Where(x => x.Success)
               .Select(x => x.Value)
               .ToList();

            var list = new List<KeyValuePair<string, string>>();
            foreach (var item in availableResources)
            {
                try
                {
                    // Read the file
                    using var fileStream = _resourcesAssembly.GetManifestResourceStream(item);
                    if (fileStream == null)
                    {
                        continue;
                    }
                    using var streamReader = new StreamReader(fileStream);
                    var content = streamReader.ReadToEnd();
                    var values = JsonSerializer.Deserialize<IDictionary<string, string>>(content);
                    list = list.Concat(values).ToList();
                }
                catch (Exception e)
                {
                    _Logger.Warning($"Failed to load resource file:{item}", e);
                }
            }
            _keyValues = list.ToDictionary(x => x.Key, x => x.Value);
        }

        public void AddAdditionalLocalizer(ILocalizer localizer)
        {
            if (_additionalLocalizers.ContainsKey(localizer.GetType()) == false)
            {
                _additionalLocalizers.TryAdd(localizer.GetType(), localizer);
            }
        }
    }
}