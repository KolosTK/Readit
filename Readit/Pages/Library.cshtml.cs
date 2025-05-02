using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Library;
using Readit.Models;
using static Readit.Library.LibraryService;

namespace Readit.Pages;

[Authorize]
public class Library : PageModel
{
    private readonly LibraryService _libraryService;

    public Library(LibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public List<UserBook> Books { get; set; } = new();

    public async Task OnGetAsync()
    {
        Books = await _libraryService.GetUserBooksAsync();
    }
}