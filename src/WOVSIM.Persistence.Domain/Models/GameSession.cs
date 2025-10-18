using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Persistence.Domain.Models;

public class GameSession
{
    [Key]
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