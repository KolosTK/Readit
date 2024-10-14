namespace Readit.Models;

public class Comment
{
    public int UserId { get; set; }
    public User Author { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public string Text { get; set; }
}