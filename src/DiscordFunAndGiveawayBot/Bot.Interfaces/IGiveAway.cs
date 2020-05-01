using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Interfaces
{
    public interface IGiveAway
    {
        string GetGiveawayMessage(CultureInfo culture, IGiveAwayValues inits, bool first);
        Task GetWinners([Summary("Name des Channels")]IMessageChannel channel, string codeword);
        List<IUser> GetWinners(uint winnersCount, List<IUser> authors);
        Task InitGiveAway();
        Task InitGiveAway(IMessageChannel channel);
        Task Start();
        Task StartGiveAway(IGiveAwayValues inits);
    }
}
