using Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BotClient
{
    class GiveAwayFunctions
    {
        private IDiscordClient _discordClient;

        public GiveAwayFunctions(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public string SetGiveAwayTime(IGiveAwayValues inits, string message)
        {
            if (inits != null)
            {
                string nextQuestion = GetCultureQuestion();

                if (TimeSpan.TryParse(message, out TimeSpan timespan))
                {
                    inits.GiveAwayTime = timespan;
                    inits.State = GiveAwayState.SetAwardCultures;
                    return string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"),
                                                   timespan.ToString("hh\\:mm")) + Environment.NewLine + nextQuestion;
                }
                else
                {
                    CultureInfo culture = _discordClient.CultureHelper.AdminCulture;
                    DateTimeStyles styles = DateTimeStyles.None;
                    if (DateTime.TryParse(message, culture, styles, out DateTime dateTime))
                    {
                        inits.GiveAwayDateTime = dateTime;
                        inits.State = GiveAwayState.SetAwardCultures;
                        return string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayTimeResponse"),
                                                       dateTime.ToLongDateString()) + Environment.NewLine + nextQuestion;
                    }
                }
            }

            return _discordClient.CultureHelper.GetAdminString("GiveawayWrongTime") + Environment.NewLine +
                        _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
        }

        private string GetCultureQuestion()
        {
            string supportedOutputLanguages = string.Join(", ",
                                                          _discordClient.CultureHelper.OutputCultures.Select(x => x.Name));

            string nextQuestion = _discordClient.CultureHelper.GetAdminString("GiveawayAwardCultures")
                    + supportedOutputLanguages
                    + Environment.NewLine + _discordClient.CultureHelper.GetAdminString("GiveawyAwardCulturesExample");
            return nextQuestion;
        }

        public string SetAwardCultures(IGiveAwayValues inits, string message)
        {
            string[] separators = { "," };
            string[] cultureNames = message.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            bool foundCulture = false;
            List<CultureInfo> awardCultures = new List<CultureInfo>();

            foreach (string item in cultureNames)
            {
                try
                {
                    CultureInfo info = CultureInfo.GetCultureInfo(item);
                    awardCultures.Add(info);
                    foundCulture = true;
                }
                catch (Exception)
                { }
            }
            if (!foundCulture)
            {
                return _discordClient.CultureHelper.GetAdminString("GiveawayWrongCulture") + Environment.NewLine +
                           _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            }
            inits.AwardCultures = awardCultures;
            inits.State = GiveAwayState.SetCodeword;
            return _discordClient.CultureHelper.GetAdminString("GiveawayKeyword") + Environment.NewLine +
               _discordClient.CultureHelper.GetAdminString("GiveawayKeywordExample");
        }

        //public string SetCountGiveAways(GiveAwayValues inits, string message)
        //{
        //    string result = null;
        //    if (inits != null && uint.TryParse(message, out uint count))
        //    {
        //        inits.CountGiveAways = count;
        //        inits.State = GiveAwayState.SetCodeword;
        //        result = _discordClient.CultureHelper.GetAdminString("GiveawayKeyword") + Environment.NewLine +
        //           _discordClient.CultureHelper.GetAdminString("GiveawayKeywordExample");
        //    }
        //    else
        //    {
        //        result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongOften") + Environment.NewLine +
        //                    _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
        //    }
        //    return result;
        //}

        public string SetCodeword(IGiveAwayValues inits, string message)
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

        public string SetAwardQuestion(IGiveAwayValues inits)
        {
            string result = null;
            foreach (CultureInfo culture in inits.AwardCultures)
            {
                if (!inits.CultureAward.ContainsKey(culture))
                {
                    inits.CurrentAwardLanguage = culture;
                    result = _discordClient.CultureHelper.GetAdminString("GiveawayPrize") + Environment.NewLine +
                             string.Format(_discordClient.CultureHelper.GetAdminString("GiveawayPrizeExample"),
                             culture.DisplayName, _discordClient.CultureHelper.GetOutputString("GiveawayExamplePrize", culture));
                }
            }

            return result;
        }

        public string SetAward(IGiveAwayValues inits, string message)
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
                result = _discordClient.CultureHelper.GetAdminString("GiveawayWrongPrize") + Environment.NewLine +
                            _discordClient.CultureHelper.GetAdminString("GiveawayTryAgain");
            }
            return result;
        }
    }
}
