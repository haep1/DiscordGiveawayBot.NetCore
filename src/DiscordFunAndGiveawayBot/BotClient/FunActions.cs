using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace BotClient
{
    public class FunActions : ModuleBase
    {
        private static IMessageChannel _channel;
        private static IUser _user;
        private static IUser _messageAuthor;
        private static IChannel _currentChannel;

        private DiscordClient _discordClient;

        public FunActions(IDiscordClient client)
        {
            _discordClient = (DiscordClient)client;
        }

        [Command("sendmessage"), Summary("Initializes a new MessageSend")]
        public async Task SendMessage(IMessageChannel channel)
        {
            _channel = channel;
            _messageAuthor = _discordClient.Client.CurrentUser;
            _currentChannel = Context.Channel;

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            _discordClient.Client.MessageReceived += Client_MessageReceived;
            await ReplyAsync(_discordClient.CultureHelper.GetAdminString("EnterMessageNow"));
        }

        [Command("sendmessage"), Summary("Initializes a new MessageSend")]
        public async Task SendMessageWithMention(IMessageChannel channel, IUser user)
        {
            _user = user;
            await SendMessage(channel);
        }

        private async Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            if (!arg.Author.IsBot && arg.Author == _messageAuthor && arg.Channel == _currentChannel)
            {
                _discordClient.Client.MessageReceived -= Client_MessageReceived;
                await _channel.SendMessageAsync(_user?.Mention + " " + arg.Content);
                _channel = null;
                _user = null;
                _messageAuthor = null;
                _currentChannel = null;
            }
        }
    }
}
