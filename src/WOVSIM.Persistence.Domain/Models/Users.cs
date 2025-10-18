using System.ComponentModel.DataAnnotations;

namespace WOVSIM.Persistence.Domain.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required]
    public ulong DiscordUserId { get; set; }
    
    public Player Player { get; set; }
}