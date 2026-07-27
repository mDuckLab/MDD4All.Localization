using System;
using System.Collections.Generic;
using System.Globalization;

namespace MDD4All.Localization.Contracts
{
    public interface ILanguageSetter
    {
        event EventHandler? CultureChanged;

        void SetCulture(string cultureName);

        List<CultureInfo> SupportedCultures { get; set; }

        CultureInfo CurrentCulture { get; }
    }
}
