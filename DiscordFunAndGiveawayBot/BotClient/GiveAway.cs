using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UltraGiveawayBot
{
    public class GiveAway : ModuleBase
    {
        public static List<GiveAwayValues> InitValues { get; private set; }

        static GiveAway()
        {
            InitValues = new List<GiveAwayValues>();
        }

        public GiveAway()
        {
            DiscordClient.Client.MessageReceived -= Client_MessageReceived;
            DiscordClient.Client.MessageReceived += Client_MessageReceived;
        }

        [Command("initgiveaway"), Summary("Initialisiert ein neues Giveaway")]
        public async Task InitGiveAway(IMessageChannel channel)
        {
            StopGiveawayInternal();
            string message = "Alles klar, lass uns einen neuen Giveaway starten!" + Environment.NewLine +
                             "Du kannst jederzeit `cancel` eingeben um das Giveaway abzubrechen" + Environment.NewLine +
                             "Bitte sag mir als Erstes, wann die Ziehungen stattfinden sollen" + Environment.NewLine +
                             "Gib entweder eine Zeit z.B. `20:00` ein (für heute um 20:00 Uhr), oder Datum + Zeit wie z.B. `22.04.2019 20:00`";
            GiveAwayValues values = new GiveAwayValues();
            values.ServerGuild = Context.Guild;
            values.GiveawayChannel = channel;
            values.AdminChannel = Context.Channel;
            values.AdminUser = Context.User;
            values.State = GiveAwayState.SetGiveAwayTime;
            InitValues.Add(values);

            await ReplyAsync(message, false, null);
        }

        private async Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            Console.WriteLine("GiveawayMessageReceived: " + arg.Content);
            GiveAwayValues inits = null;
            inits = GetCurrentInitValues();
            if (inits != null)
            {
                if (arg is SocketUserMessage userMessage &&
                    !userMessage.Author.IsBot &&
                    userMessage.Channel == inits.AdminChannel &&
                    userMessage.Author == inits.AdminUser)
                {                   
                    if (userMessage.Content.ToLower().Equals("cancel"))
                    {
                        Console.WriteLine("Giveaway canceled");
                        DiscordClient.Client.MessageReceived -= Client_MessageReceived;
                    }

                    switch (inits.State)
                    {
                        case GiveAwayState.SetGiveAwayTime:
                            await SetGiveAwayTime(userMessage.Content);
                            break;
                        case GiveAwayState.SetCountGiveAways:
                            await SetCountGiveAways(userMessage.Content);
                            break;
                        case GiveAwayState.SetCodeword:
                            await SetCodeword(userMessage.Content);
                            break;
                        case GiveAwayState.SetAwardGerman:
                            await SetAwardGerman(userMessage.Content);
                            break;
                        case GiveAwayState.SetAwardEnglish:
                            await SetAwardEnglish(userMessage.Content);
                            break;
                        case GiveAwayState.Initialized:
                            await StartGiveAwayTimer(userMessage.Content);
                            break;
                    }
                }
            }
        }

        private async Task SetCodeword(string message)
        {
            GiveAwayValues inits = GetCurrentInitValues();
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

        private async Task SetAwardGerman(string message)
        {
            GiveAwayValues inits = GetCurrentInitValues();
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

        private async Task SetAwardEnglish(string message)
        {
            GiveAwayValues inits = GetCurrentInitValues();
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

        private async Task SetCountGiveAways(string message)
        {
            GiveAwayValues inits = GetCurrentInitValues();
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

        private async Task SetGiveAwayTime(string message)
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
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

        public async Task StartGiveAwayTimer(string message)
        {
            if(!message.Equals("start"))
            {
                return;
            }

            GiveAwayValues inits = GetCurrentInitValues();
            if (inits == null)
            {
                await ReplyAsync($"Giveaway wurde nicht initialisiert!");
                return;
            }

            DiscordClient.Client.MessageReceived -= Client_MessageReceived;
            inits.Timer = new Timer();
            Console.WriteLine("StarteGiveaway");

            if (!inits.Timer.SetUpTimer(inits.GiveAwayTime, inits.GiveAwayDateTime, new Action(() =>
            {
                Task.Run(SetTimerValues);
            })))
            {
                await ReplyAsync($"Fehler bei der Erstellung des Timers. Giveaway konnte nicht initialisiert werden.");
                return;
            }

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription(inits.FirstGiveawayMessageGerman);
            AddEndTimeField(inits, embed, true);

            await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());

            embed = new EmbedBuilder();
            embed.WithDescription(inits.FirstGiveawayMessageEnglish);
            AddEndTimeField(inits, embed, false);

            await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());

            await ReplyAsync($":tada: Giveaway wurde gestartet!");
        }

        private static void AddEndTimeField(GiveAwayValues inits, EmbedBuilder embed, bool german)
        {
            if (german)
            {
                if (inits.GiveAwayTime != null)
                {
                    embed.AddField($"Die Teilnahme endet um {inits.GiveAwayTime.Value.ToString("hh\\:mm")} Uhr.", "Viel Erfolg!");
                }
                else if (inits.GiveAwayDateTime != null)
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("de-DE");
                    embed.AddField($"Die Teilnahme endet am {inits.GiveAwayDateTime.Value.ToString("g", culture)}.", "Viel Erfolg!");
                }
            }
            else
            {
                if (inits.GiveAwayTime != null)
                {
                    embed.AddField($"The participation ends at {inits.GiveAwayTime.Value.ToString("hh\\:mm")}.", "Good luck!");
                }
                else
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                    embed.AddField($"The participation ends at {inits.GiveAwayDateTime.Value.ToString("g", culture)}.", "Good luck!");
                }
            }
        }

        private async Task SetTimerValues()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                await ShoutOutTheWinners(inits.GiveawayChannel, inits.Codeword);
                inits.CountGiveAways--;
                if (inits.CountGiveAways == 0)
                {
                    await StopGiveaway();
                }
                else
                {
                    try
                    {
                        // Wait one Minute to avoid problems with timespan
                        Thread.Sleep(60000);
                        EmbedBuilder embed = new EmbedBuilder();
                        embed.WithDescription(inits.NextGiveawayMessageGerman);
                        AddEndTimeField(inits, embed, true);

                        await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());

                        embed = new EmbedBuilder();
                        embed.WithDescription(inits.NextGiveawayMessageEnglish);
                        AddEndTimeField(inits, embed, false);

                        await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());

                        inits.Timer = new Timer();
                        if (inits.GiveAwayDateTime != null)
                        {
                            inits.GiveAwayDateTime = inits.GiveAwayDateTime.Value.AddDays(1);
                        }

                        inits.Timer.SetUpTimer(inits.GiveAwayTime, inits.GiveAwayDateTime, new Action(() =>
                        {
                            Task.Run(SetTimerValues);
                        }));
                    }
                    catch (Exception ex)
                    {
                        await ReplyAsync("Fehler bei Wiederholungsankündigung: " + ex.Message);
                    }
                }
            }
        }

        [Command("getwinner"), Summary("Gibt die Gewinner eines Channels aus")]
        public async Task GetWinners([Summary("Name des Channels")] IMessageChannel channel, string codeword)
        {
            await ShoutOutTheWinners(channel, codeword);
        }

        [Command("stopgiveaway"), Summary("Stoppt das Giveaway")]
        public async Task StopGiveaway()
        {
            StopGiveawayInternal();

            await ReplyAsync("Giveaway wurde gestoppt!");
        }

        private void StopGiveawayInternal()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                inits.Reset();
                InitValues.Remove(inits);
            }
        }

        private GiveAwayValues GetCurrentInitValues()
        {
            if (InitValues != null && Context != null)
            {
                GiveAwayValues initValues = InitValues.SingleOrDefault(x => x.ServerGuild == Context.Guild);
                return initValues;
            }
            return null;
        }

        private async Task ShoutOutTheWinners(IMessageChannel channel, string codeword)
        {
            List<IReadOnlyCollection<IMessage>> readonlymessages = await channel.GetMessagesAsync(1000).ToList();
            List<IMessage> messages = readonlymessages.SelectMany(x => x).ToList();
            IMessage latestBotMessage = messages.FirstOrDefault(x => x.Author.Id == Context.Client.CurrentUser.Id);
            List<IMessage> latestMessages;
            if (latestBotMessage != null)
            {
                latestMessages = new List<IMessage>();
                int index = messages.IndexOf(latestBotMessage);
                for (int i = 0; i < index; i++)
                {
                    latestMessages.Add(messages[i]);
                }
            }
            else
            {
                latestMessages = messages;
            }

            var users = latestMessages.Where(m => m.Content.ToLower().Contains(codeword.ToLower()))
                                      .Select(y => y.Author).Distinct().Where(x => !x.IsBot).ToList();
            List<IUser> winners = GetWinners(1, users);



            string germanMessage = GiveAwayValues.WinMessageGerman + Environment.NewLine;

            int winCount = 1;
            foreach (IUser winner in winners)
            {
                germanMessage += $":trophy: {winner.Mention} :trophy:" + Environment.NewLine;
                winCount++;
            }
            germanMessage += $"Herzlichen Glückwunsch!" + Environment.NewLine;

            await channel.SendMessageAsync(germanMessage);

            string englishMessage = GiveAwayValues.WinMessageEnglish + Environment.NewLine;
            foreach (IUser winner in winners)
            {
                englishMessage += $":trophy: {winner.Mention} :trophy:" + Environment.NewLine;
            }
            englishMessage += $"Congratulations!";

            await channel.SendMessageAsync(englishMessage);

            await ReplyAsync("Der Gewinner wurde bekannt gegeben!");
        }

        public List<IUser> GetWinners(uint winnersCount, List<IUser> authors)
        {
            List<IUser> winners = new List<IUser>();
            List<int> randomNumbers = new List<int>();
            Random r = new Random();
            if (authors.Count < winnersCount)
            {
                winnersCount = (uint)authors.Count;
            }

            for (int i = 0; i < winnersCount; i++)
            {
                int nextAuthor;
                do
                {
                    nextAuthor = r.Next(authors.Count);
                } while (randomNumbers.Contains(nextAuthor));

                randomNumbers.Add(nextAuthor);
                winners.Add(authors[nextAuthor]);
            }

            return winners;
        }
    }
}
