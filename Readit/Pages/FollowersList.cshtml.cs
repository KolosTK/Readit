using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Readit.DataAccess;
using Readit.Models;

namespace Readit.Pages;

public class FollowersListModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public FollowersListModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<User> SortedUsers { get; set; } = new();
    public string UserName { get; set; } = "";
    public List<string> FollowingIds { get; set; } = new();
    public string CurrentUserId { get; set; } = "";
    public string Order { get; set; } = "following";


    public List<string> FollowerIds { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? id, string order = "following")
    {
        CurrentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
        Order = order;

        var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ??
                         await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (targetUser == null) return NotFound();

        UserName = targetUser.FirstName;

        // Get following and follower IDs
        FollowingIds = await _context.Friendships
            .Where(f => f.FollowerId == targetUser.Id)
            .Select(f => f.FolloweeId)
            .ToListAsync();

        FollowerIds = await _context.Friendships
            .Where(f => f.FolloweeId == targetUser.Id)
            .Select(f => f.FollowerId)
            .ToListAsync();

        // Combine and preserve order
        var userIds = (order == "followers"
                ? FollowerIds.Concat(FollowingIds.Except(FollowerIds))
                : FollowingIds.Concat(FollowerIds.Except(FollowingIds)))
            .Distinct()
            .ToList();

        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(); // get from DB first

        SortedUsers = users
            .OrderBy(u => userIds.IndexOf(u.Id))
            .ToList();
        
        return Page();
    }
    public async Task<IActionResult> OnPostAddFriendAsync(string id)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != null && currentUserId != id)
        {
            if (!_context.Friendships.Any(f => f.FollowerId == currentUserId && f.FolloweeId == id))
            {
                _context.Friendships.Add(new Friendship { FollowerId = currentUserId, FolloweeId = id });
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToPage(new { id = currentUserId });
    }

    public async Task<IActionResult> OnPostRemoveFriendAsync(string id)
    {
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FolloweeId == id);
    
        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id = currentUserId });
    }

}