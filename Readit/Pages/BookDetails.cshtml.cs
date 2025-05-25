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

    [BindProperty]
    public string WorkKey { get; set; } = null!;
    
    [BindProperty]
    public int? EditCommentId { get; set; }

    [BindProperty]
    public string? EditedCommentText { get; set; }

    [BindProperty]
    public string? NewCommentText { get; set; }

    public ReadingStatus? UserBookStatus { get; set; }
    public OpenLibraryBook Book { get; set; } = null!;
    public List<Comment> Comments { get; set; } = new();
    public int? UserRating { get; set; }

    public BookDetails(BookApiService bookApiService, ApplicationDbContext context, UserManager<User> userManager)
    {
        _bookApiService = bookApiService;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string workKey)
    {
        var book = await _bookApiService.GetBookDetailsByKeyAsync(workKey);
        if (book == null) return NotFound();

        Book = book;
        WorkKey = workKey;

        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var userBook = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.WorkKey.EndsWith(workKey));

            if (userBook != null)
            {
                UserBookStatus = userBook.Status;
                UserRating = userBook.Rating;
            }
        }

        Comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.WorkKey == workKey)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddCommentAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || string.IsNullOrWhiteSpace(NewCommentText))
            return RedirectToPage(new { workKey = WorkKey });

        var comment = new Comment
        {
            Text = NewCommentText!,
            UserId = user.Id,
            WorkKey = WorkKey,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { workKey = WorkKey });
    }

    public async Task<IActionResult> OnPostEditCommentAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || EditCommentId == null || string.IsNullOrWhiteSpace(EditedCommentText))
            return RedirectToPage(new { workKey = WorkKey });

        var comment = await _context.Comments.FindAsync(EditCommentId);
        if (comment == null || comment.UserId != user.Id)
            return Forbid();

        comment.Text = EditedCommentText!;
        await _context.SaveChangesAsync();

        return RedirectToPage(new { workKey = WorkKey });
    }

    public async Task<IActionResult> OnPostEditCommentFormAsync(int id)
    {
        var comment = await _context.Comments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null || comment.UserId != user.Id) return Forbid();

        WorkKey = comment.WorkKey;
        EditCommentId = id;
        EditedCommentText = comment.Text;

        await OnGetAsync(comment.WorkKey);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteCommentAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var comment = await _context.Comments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null) return NotFound();

        var isAuthor = comment.UserId == user.Id;
        var isAdmin = await _userManager.IsInRoleAsync(user, "admin");

        if (!isAuthor && !isAdmin) return Forbid();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { workKey = comment.WorkKey });
    }

    public async Task<IActionResult> OnPostRateAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var formWorkKey = Request.Form["WorkKey"];
        var formRating = Request.Form["SelectedRating"];

        if (!int.TryParse(formRating, out int rating) || rating < 1 || rating > 5)
            return BadRequest();

        var userBook = await _context.UserBooks
            .FirstOrDefaultAsync(ub => ub.UserId == user.Id && ub.WorkKey.EndsWith(formWorkKey));

        if (userBook == null)
            return NotFound();

        userBook.Rating = rating;
        await _context.SaveChangesAsync();

        return RedirectToPage(new { workKey = formWorkKey });
    }


    public class RatingRequest
    {
        public string WorkKey { get; set; } = null!;
        public int SelectedRating { get; set; }
    }
}