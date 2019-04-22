using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using UltraGiveawayBot;

namespace BotClient
{
    class GiveAwayFunctions : ModuleBase
    {
        private IDiscordClient _discordClient;

        public GiveAwayFunctions(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task SetGiveAwayTime(GiveAwayValues inits, string message)
        {
            if (inits != null)
            {
                CultureInfo culture = _discordClient.CultureHelper.AdminCulture;
                DateTimeStyles styles = DateTimeStyles.None;

                if (TimeSpan.TryParse(message, out TimeSpan timespan))
                {
                    inits.GiveAwayTime = timespan;
                    inits.State = GiveAwayState.SetCountGiveAways;
                    await ReplyAsync($"Alles klar, die nächste Ziehung findet zu folgendem Zeitpunkt statt: {timespan.ToString("hh\\:mm")}"
                        + Environment.NewLine + "Wie oft soll das Giveaway stattfinden?" + Environment.NewLine +
                        "`Gebe z.B. 2 ein um das Event zweimal zu wiederholen`", false, null);
                    return;
                }
                else if (DateTime.TryParse(message, culture, styles, out DateTime dateTime))
                {
                    inits.GiveAwayDateTime = dateTime;
                    inits.State = GiveAwayState.SetCountGiveAways;
                    await ReplyAsync($"Alles klar, die nächste Ziehung findet zu folgendem Zeitpunkt statt: {dateTime.ToLongDateString()}"
                        + Environment.NewLine + "Wie oft soll das Giveaway stattfinden?" + Environment.NewLine +
                        "`Gebe z.B. 2 ein um das Event zweimal zu wiederholen`", false, null);
                    return;
                }
            }

            await ReplyAsync($"Du hast kein korrektes Zeitformat eingegeben, z.B. `20:00`" + Environment.NewLine +
                        "Bitte probiere es erneut oder gebe `cancel` ein um das Giveaway abzubrechen", false, null);
        }

        public async Task SetCountGiveAways(GiveAwayValues inits, string message)
        {
            if (inits != null && uint.TryParse(message, out uint count))
            {
                inits.CountGiveAways = count;
                inits.State = GiveAwayState.SetCodeword;
                await ReplyAsync($"Sehr gut, wie soll das Schlüsselwort lauten um an der Ziehung teilzunehmen?" + Environment.NewLine +
                   "`Gebe jetzt das Schlüsselwort, z.B. 'giveaway!' ein.`", false, null);
            }
            else
            {
                await ReplyAsync($"Du hast keine korrekte Zahl eingegeben, z.B. `2`" + Environment.NewLine +
                            "Bitte probiere es erneut oder gebe `cancel` ein um das Giveaway abzubrechen", false, null);
            }
        }

        public async Task SetCodeword(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.Codeword = message;
                inits.State = GiveAwayState.SetAwardGerman;

                await ReplyAsync($"OK, welcher Preis soll verlost werden?!" + Environment.NewLine +
                                  "Gib zum Beispiel `Ein toller Key Code` ein!", false, null);
            }
            else
            {
                await ReplyAsync($"Du hast keine korrekte Zeichenfolge eingegeben, z.B. `giveaway!`" + Environment.NewLine +
                            "Bitte probiere es erneut oder gebe `cancel` ein um das Giveaway abzubrechen", false, null);
            }
        }

        public async Task SetAwardGerman(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.AwardGerman = message;
                inits.State = GiveAwayState.SetAwardEnglish;

                await ReplyAsync($"Und bitte jetzt den Preis auf english eingeben!" + Environment.NewLine +
                                  "Schreibe zum Beispiel: `An awesome key code` ein!", false, null);
            }
            else
            {
                await ReplyAsync($"Du hast keine korrekte Zeichenfolge eingegeben, z.B. `Ein toller Key Code`" + Environment.NewLine +
                            "Bitte probiere es erneut oder gebe `cancel` ein um das Giveaway abzubrechen", false, null);
            }
        }

        public async Task SetAwardEnglish(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.AwardEnglish = message;
                inits.State = GiveAwayState.Initialized;

                await ReplyAsync($"Super, der Giveaway wurde fertig initialisiert!" + Environment.NewLine +
                                  "Um zu Starten gebe jetzt bitte den Befehl: `start` ein!", false, null);
            }
            else
            {
                await ReplyAsync($"Du hast keine korrekte Zeichenfolge eingegeben, z.B. `An awesome key code`" + Environment.NewLine +
                            "Bitte probiere es erneut oder gebe `cancel` ein um das Giveaway abzubrechen", false, null);
            }
        }
    }
}
