using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WOVSIM.Persistence.Domain.Models;

public class Player
{
    [Key]
    public int PlayerId { get; set; }
    
    public int GameSessionId { get; set; }
    [ForeignKey(nameof(GameSessionId))]
    public GameSession GameSession { get; set; }
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }

    public bool Alive { get; set; }
}