using System.Collections.Generic;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Persistence.Domain.Models;

public class GameSession
{
    public int GameSessionId { get; set; }
    public int StarterUserId { get; set; }
    
    public ulong? DiscordGuildId { get; set; }
    public ulong? DiscordId { get; set; }
    public ulong? GameInformationResponseId { get; set; }
    public string Name { get; set; }
    
    public GameStatus Status { get; set; }
    public GameState? State { get; set; } 
    
    public ICollection<Player> Players { get; set; } 
}