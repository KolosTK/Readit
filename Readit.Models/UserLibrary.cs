namespace Readit.Models;

public class UserLibrary
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public IEnumerable<UserBook> Books { get; set; }
}