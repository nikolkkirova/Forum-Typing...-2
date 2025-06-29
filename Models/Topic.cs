using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // за атрибути като [Required], [Display], [Range]
using System.ComponentModel.DataAnnotations.Schema; // за атрибути като [ForeignKey]
using Microsoft.AspNetCore.Identity; // за достъп до Identity модели

namespace Forum.Models
{
    public class Topic
    {
        public int Id { get; set; } // уникален идентификатор на темата, EF Core ще го използва като първичен ключ

        [Required] // задължително поле, не може да е празно
        [StringLength(50, ErrorMessage = "Заглавието не може да е по-дълго от 50 символа.")] // Максимална дължина на заглавието
        public string Title { get; set; } = string.Empty; // заглавие на темата

        [Required] // задължително поле, не може да е празно
        [StringLength(500, ErrorMessage = "Описание не може да е по-дълго от 500 символа.")] // Максимална дължина на описанието
        public string Content { get; set; } = string.Empty; // съдържанието на темата, текстът, който потребителят въвежда

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // дата и час на създаване на темата, по подразбиране текущото време


        // Така всяка тема ще може да бъде свързана с потребител
        public string UserId { get; set; } = string.Empty; // идентификатор на потребителя, който е създал темата

        [ForeignKey("UserId")] // указва, че UserId е външен ключ към таблицата AspNetUsers
        public ForumUser? User { get; set; } // навигационно свойство, което позволява достъп до потребителя, който е създал темата

        public List<Comment> Comments { get; set; } = new(); // списък с коментари, свързани с дадена тема
    }
}

// Използването на string.Empty (празен низ) избягва null стойности
// ? (nullable оператор) означава, че полето може да бъде null
// Comments е списък с коментари под темата. Връзката е едно-към-много (една тема има много коментари).
// get връща стойността на свойството (четене)
// set задава стойността на свойството (писане)
// DateTime е тип в C# - дата и час 
// UserId идва от базовия клас IdentityUser и е от тип string, UserManager.CreateAsync() го генерира автоматично