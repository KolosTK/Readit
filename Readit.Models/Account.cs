using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readit.Models;

public class Account
{
    [Key]
    public int Id { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    [Required]
    public User User { get; set; }
    
    public string? Avatar { get; set; }
    public string UserName { get; set; }
    public ICollection<User> Following { get; set; }
    public ICollection<User> Followers { get; set; }
}