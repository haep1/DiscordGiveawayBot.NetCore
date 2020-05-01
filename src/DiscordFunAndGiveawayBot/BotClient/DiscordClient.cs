using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Bot.Interfaces;
using Bot.Giveaway;
using Bot.CommonModules;

namespace BotClient
{
    public class DiscordClient : IDiscordClient
    {
        private CommandService Commands { get; set; }
        private IServiceProvider Services { get; set; }

        public ICultureHelper CultureHelper { get; set; }
        public DiscordSocketClient Client { get; set; }

        public ISocketMessageChannel CurrentChannel { get; set; }


        public bool IsRunning { get; private set; }

        public async Task RunBot(IServiceProvider provider, IConfiguration configuration, ICultureHelper cultureHelper, string discordToken = null)
        {
            IsRunning = true;
            CultureHelper = cultureHelper;
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Info
            });

            Commands = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = Discord.LogSeverity.Info,
                DefaultRunMode = RunMode.Async
            });

            Services = provider;

            // Read cultures from appsettings

            // Add this assembly for command fetching
            await Commands.AddModuleAsync<GiveAway>(Services);
            await Commands.AddModuleAsync<FunActions>(Services);
            await Commands.AddModuleAsync<HelpModule>(Services);

            // Add eventhandlers
            Client.MessageReceived -= Client_MessageReceived;
            Client.MessageReceived += Client_MessageReceived;
            Client.Log += Client_Log;

            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            discordToken = discordToken ?? appSettings.DiscordToken;

            // Login and start
            await Client.LoginAsync(Discord.TokenType.Bot, discordToken);
            await Client.SetGameAsync("giveaway | %help");
            await Client.StartAsync();
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg is SocketUserMessage userMessage)
            {
                int argPos = 0;

                // Receive all commands that start with '!'
                if ((userMessage.HasCharPrefix('%', ref argPos) ||
                    userMessage.HasMentionPrefix(Client.CurrentUser, ref argPos)) &&
                    !userMessage.Author.IsBot)
                {
                    CurrentChannel = arg.Channel;
                    Console.WriteLine("CommandAccepted: " + arg.Content);
                    CommandContext context = new CommandContext(Client, userMessage);
                    // Execute the command
                    IResult result = await Commands.ExecuteAsync(context, argPos, Services);
                    // Ignore Unknown command error - otherwise all messages that begin with '!'
                    // and are no known commands would create an error
                    if (!result.IsSuccess && !result.ErrorReason.Equals("Unknown command."))
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }

        private static Task Client_Log(Discord.LogMessage arg)
        {
            if (arg.Message != null && !arg.Message.StartsWith("Received Dispatch"))
            {
                Console.WriteLine("Logging: " + arg.Message);
            }
            return Task.CompletedTask;
        }
    }
}
