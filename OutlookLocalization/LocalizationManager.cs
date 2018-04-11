using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace OutlookLocalization
{
    public static class LocalizationManager
    {
        static ResourceManager _resourceManager;

        static bool _resourceManagerLoaded;

        public static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null && !_resourceManagerLoaded)
                {
                    _resourceManager = GetResourceManager();
                    _resourceManagerLoaded = true;
                }

                return _resourceManager;
            }
            set
            {
                _resourceManager = value ?? throw new ArgumentNullException("value");
                UpdateLocalizations();
            }
        }

        public static CultureInfo UICulture
        {
            get => Thread.CurrentThread.CurrentUICulture;
            set
            {
                Thread.CurrentThread.CurrentUICulture = value ?? throw new ArgumentNullException("value");
                UpdateLocalizations();
            }
        }

        static List<LocExtension> _localizations = new List<LocExtension>();
        static int _localizationPurgeCount;

        internal static void AddLocalization(LocExtension localization)
        {
            if (localization == null)
            {
                throw new ArgumentNullException("localization");
            }

            if (_localizationPurgeCount > 50)
            {
                var localizatons = new List<LocExtension>(_localizations.Count);

                foreach (var item in _localizations)
                {
                    if (item.IsAlive)
                    {
                        localizatons.Add(item);
                    }
                }

                _localizations = localizatons;
                _localizationPurgeCount = 0;
            }

            _localizations.Add(localization);
            _localizationPurgeCount++;
        }

        static ResourceManager GetResourceManager()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null && string.Compare(assembly.GetName().Name, "Blend", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                assembly = null;
            }
            if (assembly == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var item in assemblies)
                {
                    if (item.EntryPoint != null)
                    {
                        var applicationType = item.GetType(item.GetName().Name + ".App", false);
                        if (applicationType != null && typeof(System.Windows.Application).IsAssignableFrom(applicationType))
                        {
                            if (string.Compare(item.GetName().Name, "Blend", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                            }
                            else
                            {
                                assembly = item;
                                break;
                            }
                        }
                    }
                }
            }

            if (assembly != null)
            {
                try
                {
                    return new ResourceManager(assembly.GetName().Name + ".Properties.Resources", assembly);
                }
                catch (MissingManifestResourceException) { }
            }

            return null;
        }

        static void UpdateLocalizations()
        {
            foreach (var item in _localizations)
            {
                item.UpdateTargetValue();
            }
        }
    }
}