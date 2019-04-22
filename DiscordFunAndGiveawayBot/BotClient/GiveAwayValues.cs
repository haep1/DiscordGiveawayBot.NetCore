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
        public string AwardGerman { get; set; }
        public string AwardEnglish { get; set; }
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

        public string NextGiveawayMessageGerman
        {
            get
            {
                return Environment.NewLine + $"Auf geht's zum nächsten Giveaway! " + Environment.NewLine +
                       $"Zu gewinnen gibt es erneut: **{AwardGerman}**" + Environment.NewLine + 
                       $"Schreibt einfach wieder `{Codeword}` um teilzunehmen. :tada:";
            }
        }

        public string NextGiveawayMessageEnglish
        {
            get
            {
                return Environment.NewLine + $"Let's start the next giveaway! " + Environment.NewLine +
                       $"Again you can win: **{AwardEnglish}**" + Environment.NewLine +
                       $"Just write the codeword `{Codeword}` to join. :tada:";
            }
        }

        public string FirstGiveawayMessageGerman
        {
            get
            {
                return Environment.NewLine + $"Liebe Wusel Freunde, lasst uns ein neues Giveaway starten! " + Environment.NewLine +
                       $"Zu gewinnen gibt es: **{AwardGerman}**" + Environment.NewLine +
                       $"Schreibt einfach `{Codeword}` um teilzunehmen. :tada:";
            }
        }

        public string FirstGiveawayMessageEnglish
        {
            get
            {
                return Environment.NewLine + $"Dear Wusel friends, let's start a new giveaway! " + Environment.NewLine +
                       $"You can win: **{AwardEnglish}**" + Environment.NewLine +
                       $"Just write the codeword `{Codeword}` to join. :tada:";
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
            AwardGerman = null;
            AwardEnglish = null;
            State = GiveAwayState.None;
            if (Timer != null)
            {
                Timer.StopTimer();
            }
            Timer = null;
        }

        private void SaveValuesToDb()
        {
            
        }

        private void LoadValuesFromDb()
        {
            
        }
    }
}
