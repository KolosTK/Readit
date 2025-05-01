using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Api.Models;
using Readit.Services;

namespace Readit.Pages;
[IgnoreAntiforgeryToken]
public class Search : PageModel
{
    private readonly BookApiService _bookApiService;

    public Search(BookApiService bookApiService)
    {
        _bookApiService = bookApiService;
    }

    public List<OpenLibraryBook> Books { get; set; } = new();

    [BindProperty]
    public string Query { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(Query))
        {
            Books = await _bookApiService.SearchBooksAsync(Query);
        }
        Console.WriteLine($"Book count: {Books.Count}");
        return Page();
    }
    public async Task<IActionResult> OnGetMoreAsync(string query, int offset)
    {
        var books = await _bookApiService.SearchBooksAsync(query, 12, offset);
        return Partial("_BookCardsPartial", books);
    }

    /*public IActionResult OnGetDetails(int coverId)
    {
        var book = Books.FirstOrDefault(b => b.CoverId == coverId);
        if (book == null) return NotFound();

        return Partial("_BookDetailsPartial", book);
    }*/
}