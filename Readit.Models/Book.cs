using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Readit.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(13)]
    public string ISBN { get; set; }
    
    [Required]
    public string Author { get; set; }
    [MaxLength(10)]
    public int? GlobalRate { get; set; }
    public string Cover { get; set; }
    public string? Summary { get; set; }
    public int? PagesNumber { get; set; }
    
    public virtual ICollection<BookGenre> BookGenres { get; set; }
    
    [MaxLength(2024)]
    public int? YearOfPublishing { get; set; }

    public Book()
    {
        Cover = "cover.jpg";
    }
}