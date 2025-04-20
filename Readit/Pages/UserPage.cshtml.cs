using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Readit.Pages;
[Authorize(Roles = "user")]
public class UserPage : PageModel
{
    public void OnGet()
    {
        
    }
}