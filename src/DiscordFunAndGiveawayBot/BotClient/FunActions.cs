﻿using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace UltraGiveawayBot
{
    public class FunActions : ModuleBase
    {
        private IDiscordClient _discordClient;
        public FunActions(IDiscordClient client)
        {
            _discordClient = client;
        }

        private static IMessageChannel _channel;
        private static IUser _user;
        [Command("sendmessage"), Summary("Initializes a new MessageSend")]
        public async Task SendMessage(IMessageChannel channel, IUser user)
        {
            _channel = channel;
            _user = user;

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            _discordClient.Client.MessageReceived += Client_MessageReceived;
            await ReplyAsync(_discordClient.CultureHelper.GetAdminString("EnterMessageNow"));
        }

        [Command("sendmessage"), Summary("Initializes a new MessageSend")]
        public async Task SendMessage2(IMessageChannel channel)
        {
            _channel = channel;

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            _discordClient.Client.MessageReceived += Client_MessageReceived;
            await ReplyAsync(_discordClient.CultureHelper.GetAdminString("EnterMessageNow"));
        }

        private async Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            if (!arg.Author.IsBot)
            {
                _discordClient.Client.MessageReceived -= Client_MessageReceived;
                await _channel.SendMessageAsync(_user?.Mention + " " + arg.Content);
                _channel = null;
                _user = null;
            }
        }
    }
}