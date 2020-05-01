using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Bot.Interfaces
{
    public interface IDiscordClient
    {
        ICultureHelper CultureHelper { get; set; }
        DiscordSocketClient Client { get; set; }
        bool IsRunning { get; }

        Task RunBot(IServiceProvider provider, IConfiguration configuration, ICultureHelper cultureHelper, string discordToken = null);
    }
}