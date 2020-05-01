using Bot.Interfaces;
using Discord;

namespace BotClient
{
    public class DiscordValues : IDiscordValues
    {
        public IGuild ServerGuild { get; set; }

        public IMessageChannel TargetChannel { get; set; }
        public IMessageChannel SourceChannel { get; set; }
        public IUser AdminUser { get; set; }

        public virtual void Reset()
        {
            AdminUser = null;
            TargetChannel = null;
            SourceChannel = null;
        }
    }
}
