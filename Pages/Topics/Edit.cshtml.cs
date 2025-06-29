using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using Microsoft.AspNetCore.Identity;


namespace Forum.Pages.Topics
{
    public class EditModel : PageModel
    {
        private readonly Forum.Data.ApplicationDbContext _context;
        private readonly UserManager<ForumUser> _userManager;

        public EditModel(Forum.Data.ApplicationDbContext context, UserManager<ForumUser> userManager)
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

            var topic =  await _context.Topics.FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Forbid();
            }

            var isModerator = await _userManager.IsInRoleAsync(currentUser, "Moderator");

            if (topic.UserId != currentUser.Id && !isModerator)
            {
                return Forbid(); // само собственици или модератори могат да редактират
            }


            Topic = topic;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingTopic = await _context.Topics.AsNoTracking().FirstOrDefaultAsync(t => t.Id == Topic.Id);

            if (existingTopic == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Forbid();
            }
            
            var isModerator = await _userManager.IsInRoleAsync(currentUser, "Moderator");

            if (existingTopic.UserId != currentUser.Id && !isModerator)
            {
                return Forbid(); // само собственици или модератори могат да запазят редакции
            }       

            _context.Attach(Topic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(Topic.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
