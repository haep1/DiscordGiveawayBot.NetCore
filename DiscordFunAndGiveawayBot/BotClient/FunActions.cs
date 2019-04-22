using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace UltraGiveawayBot
{
    public class FunActions : ModuleBase
    {
        private static IMessageChannel _channel;
        private static IUser _user;
        [Command("sendmessage"), Summary("Initialisiert ein neues Giveaway")]
        public async Task SendMessage(IMessageChannel channel, IUser user)
        {
            _channel = channel;
            _user = user;

            DiscordClient.Client.MessageReceived -= Client_MessageReceived;
            DiscordClient.Client.MessageReceived += Client_MessageReceived;
            await ReplyAsync("Kein Problem, gib die Nachricht jetzt ein!");
        }

        [Command("sendmessage"), Summary("Initialisiert ein neues Giveaway")]
        public async Task SendMessage2(IMessageChannel channel)
        {
            _channel = channel;

            DiscordClient.Client.MessageReceived -= Client_MessageReceived;
            DiscordClient.Client.MessageReceived += Client_MessageReceived;
            await ReplyAsync("Kein Problem, gib die Nachricht jetzt ein!");
        }

        private async Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            if (!arg.Author.IsBot)
            {
                DiscordClient.Client.MessageReceived -= Client_MessageReceived;
                await _channel.SendMessageAsync(_user?.Mention + " " + arg.Content);
                _channel = null;
                _user = null;
            }
        }
    }
}
