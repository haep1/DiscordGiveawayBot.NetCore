using Discord;

namespace Bot.BusinessObject
{
    public class DiscordValues
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
