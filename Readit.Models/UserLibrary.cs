using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readit.Models;

public class UserLibrary
{
    [Key]
    public int Id { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    [Required]
    public User User { get; set; }
    
    public ICollection<UserBook> UserBooks { get; set; }
}