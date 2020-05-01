using Discord;

namespace Bot.Interfaces
{
    public partial interface IDiscordValues
    {
        void Reset();

        IUser AdminUser { get; set; }
        IGuild ServerGuild { get; set; }
        IMessageChannel SourceChannel { get; set; }
        IMessageChannel TargetChannel { get; set; }
    }
}
