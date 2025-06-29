using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Forum.Data;
using Forum.Models;
using Forum.MLModel;

namespace Forum.Pages.Topics
{
    public class IndexModel : PageModel
    {
        // DI зависимости
        private readonly Forum.Data.ApplicationDbContext _context;
        private readonly UserManager<ForumUser> _userManager;
        private readonly SentimentService _sentimentService;

        public IndexModel(Forum.Data.ApplicationDbContext context, UserManager<ForumUser> userManager, SentimentService sentimentService)
        {
            _context = context;
            _userManager = userManager;
            _sentimentService = sentimentService;
        }

        // Списък с теми и техните коментари
        public IList<Topic> Topic { get; set; } = default!;

        // Форма за коментиране
        [BindProperty]
        public string NewCommentContent { get; set; } = string.Empty;

        [BindProperty]
        public int NewCommentTopicId { get; set; }

        public bool IsModerator { get; set; }
        public string? CurrentUserId { get; set; }
        public async Task OnGetAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                IsModerator = await _userManager.IsInRoleAsync(user, "Moderator");
                CurrentUserId = user.Id;
            }

            Topic = await _context.Topics
                .Include(t => t.User)
                .Include(t => t.Comments.Where(c => !c.IsFlagged)) // показване само на одобрени коментари
                    .ThenInclude(c => c.CreatedByUser)
                .OrderByDescending(t => t.CreatedAt) // Подреждане на коментарите по дата и час, най - новите са най - горе
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(NewCommentContent))
            {
                ModelState.AddModelError(string.Empty, "Коментарът не може да бъде празен.");
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);

            // Моделът за машинно обучение се вика тук!!!!
            // метод Predict();   --> подава текста на коментара за анализ
            var result = _sentimentService.Predict(NewCommentContent);

            // SentimentService използва заредения ML.NET модел (SentimentModel.zip), за да предскаже дали коментарът е положителен или негативен

            bool isFlagged = !result.PredictedLabel; // ако е негативен, го флагваме

            // създаване на нов коментар
            var comment = new Comment
            {
                Content = NewCommentContent.Trim(),
                TopicId = NewCommentTopicId,
                CreatedByUserId = user!.Id, // Вземаме идентификатора на текущия потребител
                CreatedAt = DateTime.UtcNow,
                IsFlagged = isFlagged
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // да се презареди страницата, за да се покаже новият коментар
        } 
        public async Task<IActionResult> OnPostDeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var isOwner = comment.CreatedByUserId == currentUser.Id;
            var isModerator = await _userManager.IsInRoleAsync(currentUser, "Moderator");

            if (!isOwner && !isModerator)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }             
    }
}
