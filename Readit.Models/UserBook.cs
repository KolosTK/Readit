namespace Readit.Models;

public class UserBook
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public int? CoverId { get; set; }
    public string WorkKey { get; set; } = null!;

    public User User { get; set; } = null!;
}