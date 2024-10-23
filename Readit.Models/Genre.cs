using System.ComponentModel.DataAnnotations;

namespace Readit.Models;

public class Genre
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    
    public virtual ICollection<BookGenre> BookGenres { get; set; }
}