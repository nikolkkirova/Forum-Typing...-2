using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;

namespace Forum.Pages.Topics
{
    public class DeleteModel : PageModel
    {
        private readonly Forum.Data.ApplicationDbContext _context;
        private readonly UserManager<ForumUser> _userManager;

        public DeleteModel(Forum.Data.ApplicationDbContext context, UserManager<ForumUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Topic Topic { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics
                .Include(t => t.User) // зарежда потребителя, който е създал темата
                .FirstOrDefaultAsync(m => m.Id == id);

            if (topic is null)
            {
                return NotFound();
            }

            Topic = topic;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Forbid();
            }

            var isModerator = await _userManager.IsInRoleAsync(user, "Moderator");

            if (topic.UserId != user.Id && !isModerator)
            {
                return Forbid(); // само собственик или модератор може да изтрива теми
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
