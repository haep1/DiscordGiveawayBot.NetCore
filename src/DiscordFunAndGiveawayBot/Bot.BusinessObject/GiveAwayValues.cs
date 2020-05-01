using Discord;
using Scheduler;
using System;
using System.Collections.Generic;

namespace Bot.BusinessObject
{
    public class GiveAwayValues : DiscordValues
    {
        public IGuild ServerGuild { get; set; }
        public TimeSpan? GiveAwayTime { get; set; }
        public DateTime? GiveAwayDateTime { get; set; }
        public uint CountGiveAways { get; set; }
        public uint CountWinners { get { return 1; } }
        public string Codeword { get; set; }

        public Dictionary<string, string> CultureAward { get; set; } = new Dictionary<string, string>();

        public GiveAwayState State { get; set; }
        public ScheduleManager Timer { get; set; }

        public string CurrentAwardLanguage { get; set; }

        public override void Reset()
        {
            base.Reset();
            GiveAwayTime = null;
            GiveAwayDateTime = null;
            CountGiveAways = 0;
            Codeword = null;
            CultureAward = new Dictionary<string, string>();
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
