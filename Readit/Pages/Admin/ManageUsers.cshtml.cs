using Microsoft.AspNetCore.Mvc.RazorPages;
using Readit.Models;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Readit.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public ManageUsersModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public List<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = _userManager.Users.ToList();
        }

        public async Task<IActionResult> OnPostPromoteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "admin");
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDemoteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "admin");
            }
            return RedirectToPage();
        }
    }
}
