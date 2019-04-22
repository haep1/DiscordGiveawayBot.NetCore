using BotClient;
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

        private IDiscordClient _discordClient;
        private GiveAwayFunctions _functions;

        public GiveAway(IDiscordClient client)
        {
            _discordClient = client;
            _functions = new GiveAwayFunctions(client);

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            _discordClient.Client.MessageReceived += Client_MessageReceived;
        }

        [Command("initgiveaway"), Summary("Initialisiert ein neues Giveaway")]
        public async Task InitGiveAway(IMessageChannel channel)
        {
            StopGiveawayInternal();
            string message = _discordClient.CultureHelper.GetAdminString("GiveawayStartNew") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayTypeCancel") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayEnterTime") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayTimeExample");
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
                        _discordClient.Client.MessageReceived -= Client_MessageReceived;

                        await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayCanceled"), false, null);
                    }

                    switch (inits.State)
                    {
                        case GiveAwayState.SetGiveAwayTime:
                            await _functions.SetGiveAwayTime(inits, userMessage.Content);
                            break;
                        case GiveAwayState.SetCountGiveAways:
                            await _functions.SetCountGiveAways(inits, userMessage.Content);
                            break;
                        case GiveAwayState.SetCodeword:
                            await _functions.SetCodeword(inits, userMessage.Content);
                            break;
                        case GiveAwayState.SetAwardGerman:
                            await _functions.SetAwardGerman(inits, userMessage.Content);
                            break;
                        case GiveAwayState.SetAwardEnglish:
                            await _functions.SetAwardEnglish(inits, userMessage.Content);
                            break;
                        case GiveAwayState.Initialized:
                            await StartGiveAway(inits, userMessage.Content);
                            break;
                    }
                }
            }
        }
      

        public async Task StartGiveAway(GiveAwayValues inits, string message)
        {
            if (!message.Equals("start"))
            {
                return;
            }

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            inits.Timer = new Timer();
            Console.WriteLine("StartingGiveaway");

            if (!inits.Timer.SetUpTimer(inits.GiveAwayTime, inits.GiveAwayDateTime, new Action(() =>
            {
                Task.Run(SetTimerValues);
            })))
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayErrorTimer"));
                return;
            }

            foreach (CultureInfo culture in _discordClient.CultureHelper.OutputCultures)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithDescription(GetGiveawayMessage(culture, inits, true));
                AddEndTimeField(inits, embed, true);

                await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());
            }

            await ReplyAsync(":tada: " + _discordClient.CultureHelper.GetAdminString("GiveawayStarted"));
        }

        public string GetGiveawayMessage(CultureInfo culture, GiveAwayValues inits, bool first)
        {
            string announce = first ? _discordClient.CultureHelper.GetOutputString("GiveawayAnnounce", culture) :
                                      _discordClient.CultureHelper.GetOutputString("GiveawayAnnounceNext", culture);
            return Environment.NewLine + announce + Environment.NewLine +
                   string.Format(_discordClient.CultureHelper.GetOutputString("GiveawayAnnounceWin", culture),
                                 inits.CultureAward[culture.Name]) + Environment.NewLine +
                   _discordClient.CultureHelper.GetOutputString("GiveawayAnnounceKeyword", culture) + " :tada:";
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
                        foreach (CultureInfo culture in _discordClient.CultureHelper.OutputCultures)
                        {
                            EmbedBuilder embed = new EmbedBuilder();
                            embed.WithDescription(GetGiveawayMessage(culture, inits, false));
                            AddEndTimeField(inits, embed, true);

                            await inits.GiveawayChannel.SendMessageAsync(string.Empty, false, embed.Build());
                        }

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

        internal GiveAwayValues GetCurrentInitValues()
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
