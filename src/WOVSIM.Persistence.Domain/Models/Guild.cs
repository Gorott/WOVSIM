namespace WOVSIM.Persistence.Domain.Models;

public class Guild
{
    public int GuildId { get; set; }
    public ulong DiscordGuildId { get; set; }
    public string Name { get; set; }
    
    public int? MainGuildId { get; set; }
    public Guild MainGuild { get; set; }


    public Guild GameGuild { get; set; } 
    
    public bool IsGameGuild { get; set; }
}
