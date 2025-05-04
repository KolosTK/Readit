using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Models;
using Readit.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Readit.Pages;

[Authorize]
public class UserPage : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    public UserPage(UserManager<User> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public string Username { get; set; } = "";
    public List<UserBook> Books { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        Username = user.FirstName!;
        Books = await _context.UserBooks
            .Where(b => b.UserId == user.Id)
            .ToListAsync();
    }
}