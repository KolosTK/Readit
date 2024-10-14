namespace Readit.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public string Author { get; set; }
    public List<int> GlobalRate { get; set; }
    public int Rate { get; set; }
    public Category Category { get; set; }
    public string Cover { get; set; }
    public string Summary { get; set; }
    public int PagesNumber { get; set; }
    public int YearOfPublishing { get; set; }
    public IEnumerator<Comment> Comments { get; set; }
}