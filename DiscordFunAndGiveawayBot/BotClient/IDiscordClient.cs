using System.Threading.Tasks;
using Discord.WebSocket;

namespace UltraGiveawayBot
{
    public interface IDiscordClient
    {
        DiscordSocketClient Client { get; set; }
        bool IsRunnging { get; }

        Task RunBot(string token, System.IServiceProvider provider);
    }
}