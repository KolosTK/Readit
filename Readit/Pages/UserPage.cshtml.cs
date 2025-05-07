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

    public string Username { get; set; } = "";
    public List<UserBook> Books { get; set; } = new();

    public UserPage(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        Username = user.FirstName!;
        Books = await _context.UserBooks
            .Where(b => b.UserId == user.Id)
            .ToListAsync();
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
}
