using System.ComponentModel.DataAnnotations;

namespace Readit.Models;

public class Book
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string ISBN { get; set; }
    
    [Required]
    public string Author { get; set; }
    
    public int? GlobalRate { get; set; }
    public int? Rate { get; set; }
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public string Cover { get; set; }
    public string Summary { get; set; }
    public int PagesNumber { get; set; }
    
    [MaxLength(2024)]
    public int YearOfPublishing { get; set; }
}