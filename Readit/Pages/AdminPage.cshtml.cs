using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Readit.Pages;
[Authorize(Roles = "admin")]
public class AdminPage : PageModel
{
    public void OnGet()
    {
        
    }
}