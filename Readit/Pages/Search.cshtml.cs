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
    [BindProperty]
    public string Mode { get; set; } = "books";
    public List<User> Users { get; set; } = new();
    public Search(BookApiService bookApiService, LibraryService libraryService,ApplicationDbContext context)
    {
        _bookApiService = bookApiService;
        _libraryService = libraryService; 
        _context = context;
    }

    public List<OpenLibraryBook> Books { get; set; } = new();

    [BindProperty]
    public string Query { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Mode == "books" && !string.IsNullOrWhiteSpace(Query))
        {
            Books = await _bookApiService.SearchBooksAsync(Query);
            if (User.Identity?.IsAuthenticated == true)
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

        return Page();
    }



    public async Task<IActionResult> OnGetMoreAsync(string query, int offset)
    {
        var books = await _bookApiService.SearchBooksAsync(query, 12, offset);
        return Partial("_BookCardsPartial", books);
    }
    public async Task<IActionResult> OnPostToggleAsync([FromBody] OpenLibraryBook book)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return new JsonResult(new { added = false, unauthorized = true });
        }

        var added = await _libraryService.ToggleBookAsync(book);
        return new JsonResult(new { added });
    }

}