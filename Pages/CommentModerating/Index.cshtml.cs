using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Pages.CommentModerating
{
    [Authorize(Roles = "Moderator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Comment> FlaggedComments { get; set; } = [];

        public async Task OnGetAsync()
        {
            FlaggedComments = await _context.Comments
            .Include(c => c.CreatedByUser)
            .Include(c => c.Topic)
            .Where(c => c.IsFlagged)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id) // одобрение на флагнат коментар
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            comment.IsFlagged = false;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id) // отхвърляне на флагнат коментар
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();

            _context.Comments.Remove(comment); // премахване на коментара
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}


// в този файл се намират всички коментари, които са IsFlagged == true