﻿using Bot.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotClient;
using Scheduler;

namespace Bot.Giveaway
{
    public class GiveAway : ModuleBase
    {
        #region Static

        public static List<GiveAwayValues> InitValues { get; private set; }

        static GiveAway()
        {
            InitValues = new List<GiveAwayValues>();
        }

        #endregion

        #region Members & Constructor

        private Bot.Interfaces.IDiscordClient _discordClient;
        private GiveAwayFunctions _functions;


        public GiveAway(Bot.Interfaces.IDiscordClient client)
        {
            _discordClient = client;
            _functions = new GiveAwayFunctions(client);
        }

        #endregion

        #region Discord commands

        [Command("initgiveaway"), Summary("Initializes a new giveaway")]
        public async Task InitGiveAway()
        {
            string message = _discordClient.CultureHelper.GetAdminString("GiveawayMissingChannel") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayEnterChannelTip");
            await ReplyAsync(message, false, null);
        }

        [Command("initgiveaway"), Summary("Initializes a new giveaway")]
        public async Task InitGiveAway(IMessageChannel channel)
        {
            StopGiveawayInternal();
            string message = _discordClient.CultureHelper.GetAdminString("GiveawayStartNew") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayTypeCancel") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayEnterTime") + Environment.NewLine +
                             _discordClient.CultureHelper.GetAdminString("GiveawayTimeExample");
            GiveAwayValues values = new GiveAwayValues();
            values.Owner = this;
            values.ServerGuild = Context.Guild;
            values.TargetChannel = channel;
            values.SourceChannel = Context.Channel;
            values.AdminUser = Context.User;
            values.State = GiveAwayState.SetGiveAwayTime;
            values.CountGiveAways = 1;
            InitValues.Add(values);

            _discordClient.Client.MessageReceived -= Client_MessageReceived;
            _discordClient.Client.MessageReceived += Client_MessageReceived;

            await ReplyAsync(message, false, null);
        }


