﻿using System;
using System.Globalization;
using UltraGiveawayBot;

namespace BotClient
{
    class GiveAwayFunctions
    {
        private IDiscordClient _discordClient;

        public GiveAwayFunctions(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public string SetGiveAwayTime(GiveAwayValues inits, string message)
        {
            string result = null;
            if (inits != null)
            {
                CultureInfo culture = _discordClient.CultureHelper.AdminCulture;
                DateTimeStyles styles = DateTimeStyles.None;

                if (TimeSpan.TryParse(message, out TimeSpan timespan))
                {
                    inits.GiveAwayTime = timespan;
                    inits.State = GiveAwayState.SetCountGiveAways;
                    result = string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"),
                                                   timespan.ToString("hh\\:mm"))
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayHowOften")
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayOftenExample");
                    return result;
                }
                else if (DateTime.TryParse(message, culture, styles, out DateTime dateTime))
                {
                    inits.GiveAwayDateTime = dateTime;
                    inits.State = GiveAwayState.SetCountGiveAways;
                    result = string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"),
                                                   dateTime.ToLongDateString())
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayHowOften")
                        + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawayOftenExample");
                    return result;
                }
            }

            result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongTime") + Environment.NewLine +
                        _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            return result;
        }

        public string SetCountGiveAways(GiveAwayValues inits, string message)
        {
            string result = null;
            if (inits != null && uint.TryParse(message, out uint count))
            {
                inits.CountGiveAways = count;
                inits.State = GiveAwayState.SetCodeword;
                result = _discordClient.CultureHelper.GetAdminString("GiveawayKeyword") + Environment.NewLine +
                   _discordClient.CultureHelper.GetAdminString("GiveawayKeywordExample");
            }
            else
            {
                result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongOften") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            }
            return result;
        }

        public string SetCodeword(GiveAwayValues inits, string message)
        {
            string result = null;
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.Codeword = message;
                inits.State = GiveAwayState.SetAward;

                result = SetAwardQuestion(inits);
            }
            else
            {
                result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongKeyword") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            }
            return result;
        }

        public string SetAwardQuestion(GiveAwayValues inits)
        {
            string result = null;
            foreach (CultureInfo culture in _discordClient.CultureHelper.OutputCultures)
            {
                if (!inits.CultureAward.ContainsKey(culture.Name))
                {
                    inits.CurrentAwardLanguage = culture.Name;
                    result = _discordClient.CultureHelper.GetAdminString("GiveawayPrice") + Environment.NewLine +
                             string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayPriceExample"),
                             culture.DisplayName, _discordClient.CultureHelper.GetOutputString("GiveawayExamplePrice", culture));
                }
            }

            return result;
        }

        public string SetAward(GiveAwayValues inits, string message)
        {
            string result = null;
            if (inits != null && !string.IsNullOrWhiteSpace(message))
            {
                inits.CultureAward.Add(inits.CurrentAwardLanguage, message);

                result = SetAwardQuestion(inits);
                if (result == null)
                {
                    inits.State = GiveAwayState.Initialized;

                    result = _discordClient.CultureHelper.GetAdminString("GiveawayInitialized") + Environment.NewLine +
                                       _discordClient.CultureHelper.GetAdminString("GiveawayEnterStart");
                }
            }
            else
            {
                result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongPrice") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            }
            return result;
        }
    }
}
