using BotClient;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace UltraGiveawayBot
{
    public class DiscordClient : IDiscordClient
    {
        private CommandService Commands { get; set; }
        private IServiceProvider Services { get; set; }

        public CultureHelper CultureHelper { get; set; }        
        public DiscordSocketClient Client { get; set; }
        

        public bool IsRunnging { get; private set; }

        public async Task RunBot(IServiceProvider provider, IConfiguration configuration)
        {
            IsRunnging = true;
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                LogLevel = Discord.LogSeverity.Debug,
                DefaultRunMode = RunMode.Async
            });

            Services = provider;
            AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
            var outputCultures = appSettings.OutputCultures.Select(culture => new CultureInfo(culture)).ToList();
            var adminCulture = new CultureInfo(appSettings.AdminCulture);

            CultureHelper = new CultureHelper(outputCultures, adminCulture);

            await Commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Services);

            Client.MessageReceived -= Client_MessageReceived;
            Client.MessageReceived += Client_MessageReceived;
            Client.Log += Client_Log;

            await Client.LoginAsync(Discord.TokenType.Bot, appSettings.DiscordToken);
            await Client.StartAsync();
        }

        private static Task Client_Log(Discord.LogMessage arg)
        {
            if (arg.Message != "Received Dispatch (PRESENCE_UPDATE)")
            {
                Console.WriteLine("Logging: " + arg.Message);
            }
            return Task.CompletedTask;
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            Console.WriteLine("MessageReceived: " + arg.Content);
            if (arg is SocketUserMessage userMessage)
            {
                int argPos = 0;
                if ((userMessage.HasCharPrefix('!', ref argPos) ||
                    userMessage.HasMentionPrefix(Client.CurrentUser, ref argPos)) &&
                    !userMessage.Author.IsBot)
                {
                    Console.WriteLine("MessageAccepted: " + arg.Content);
                    CommandContext context = new CommandContext(Client, userMessage);
                    // Execute the command. (result does not indicate a return value, 
                    // rather an object stating if the command executed successfully)
                    IResult result = await Commands.ExecuteAsync(context, argPos, Services);
                    if (!result.IsSuccess && !result.ErrorReason.Equals("Unknown command."))
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }
    }
}
