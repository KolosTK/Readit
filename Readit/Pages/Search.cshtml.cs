using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Api.Models;
using Readit.Library;
using Readit.Services;

namespace Readit.Pages;
[IgnoreAntiforgeryToken]
public class Search : PageModel
{
    private readonly BookApiService _bookApiService;
    private readonly LibraryService _libraryService;

    public Search(BookApiService bookApiService, LibraryService libraryService)
    {
        _bookApiService = bookApiService;
        _libraryService = libraryService; 
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
    public async Task<IActionResult> OnPostToggleAsync([FromBody] OpenLibraryBook book)
    {
        var added = await _libraryService.ToggleBookAsync(book);
        return new JsonResult(new { added });
    }

}