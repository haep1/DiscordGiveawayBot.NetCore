﻿using System;
using System.Threading.Tasks;
using BotClient;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace UltraGiveawayBot
{
    public interface IDiscordClient
    {
        CultureHelper CultureHelper { get; set; }
        DiscordSocketClient Client { get; set; }
        bool IsRunnging { get; }

        Task RunBot(IServiceProvider provider, IConfiguration configuration);
    }
}