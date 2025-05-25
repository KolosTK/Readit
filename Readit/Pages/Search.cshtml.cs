using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Readit.Api.Models;
using Readit.DataAccess;
using Readit.Library;
using Readit.Models;
using Readit.Services;

namespace Readit.Pages;
[IgnoreAntiforgeryToken]
public class Search : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly BookApiService _bookApiService;
    private readonly LibraryService _libraryService;
    [BindProperty(SupportsGet = true)]
    public string Mode { get; set; } = "books";

    [BindProperty(SupportsGet = true)]
    public string Query { get; set; } = "";
    public List<User> Users { get; set; } = new();
    public List<string> FriendIds { get; set; } = new();
    private readonly UserManager<User> _userManager;
    public List<OpenLibraryBook> Books { get; set; } = new();
    public Search(BookApiService bookApiService, LibraryService libraryService,ApplicationDbContext context, UserManager<User> userManager)
    {
        _bookApiService = bookApiService;
        _libraryService = libraryService; 
        _context = context;
        _userManager = userManager;
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (Mode == "books" && !string.IsNullOrWhiteSpace(Query))
        {
            Books = await _bookApiService.SearchBooksAsync(Query,12,0);
            var userBooks = await _libraryService.GetUserBooksAsync();
            var userBookKeys = userBooks.Select(b => b.WorkKey).ToHashSet();

            foreach (var book in Books)
                book.IsInLibrary = userBookKeys.Contains(book.Key);
        }
        else if (Mode == "friends" && !string.IsNullOrWhiteSpace(Query))
        {
            var normalizedQuery = Query.ToLower();

            Users = await _context.Users
                .Where(u =>
                    (u.FirstName + " " + u.LastName).ToLower().Contains(normalizedQuery) ||
                    u.FirstName.ToLower().Contains(normalizedQuery) ||
                    u.LastName.ToLower().Contains(normalizedQuery))
                .ToListAsync();
        }

        return Page();
    }

  public async Task<IActionResult> OnGetAsync()
{
    var currentUser = await _userManager.GetUserAsync(User);

    if (Mode == "books" && !string.IsNullOrWhiteSpace(Query))
    {
        Books = await _bookApiService.SearchBooksAsync(Query);
        if (currentUser != null)
        {
            var userBooks = await _libraryService.GetUserBooksAsync();
            var userBookKeys = userBooks.Select(b => b.WorkKey).ToHashSet();

            foreach (var book in Books)
                book.IsInLibrary = userBookKeys.Contains(book.Key);
        }
    }
    else if (Mode == "friends" && !string.IsNullOrWhiteSpace(Query))
    {
        var normalizedQuery = Query.ToLower();

        Users = await _context.Users
            .Where(u =>
                (u.FirstName + " " + u.LastName).ToLower().Contains(normalizedQuery) ||
                u.FirstName.ToLower().Contains(normalizedQuery) ||
                u.LastName.ToLower().Contains(normalizedQuery))
            .ToListAsync();
    }
    else if (Mode == "recommendations")
    {
        if (currentUser == null) return Page();

        FriendIds = await _context.Friendships
            .Where(f => f.FollowerId == currentUser.Id)
            .Select(f => f.FolloweeId)
            .ToListAsync();

        var friendBooks = await _context.UserBooks
            .Where(ub => FriendIds.Contains(ub.UserId))
            .Include(ub => ub.User)
            .ToListAsync();

        Books = friendBooks
            .GroupBy(ub => ub.WorkKey)
            .Select(group => new OpenLibraryBook
            {
                Key = group.Key,
                Title = group.First().Title,
                AuthorName = group.First().Authors?.Split(", ").ToList(),
                CoverId = group.First().CoverId,
                AddedByUsers = group.Select(ub => ub.User).ToList()
            })
            .OrderByDescending(b => b.AddedByUsers.Count) // ✅ Sort by number of friends who added this book
            .ToList();

    }

    if (currentUser != null)
    {
        FriendIds = await _context.Friendships
            .Where(f => f.FollowerId == currentUser.Id)
            .Select(f => f.FolloweeId)
            .ToListAsync();
    }

    return Page();
}



    public async Task<IActionResult> OnGetMoreAsync(string query, int offset)
    {
        var books = await _bookApiService.SearchBooksAsync(query, 12, offset);
        return Partial("_BookCardsPartial", books);
    }
    public async Task<IActionResult> OnPostToggleAsync([FromBody] OpenLibraryBook book)
    {
        var added = await _libraryService.ToggleBookAsync(book);
        return new JsonResult(new { added });
    }
    
    public async Task<IActionResult> OnPostAddFriendAsync(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || id == currentUser.Id) return RedirectToPage();

        if (!_context.Friendships.Any(f => f.FollowerId == currentUser.Id && f.FolloweeId == id))
        {
            _context.Friendships.Add(new Friendship { FollowerId = currentUser.Id, FolloweeId = id });
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { Mode, Query });
    }

    public async Task<IActionResult> OnPostRemoveFriendAsync(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(f => f.FollowerId == currentUser.Id && f.FolloweeId == id);

        if (friendship != null)
        {
            _context.Friendships.Remove(friendship);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { Mode, Query });
    }

}