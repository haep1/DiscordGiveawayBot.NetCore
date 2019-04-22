using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace UltraGiveawayBot
{
    public class GiveAwayValues
    {
        public IGuild ServerGuild { get; set; }
        public TimeSpan? GiveAwayTime { get; set; }
        public DateTime? GiveAwayDateTime { get; set; }
        public uint CountGiveAways { get; set; }
        public uint CountWinners { get { return 1; } }
        public string Codeword { get; set; }
        //public List<IUser> Winners { get; set; }
        public IMessageChannel GiveawayChannel { get; set; }
        public IMessageChannel AdminChannel { get; set; }
        public IUser AdminUser { get; set; }
        public Dictionary<string, string> CultureAward { get; set; } = new Dictionary<string, string>();
        public static string WinMessageGerman { get
            {
                return "Liebe Wusel Freunde, wir haben einen Gewinner:";
            }
        }
        public static string WinMessageEnglish
        {
            get
            {
                return "Dear Wusel friends, we got a winner:";
            }
        }

        public GiveAwayState State { get; set; }
        public Timer Timer { get; set; }

        public void Reset()
        {
            GiveAwayTime = null;
            GiveAwayDateTime = null;
            CountGiveAways = 0;
            Codeword = null;
            GiveawayChannel = null;
            AdminChannel = null;
            AdminUser = null;
            CultureAward = new Dictionary<string, string>();
            State = GiveAwayState.None;
            if (Timer != null)
            {
                Timer.StopTimer();
            }
            Timer = null;
        }
    }
}
