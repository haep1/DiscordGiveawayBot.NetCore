using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace BotClient
{
    public class CultureHelper
    {
        private ResourceManager _resManager;


        public List<CultureInfo> OutputCultures { get; private set; }

        public CultureInfo AdminCulture { get; private set; }

        public CultureHelper(List<CultureInfo> outputCultures, CultureInfo adminCulture)
        {
            OutputCultures = outputCultures;
            AdminCulture = adminCulture;

            _resManager = Resources.Resources.ResourceManager;
        }

        public string GetAdminString(string key)
        {
            string result = _resManager.GetString(key, AdminCulture);
            return result;
        }

        public string GetOutputString(string key, CultureInfo culture)
        {
            if (!OutputCultures.Contains(culture))
            {
                throw new ApplicationException("Culture not supported:" + culture.DisplayName);
            }
            return _resManager.GetString(key, culture);
        }
    }
}
