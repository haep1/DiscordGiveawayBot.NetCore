using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace UltraGiveawayBot
{
    public static class DiscordClient
    {
        private static CommandService Commands { get; set; }
        public static DiscordSocketClient Client { get; set; }
        private static IServiceProvider Services { get; set; }

        public static async Task RunBot(string token)
        {
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

            Services = new ServiceCollection().BuildServiceProvider();

            await Commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Services);

            Client.MessageReceived -= Client_MessageReceived;
            Client.MessageReceived += Client_MessageReceived;
            Client.Log += Client_Log;

            await Client.LoginAsync(Discord.TokenType.Bot, token);
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

        private static async Task Client_MessageReceived(SocketMessage arg)
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
