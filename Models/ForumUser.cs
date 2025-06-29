using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class ForumUser : IdentityUser
    {
        public bool IsActive { get; set; } = false; // дали потребителят е активиран от администратор, т.е. може да влиза във форума
        
        public bool IsDeactivated { get; set; } = false; // дали потребителят е деактивиран от администратор, т.е. не може да влиза във форума

        public string? DisplayName { get; set; } // показване на име
    }
}

// ForumUser е персонализиран потребителски клас, който наследява от IdentityUser. Има две свойства.