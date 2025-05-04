namespace Readit.Models;

public class Comment
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;
    public string WorkKey { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
