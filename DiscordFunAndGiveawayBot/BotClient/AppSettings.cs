using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotClient
{
    public class AppSettings
    {
        public string DiscordToken { get; set; }
        public string[] OutputCultures { get; set; }
        public string AdminCulture { get; set; }
    }
}
