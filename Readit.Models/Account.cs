namespace Readit.Models;

public class Account
{
    public User UserId { get; set; }
    public User User { get; set; }
    public string Avatar { get; set; }
    public string UserName { get; set; }
    public IEnumerable<User> Following { get; set; }
    public IEnumerable<User> Followers { get; set; }
}