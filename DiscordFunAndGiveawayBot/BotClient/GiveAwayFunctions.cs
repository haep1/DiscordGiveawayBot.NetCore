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
                    await ReplyAsync(string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"), 
                                                   timespan.ToString("hh\\:mm"))
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayHowOften") 
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayOftenExample"), false, null);
                    return;
                }
                else if (DateTime.TryParse(message, culture, styles, out DateTime dateTime))
                {
                    inits.GiveAwayDateTime = dateTime;
                    inits.State = GiveAwayState.SetCountGiveAways;
                    await ReplyAsync(string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"),
                                                   dateTime.ToLongDateString())
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayHowOften")
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayOftenExample"), false, null);
                    return;
                }
            }

            await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWrongTime") + Environment.NewLine +
                        _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain"), false, null);
        }

        public async Task SetCountGiveAways(GiveAwayValues inits, string message)
        {
            if (inits != null && uint.TryParse(message, out uint count))
            {
                inits.CountGiveAways = count;
                inits.State = GiveAwayState.SetCodeword;
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayKeyword") + Environment.NewLine +
                   _discordClient.CultureHelper.GetAdminString("GiveawayKeywordExample"), false, null);
            }
            else
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWrongOften") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain"), false, null);
            }
        }

        public async Task SetCodeword(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.Codeword = message;
                inits.State = GiveAwayState.SetAwardGerman;

                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayPrice") + Environment.NewLine +
                                 _discordClient.CultureHelper.GetAdminString("GiveawayPriceExample"), false, null);
            }
            else
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWrongKeyword") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain"), false, null);
            }
        }

        public async Task SetAwardGerman(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.CultureAward.Add("de-DE", message);
                inits.State = GiveAwayState.SetAwardEnglish;

                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayPriceEnglish") + Environment.NewLine +
                                  _discordClient.CultureHelper.GetAdminString("GiveawayPriceEnglishExample"), false, null);
            }
            else
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWrongPrice") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain"), false, null);
            }
        }

        public async Task SetAwardEnglish(GiveAwayValues inits, string message)
        {
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.CultureAward.Add("en-US", message);
                inits.State = GiveAwayState.Initialized;

                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayInitialized") + Environment.NewLine +
                                  _discordClient.CultureHelper.GetAdminString("GiveawayEnterStart"), false, null);
            }
            else
            {
                await ReplyAsync(_discordClient.CultureHelper.GetAdminString("GiveawayWrongPriceEnglish") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayEnterStart"), false, null);
            }
        }
    }
}
