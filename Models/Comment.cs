using System;
using System.ComponentModel.DataAnnotations; // за атрибути като [Required], [StringLength]
using System.ComponentModel.DataAnnotations.Schema; // за атрибути като [ForeignKey]

namespace Forum.Models
{
    public class Comment
    {
        public int Id { get; set; } // уникален идентификатор на коментара, EF Core ще го използва като първичен ключ

        [Required]
        public string Content { get; set; } = string.Empty; // съдържанието на коментара, текстът, който потребителят въвежда

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // дата и час на създаване на коментара, по подразбиране текущото време
        

        public string? CreatedByUserId { get; set; } = string.Empty; // кой потребител е създал коментара
        public ForumUser? CreatedByUser { get; set; } // навигационно свойство, което позволява достъп до потребителя, който е създал коментара

        public bool IsFlagged { get; set; } = false; // за да се отбелязват коментари, които трябва да бъдат прегледани от модератор, по подразбиране е false (не е отбелязан)

        public bool? IsApproved { get; set; } = null; // дали коментарът е одобрен от модератор, може да бъде null (неодобрен), true (одобрен) или false (отхвърлен)

        public int TopicId { get; set; } // връзка към темата, към която принадлежи коментарът (външен ключ)
        public Topic? Topic { get; set; } // навигационно свойство, което позволява достъп до темата, към която принадлежи коментарът
    }
}