        [Command("start"), Summary("Starts the initialized giveaway")]
        public async Task Start()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null && inits.State == GiveAwayState.Initialized) {
                await StartGiveAway(inits);
            }
            else
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayNotInitialized")
                    + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayEnterChannelTip"));
            }
        }

        [Command("announcewinner"), Summary("Shouts out the winner")]
        public async Task AnnounceWinner()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            await ShoutOutTheWinners(inits);
        }

        [Command("cancel"), Summary("Cancels the Giveaway")]
        public async Task CancelGiveaway()
        {
            if (StopGiveawayInternal())
            {
                Console.WriteLine("Giveaway stopped");
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayStopped"));
            }
        }

        #endregion

        #region EventHandlers

        private async Task Client_MessageReceived(Discord.WebSocket.SocketMessage arg)
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                if (arg is SocketUserMessage userMessage &&
                    !userMessage.Author.IsBot &&
                    userMessage.Channel == inits.SourceChannel &&
                    userMessage.Author == inits.AdminUser && 
                    !userMessage.Content.ToLower().Equals("%cancel"))
                {
                    string message = null;
                    switch (inits.State)
                    {
                        case GiveAwayState.SetGiveAwayTime:
                            message = _functions.SetGiveAwayTime(inits, userMessage.Content);
                            await ReplyAsync(message);
                            break;
                        //case GiveAwayState.SetCountGiveAways:
                        //    message = _functions.SetCountGiveAways(inits, userMessage.Content);
                        //    await ReplyAsync(message);
                        //    break;
                        case GiveAwayState.SetAwardCultures:
                            message = _functions.SetAwardCultures(inits, userMessage.Content);
                            await ReplyAsync(message);
                            break;
                        case GiveAwayState.SetCodeword:
                            message = _functions.SetCodeword(inits, userMessage.Content);
                            await ReplyAsync(message);
                            break;
                        case GiveAwayState.SetAward:
                            message = _functions.SetAward(inits, userMessage.Content);
                            await ReplyAsync(message);
                            break;
                    }
                }

                if (inits.State == GiveAwayState.Initialized)
                {
                    _discordClient.Client.MessageReceived -= Client_MessageReceived;
                }
            }
        }

        #endregion

        #region Methods

        public async Task StartGiveAway(GiveAwayValues inits)
        {
            inits.Timer = new ScheduleManager();
            Console.WriteLine("StartingGiveaway");

            if (!inits.Timer.SetUpTimer(inits.GiveAwayTime, inits.GiveAwayDateTime, new Action(() =>
            {
                Task.Run(SetTimerValues);
            })))
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayErrorTimer"));
                return;
            }

            foreach (CultureInfo culture in inits.AwardCultures)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.WithDescription(GetGiveawayMessage(culture, inits, true));
                AddEndTimeField(inits, embed, culture);

                await inits.TargetChannel.SendMessageAsync(string.Empty, false, embed.Build());
            }

            await ReplyAsync(":tada: " + _discordClient.CultureHelper.GetAdminString("GiveawayStarted"));
        }

        public string GetGiveawayMessage(CultureInfo culture, GiveAwayValues inits, bool first)
        {
            string announce = first ? _discordClient.CultureHelper.GetOutputString("GiveawayAnnounce", culture) :
                                      _discordClient.CultureHelper.GetOutputString("GiveawayAnnounceNext", culture);
            return Environment.NewLine + announce + Environment.NewLine +
                   string.Format(_discordClient.CultureHelper.GetOutputString("GiveawayAnnounceWin", culture),
                                 inits.CultureAward[culture]) + Environment.NewLine +
                   string.Format(_discordClient.CultureHelper.GetOutputString("GiveawayAnnounceKeyword", culture),
                                 inits.Codeword) + " :tada:";
        }

        private void AddEndTimeField(GiveAwayValues inits, EmbedBuilder embed, CultureInfo culture)
        {
            if (inits.GiveAwayTime != null)
            {
                DateTime dt = new DateTime(1999, 1, 1, inits.GiveAwayTime.Value.Hours, inits.GiveAwayTime.Value.Minutes, 0);
                string time = dt.ToString("t", culture);

                embed.AddField(string.Format(_discordClient.CultureHelper.GetOutputString("GiveawayParticipEndTime", culture),
                               time), _discordClient.CultureHelper.GetOutputString("GoodLuck", culture));
            }
            else if (inits.GiveAwayDateTime != null)
            {
                DateTime dt = inits.GiveAwayDateTime.Value;
                string date = dt.ToString("g", culture);

                embed.AddField(string.Format(_discordClient.CultureHelper.GetOutputString("GiveawayParticipEndDate", culture),
                               date), _discordClient.CultureHelper.GetOutputString("GoodLuck", culture));
            }
        }

        private async Task SetTimerValues()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                if (await ShoutOutTheWinners(inits))
                {
                    try
                    {
                        // Wait one Minute to avoid problems with timespan
                        Thread.Sleep(60000);
                        foreach (CultureInfo culture in _discordClient.CultureHelper.OutputCultures)
                        {
                            EmbedBuilder embed = new EmbedBuilder();
                            embed.WithDescription(GetGiveawayMessage(culture, inits, false));
                            AddEndTimeField(inits, embed, culture);

                            await inits.TargetChannel.SendMessageAsync(string.Empty, false, embed.Build());
                        }

                        inits.Timer = new ScheduleManager();
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
                        await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayErrorRepeat") + ex.Message);
                    }
                }
            }
        }

        private void UnhandleMessageReceive()
        {
            _discordClient.Client.MessageReceived -= Client_MessageReceived;
        }

        private bool StopGiveawayInternal()
        {
            GiveAwayValues inits = GetCurrentInitValues();
            if (inits != null)
            {
                inits.Reset();
                (inits.Owner as GiveAway)?.UnhandleMessageReceive();
                InitValues.Remove(inits);
                return true;
            }
            return false;
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

        private async Task<bool> ShoutOutTheWinners(GiveAwayValues inits)
        {
            List<IReadOnlyCollection<IMessage>> readonlymessages = await inits.TargetChannel.GetMessagesAsync(1000).ToListAsync();
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

            List<IUser> users = latestMessages.Where(m => m.Content.ToLower().Contains(inits.Codeword.ToLower()))
                                      .Select(y => y.Author).Distinct().Where(x => !x.IsBot).ToList();
            List<IUser> winners = GetWinners(1, users);


            foreach (CultureInfo culture in inits.AwardCultures)
            {
                string message = _discordClient.CultureHelper.GetOutputString("GiveawayWinner", culture)
                                + Environment.NewLine + Environment.NewLine;

                foreach (IUser winner in winners)
                {
                    message += $":trophy: {winner.Mention} :trophy:" + Environment.NewLine + Environment.NewLine
                            + winner.Username + _discordClient.CultureHelper.GetOutputString("GiveawayWinnerPrize", culture)
                            + "**" + inits.CultureAward[culture] + "**" + Environment.NewLine;
                }
                message += _discordClient.CultureHelper.GetOutputString("GiveawayCongrats", culture) + Environment.NewLine + Environment.NewLine;

                await inits.TargetChannel.SendMessageAsync(message);
            }

            await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWinnerAnnounced"));

            inits.CountGiveAways--;
            if (inits.CountGiveAways == 0)
            {
                StopGiveawayInternal();
                return false;
            }
            return true;
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

        #endregion
    }
}
