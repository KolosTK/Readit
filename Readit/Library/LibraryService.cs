using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Readit.Api.Models;
using Readit.DataAccess;
using Readit.Models;

namespace Readit.Library;

public class LibraryService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _http;

    public LibraryService(ApplicationDbContext context, UserManager<User> userManager, IHttpContextAccessor http)
    {
        _context = context;
        _userManager = userManager;
        _http = http;
    }

    public async Task<bool> ToggleBookAsync(OpenLibraryBook book)
    {
        var user = await _userManager.GetUserAsync(_http.HttpContext!.User);
        if (user == null) return false;

        var existing = await _context.UserBooks.FirstOrDefaultAsync(b =>
            b.UserId == user.Id && b.WorkKey == book.Key);

        if (existing != null)
        {
            _context.UserBooks.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }

        var userBook = new UserBook
        {
            UserId = user.Id,
            Title = book.Title ?? "Untitled",
            Authors = book.AuthorName != null ? string.Join(", ", book.AuthorName) : null,
            /*CoverId = book.CoverId.HasValue && book.CoverId > 0 ? book.CoverId : null,*/
            CoverId = book.CoverId,
            WorkKey = book.Key ?? ""
        };
        Console.WriteLine("📥 Received book from client:");
        Console.WriteLine($"Title: {book.Title}");
        Console.WriteLine($"CoverId: {book.CoverId}");
        Console.WriteLine($"Key: {book.Key}");
        
        _context.UserBooks.Add(userBook);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserBook>> GetUserBooksAsync()
    {
        var user = await _userManager.GetUserAsync(_http.HttpContext!.User);
        return await _context.UserBooks
            .Where(b => b.UserId == user!.Id)
            .ToListAsync();
    }
}
