using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Models;
using Readit.DataAccess;
using Microsoft.EntityFrameworkCore;
using Readit.Services;

namespace Readit.Pages;
[Authorize]
public class UserPage : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }

    public User CurrentUser { get; set; } = null!;
    public bool IsFollowing { get; set; }
    public string Username { get; set; } = "";
    public List<UserBook> Books { get; set; } = new();
    public bool IsSelf { get; set; }

    public UserPage(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        var loggedInUser = await _userManager.GetUserAsync(User);

        // if id is not passed, use current user
        var targetUserId = id ?? loggedInUser?.Id;
        if (targetUserId == null) return NotFound();

        CurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetUserId);
        if (CurrentUser == null) return NotFound();

        IsSelf = loggedInUser != null && loggedInUser.Id == CurrentUser.Id;

        if (!IsSelf && loggedInUser != null)
        {
            IsFollowing = await _context.Friendships
                .AnyAsync(f => f.FollowerId == loggedInUser.Id && f.FolloweeId == CurrentUser.Id);
        }
        
        Username = CurrentUser.FirstName!;
        Books = await _context.UserBooks
            .Where(b => b.UserId == CurrentUser.Id)
            .ToListAsync();

        FollowersCount = await _context.Friendships
            .CountAsync(f => f.FolloweeId == CurrentUser.Id);

        FollowingCount = await _context.Friendships
            .CountAsync(f => f.FollowerId == CurrentUser.Id);

        return Page();
    }



    public class UpdateStatusRequest
    {
        public string Key { get; set; }
        public ReadingStatus Status { get; set; }
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync([FromBody] UpdateStatusRequest data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return new JsonResult(new { success = false });

        var book = await _context.UserBooks
            .FirstOrDefaultAsync(b => b.UserId == user.Id && b.WorkKey == data.Key);

        if (book == null) return new JsonResult(new { success = false });

        book.Status = data.Status;
        await _context.SaveChangesAsync();
        return new JsonResult(new { success = true });
    }
    
    [BindProperty]
    public IFormFile? AvatarUpload { get; set; }

    public async Task<IActionResult> OnPostUploadAvatarAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || AvatarUpload == null || AvatarUpload.Length == 0)
            return RedirectToPage();

        var fileName = $"{user.Id}{Path.GetExtension(AvatarUpload.FileName)}";
        var savePath = Path.Combine("wwwroot", "avatars", fileName);

        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await AvatarUpload.CopyToAsync(stream);
        }

        user.AvatarFileName = fileName;
        await _userManager.UpdateAsync(user);

        // ✅ Redirect with id so avatar updates on reload
        return RedirectToPage(new { id = user.Id });
    }


    public async Task<IActionResult> OnPostFollowAsync(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.Id == id) return RedirectToPage(new { id });

        bool alreadyFollowing = await _context.Friendships
            .AnyAsync(f => f.FollowerId == currentUser.Id && f.FolloweeId == id);

        if (!alreadyFollowing)
        {
            _context.Friendships.Add(new Friendship
            {
                FollowerId = currentUser.Id,
                FolloweeId = id
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnfollowAsync(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.Id == id) return RedirectToPage(new { id });

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.FollowerId == currentUser.Id && f.FolloweeId == id);

        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

}
