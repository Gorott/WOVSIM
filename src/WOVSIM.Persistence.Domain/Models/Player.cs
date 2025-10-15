namespace WOVSIM.Persistence.Domain.Models;

public class Player
{
    public int PlayerId { get; set; }
    public ulong DiscordUserId { get; set; }
    public bool Alive { get; set; }
}