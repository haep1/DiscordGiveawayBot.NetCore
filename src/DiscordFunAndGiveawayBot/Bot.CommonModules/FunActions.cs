using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.CommonModules
{
    public class FunActions : ModuleBase
    {
        private static IMessageChannel _channel;
        private static IUser _user;
        private static IUser _messageAuthor;
        private static IChannel _currentChannel;

        private Interfaces.IDiscordClient _discordClient;

        public FunActions(Interfaces.IDiscordClient client)
        {
            _discordClient = client;
        }

        [Command("sendmessage"), Summary("Initializes a new MessageSend")]
        public async Task SendMessage(IMessageChannel channel)
        {
            _channel = channel;
            _messageAuthor = Context.User;
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
                EmbedBuilder embed = null;

                if (arg.Attachments.Count > 0)
                {
                    embed = new EmbedBuilder();
                    foreach (Attachment attachment in arg.Attachments)
                    {
                        // check if it is an image is only possible by checking width/height
                        if (attachment.Width != null)
                        {
                            embed.WithImageUrl(attachment.Url);
                        }
                        else
                        {
                            embed.WithDescription(attachment.Url);
                        }
                    }
                }

                await _channel.SendMessageAsync(_user?.Mention + " " + arg.Content, false, embed.Build());
                _channel = null;
                _user = null;
                _messageAuthor = null;
                _currentChannel = null;
            }
        }
    }
}
