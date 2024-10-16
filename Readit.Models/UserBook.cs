using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readit.Models.Enums;
namespace Readit.Models;

public class UserBook
{
    [Key]
    public int Id { get; set; }
    
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    [Required]
    public Book Book { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    [Required]
    public User User { get; set; }
    
    [Required]
    public Status Status { get; set; }
    
    public int? Rate { get; set; }
    
    [Required]
    public DateTime DateAdded { get; set; }
    
    public DateTime? DateModified { get; set; }
}