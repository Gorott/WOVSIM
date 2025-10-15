using System.Collections.Generic;
using WOVSIM.Domain.Enums;

namespace WOVSIM.Persistence.Domain.Models;

public class Game
{
    public int GameId { get; set; }
    public string Name { get; set; }
    
    public GameStatus Status { get; set; }
    public GameState? State { get; set; } 
    
    public ICollection<Player> Players { get; set; } 
}