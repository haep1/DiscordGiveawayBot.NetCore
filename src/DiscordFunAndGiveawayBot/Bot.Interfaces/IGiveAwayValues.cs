using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bot.Interfaces
{
    public interface IGiveAwayValues : IDiscordValues
    {
        void Reset();

        List<CultureInfo> AwardCultures { get; set; }
        string Codeword { get; set; }
        uint CountGiveAways { get; set; }
        uint CountWinners { get; }
        Dictionary<CultureInfo, string> CultureAward { get; set; }
        CultureInfo CurrentAwardLanguage { get; set; }
        DateTime? GiveAwayDateTime { get; set; }
        TimeSpan? GiveAwayTime { get; set; }
        IGiveAway Owner { get; set; }
        IGuild ServerGuild { get; set; }
        GiveAwayState State { get; set; }
        IScheduleManager Timer { get; set; }
    }
}
