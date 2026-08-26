using MDD4All.Localization.Contracts;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace MDD4All.Localization
{
    // Reads a resx for the language the user picked, handed over explicitly.
    //
    // The localizer the framework builds resolves against CultureInfo.CurrentUICulture. That one
    // lives in an AsyncLocal and belongs to an execution flow - and the flow the Blazor Hybrid
    // renderer runs in cannot be written to from outside. ILanguageSetter always knows which
    // language was picked, so there is nothing to guess.
    public class LanguageSetterStringLocalizer : IStringLocalizer
    {
        private readonly ResourceManager _resources;

        private readonly ILanguageSetter _languageSetter;

        public LanguageSetterStringLocalizer(string baseName, Assembly assembly,
                                             ILanguageSetter languageSetter)
        {
            _resources = new ResourceManager(baseName, assembly);
            _languageSetter = languageSetter;
        }

        public LocalizedString this[string name]
        {
            get
            {
                string? text = Lookup(name);

                LocalizedString result;

                if (text == null)
                {
                    // Missing keys give back the key itself, marked as not found.
                    result = new LocalizedString(name, name, true);
                }
                else
                {
                    result = new LocalizedString(name, text, false);
                }

                return result;
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                LocalizedString found = this[name];

                string text = string.Format(_languageSetter.CurrentCulture, found.Value, arguments);

                return new LocalizedString(name, text, found.ResourceNotFound);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            List<LocalizedString> result = new List<LocalizedString>();

            ResourceSet? set = null;

            try
            {
                set = _resources.GetResourceSet(_languageSetter.CurrentCulture, true,
                                                includeParentCultures);
            }
            catch (MissingManifestResourceException)
            {
                set = null;
            }

            if (set != null)
            {
                foreach (DictionaryEntry entry in set)
                {
                    string key = (string)entry.Key;

                    string? text = Lookup(key);

                    if (text != null)
                    {
                        result.Add(new LocalizedString(key, text, false));
                    }
                }
            }

            return result;
        }

        private string? Lookup(string name)
        {
            string? result = null;

            try
            {
                result = _resources.GetString(name, _languageSetter.CurrentCulture);
            }
            catch (MissingManifestResourceException)
            {
                result = null;
            }

            return result;
        }
    }
}
