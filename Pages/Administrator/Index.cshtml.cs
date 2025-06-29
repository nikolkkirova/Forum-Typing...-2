using Forum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Pages.Administrator
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ForumUser> _userManager;

        public IndexModel(UserManager<ForumUser> userManager)
        {
            _userManager = userManager;
        }

        public UserManager<ForumUser> UserManager => _userManager;
        public IList<ForumUser> Users { get; set; } = [];

        public async Task OnGetAsync() // когато се зарежда страницата
        {
            Users = await _userManager.Users.ToListAsync(); // зарежда се списък с всички потребители
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(string id) // активиране/деактивиране на профил
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleModeratorAsync(string id) // прави/премахва модератор
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var isModerator = await _userManager.IsInRoleAsync(user, "Moderator");

            if (isModerator)
            {
                await _userManager.RemoveFromRoleAsync(user, "Moderator");
            }

            else
            {
                await _userManager.AddToRoleAsync(user, "Moderator");
            }

            return RedirectToPage();

        }

        public async Task<IActionResult> OnPostToggleDeactivationAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsDeactivated = !user.IsDeactivated;
            await _userManager.UpdateAsync(user);
            return RedirectToPage();
        }
        
    }
}