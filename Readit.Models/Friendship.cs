namespace Readit.Models;

public class Friendship
{
    public int Id { get; set; }

    public string FollowerId { get; set; } = null!;
    public User Follower { get; set; } = null!;

    public string FolloweeId { get; set; } = null!;
    public User Followee { get; set; } = null!;
}