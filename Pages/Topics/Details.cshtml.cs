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
    public class DetailsModel : PageModel
    {
        private readonly Forum.Data.ApplicationDbContext _context;
        // private readonly UserManager<ForumUser> _userManager;

        public DetailsModel(Forum.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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

            if (topic is not null)
            {
                Topic = topic;

                return Page();
            }

            return NotFound();
        }
    }
}
