using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Api.Models;
using Readit.Services;

namespace Readit.Pages;

public class BookDetails : PageModel
{
    private readonly BookApiService _bookApiService;

    public BookDetails(BookApiService bookApiService)
    {
        _bookApiService = bookApiService;
    }

    public OpenLibraryBook Book { get; set; }

    public async Task<IActionResult> OnGetAsync(int coverId)
    {
        var books = await _bookApiService.SearchBooksAsync("harry potter"); // test only
        Book = books.FirstOrDefault(b => b.CoverId == coverId || coverId == 0);
        if (Book == null) return NotFound();

        return Page();
    }
}