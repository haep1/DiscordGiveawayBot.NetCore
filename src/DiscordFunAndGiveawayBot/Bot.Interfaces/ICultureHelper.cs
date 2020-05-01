using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bot.Interfaces
{
    public interface ICultureHelper
    {
        string GetAdminString(string key);
        string GetOutputString(string key, CultureInfo culture);

        CultureInfo AdminCulture { get; }
        List<CultureInfo> OutputCultures { get; }
    }
}
