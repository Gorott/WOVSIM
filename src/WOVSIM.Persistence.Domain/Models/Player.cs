namespace WOVSIM.Persistence.Domain.Models;

public class Player
{
    public int PlayerId { get; set; }
    public int UserId { get; set; }
    public bool Alive { get; set; }
    
    public User User { get; set; }
}