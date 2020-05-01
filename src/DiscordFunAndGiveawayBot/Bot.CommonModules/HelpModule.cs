using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Bot.CommonModules
{
    public class HelpModule : ModuleBase
    {
        [Command("help"), Summary("Shows help information")]
        public async Task Help()
        {
            string help = string.Format("Halleluja! :tada:{0}{0}" +
                "With this bot you can create awesome giveaways in specific channels." +
                "{0}And there are a lot of more fun actions you can do with me!" +
                "{0}Enter `%initgiveaway #[channelname]` to init a new giveaway" +
                "{0}Enter `%sendmessage [channelname]` to let the bot post a message in a specific channel" +
                "{0}{0}Find even more commands and additional information about this bot " +
                "at https://github.com/haep1/DiscordGiveawayBot.NetCore", Environment.NewLine);


            EmbedBuilder embed = new EmbedBuilder();
            embed.WithDescription(help);
            await ReplyAsync(string.Empty, false, embed.Build());
        }
    }
}
