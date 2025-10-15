using Discord;
using WOVSIM.Persistence.Domain.Models;

namespace WOVSIM.Bot.Models;

public class ContextModel
{
    public ContextModel(IInteractionContext context, User contextUser = null, IUser discordContextUser = null)
    {
        this.DiscordGuild = context.Guild;
        this.DiscordChannel = context.Channel;
        this.DiscordUser = discordContextUser ?? context.User;
        this.UserContext = contextUser;
        this.InteractionId = context.Interaction.Id;
    }
    
    public IGuild DiscordGuild { get; set; }
    public IChannel DiscordChannel { get; set; }
    public IUser DiscordUser { get; set; }
    
    public ulong InteractionId { get; set; }
    
    public User UserContext { get; set; }
}