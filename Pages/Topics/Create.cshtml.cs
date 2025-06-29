using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.Pages.Topics
{
    public class CreateModel : PageModel
    {
        private readonly Forum.Data.ApplicationDbContext _context;
        private readonly UserManager<ForumUser> _userManager;

        public CreateModel(Forum.Data.ApplicationDbContext context, UserManager<ForumUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Topic.UserId = user.Id;
            Topic.CreatedAt = DateTime.UtcNow;

            _context.Topics.Add(Topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
