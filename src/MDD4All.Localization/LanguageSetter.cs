using MDD4All.Localization.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MDD4All.Localization
{
    public class LanguageSetter : ILanguageSetter
    {
        public LanguageSetter() 
        {
            SupportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("de-DE")
            };

            CurrentCulture = CultureInfo.CurrentUICulture;

            SetCulture(CurrentCulture.Name);
        }

        public LanguageSetter(List<CultureInfo> supportedCutures, CultureInfo currentCulture)
        {
            SupportedCultures = supportedCutures;
            CurrentCulture = currentCulture;

            SetCulture(CurrentCulture.Name);
        }

        public event EventHandler? CultureChanged;

        public List<CultureInfo> SupportedCultures { get; set; }
    
        public CultureInfo CurrentCulture { get; set; }

        public void SetCulture(string cultureName)
        {
            CultureInfo? culture = null;
            foreach (CultureInfo cultureInfo in SupportedCultures)
            {
                if (cultureInfo.Name == cultureName)
                {
                    culture = cultureInfo;
                    break;
                }
            }

            if (culture != null)
            {
                CurrentCulture = culture;

                CultureChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
