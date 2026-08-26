using MDD4All.Localization.Contracts;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace MDD4All.Localization
{
    // Put in front of the built-in factory so that every IStringLocalizer<T> in the app reads
    // the picked language. StringLocalizer<T> asks a factory for its texts, so swapping the
    // factory reaches components we do not own without touching a line of them.
    public class LanguageSetterStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILanguageSetter _languageSetter;

        private readonly string _resourcesPath;

        private readonly ConcurrentDictionary<string, IStringLocalizer> _localizers
            = new ConcurrentDictionary<string, IStringLocalizer>();

        public LanguageSetterStringLocalizerFactory(ILanguageSetter languageSetter,
                                                    string resourcesPath = "Resources")
        {
            _languageSetter = languageSetter;
            _resourcesPath = resourcesPath;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            Assembly assembly = resourceSource.Assembly;

            return GetOrAdd(BuildBaseName(assembly, resourceSource.FullName), assembly);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(location));

            return GetOrAdd(baseName, assembly);
        }

        // The name the compiler gives the embedded resource, the same one the built-in factory
        // builds: assembly name, the resources folder, then what the type's namespace adds.
        private string BuildBaseName(Assembly assembly, string typeFullName)
        {
            string assemblyName = new AssemblyName(assembly.FullName).Name;

            string rest = typeFullName;

            if (rest.StartsWith(assemblyName + ".", StringComparison.Ordinal))
            {
                rest = rest.Substring(assemblyName.Length + 1);
            }

            string result = assemblyName + ".";

            if (!string.IsNullOrEmpty(_resourcesPath))
            {
                string folder = _resourcesPath.Replace('/', '.');

                folder = folder.Replace(Path.DirectorySeparatorChar, '.');

                result = result + folder + ".";
            }

            return result + rest;
        }

        private IStringLocalizer GetOrAdd(string baseName, Assembly assembly)
        {
            return _localizers.GetOrAdd(baseName,
                key => new LanguageSetterStringLocalizer(key, assembly, _languageSetter));
        }
    }
}
