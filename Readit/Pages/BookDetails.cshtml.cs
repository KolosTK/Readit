using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Readit.Api.Models;
using Readit.DataAccess;
using Readit.Models;
using Readit.Services;

namespace Readit.Pages;

public class BookDetails : PageModel
{
    private readonly BookApiService _bookApiService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    public ReadingStatus? UserBookStatus { get; set; }

    public BookDetails(BookApiService bookApiService, ApplicationDbContext context, UserManager<User> userManager)
    {
        _bookApiService = bookApiService;
        _context = context;
        _userManager = userManager;
    }

    public OpenLibraryBook Book { get; set; } = null!;
    public List<Comment> Comments { get; set; } = new();

    [BindProperty]
    public string? NewCommentText { get; set; }

    public async Task<IActionResult> OnGetAsync(string workKey)
    {
        var book = await _bookApiService.GetBookDetailsByKeyAsync(workKey);
        if (book == null) return NotFound();

        Book = book;

        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var userBook = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.WorkKey.EndsWith(workKey));

            if (userBook != null)
            {
                UserBookStatus = userBook.Status;
            }
        }

        Comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.WorkKey == workKey)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Page();
    }

   

    public async Task<IActionResult> OnPostAddCommentAsync(int coverId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || string.IsNullOrWhiteSpace(NewCommentText))
            return RedirectToPage(new { coverId });

        var books = await _bookApiService.SearchBooksAsync("harry potter"); // same temp logic
        Book = books.FirstOrDefault(b => b.CoverId == coverId || coverId == 0);
        if (Book == null || string.IsNullOrEmpty(Book.Key)) return NotFound();

        var comment = new Comment
        {
            UserId = user.Id,
            WorkKey = Book.Key,
            Text = NewCommentText!,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { coverId });
    }
}
