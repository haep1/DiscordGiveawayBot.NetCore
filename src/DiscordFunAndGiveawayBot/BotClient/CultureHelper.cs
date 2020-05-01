using Bot.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

namespace BotClient
{
    public class CultureHelper : ICultureHelper
    {
        private ResourceManager _resManager;


        public List<CultureInfo> OutputCultures { get; private set; }

        public CultureInfo AdminCulture { get; private set; }

        public CultureHelper(IConfiguration configuration)
        {
            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            OutputCultures = appSettings.OutputCultures.Select(culture => new CultureInfo(culture)).ToList();
            AdminCulture = new CultureInfo(appSettings.AdminCulture);
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
