using Bot.Interfaces;
using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bot.Giveaway
{
    public class GiveAwayValues : IDiscordValues
    {
        public IUser AdminUser { get; set; }
        public IMessageChannel SourceChannel { get; set; }
        public IMessageChannel TargetChannel { get; set; }
        public GiveAway Owner { get; set; }
        public IGuild ServerGuild { get; set; }
        public TimeSpan? GiveAwayTime { get; set; }
        public DateTime? GiveAwayDateTime { get; set; }
        public uint CountGiveAways { get; set; }
        public uint CountWinners { get { return 1; } }
        public string Codeword { get; set; }

        public Dictionary<CultureInfo, string> CultureAward { get; set; } = new Dictionary<CultureInfo, string>();

        public GiveAwayState State { get; set; }
        public IScheduleManager Timer { get; set; }

        public List<CultureInfo> AwardCultures
        {
            get;
            set;
        } = new List<CultureInfo>() { CultureInfo.GetCultureInfo("en-US") };

        public CultureInfo CurrentAwardLanguage { get; set; }

        public void Reset()
        {
            AdminUser = null;
            TargetChannel = null;
            SourceChannel = null;
            GiveAwayTime = null;
            GiveAwayDateTime = null;
            CountGiveAways = 0;
            Codeword = null;
            CultureAward = new Dictionary<CultureInfo, string>();
            CurrentAwardLanguage = null;
            State = GiveAwayState.None;
            if (Timer != null)
            {
                Timer.StopTimer();
            }
            Timer = null;
        }
    }
}
