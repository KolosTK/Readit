using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readit.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    [Required]
    public User User { get; set; }
    
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    [Required]
    public Book Book { get; set; }
    
    [Required]
    public string Text { get; set; }
}