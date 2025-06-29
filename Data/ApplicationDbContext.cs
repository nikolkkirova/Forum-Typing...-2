using Forum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Data;

public class ApplicationDbContext : IdentityDbContext<ForumUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    // моите DbSets
    public DbSet<Topic> Topics { get; set; } = default!; // Таблица с теми, които ще бъдат съхранявани в базата данни
    public DbSet<Comment> Comments { get; set; } = default!; // Таблица с коментари, които ще бъдат съхранявани в базата данни

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);

        // Връзка: един Topic може да има много Comments
        builder.Entity<Comment>()
            .HasOne(c => c.Topic) // даден коментар принадлежи на една тема
            .WithMany(t => t.Comments) // една тема може да има много коментари
            .HasForeignKey(c => c.TopicId) // TopicId е външен ключ
            .OnDelete(DeleteBehavior.Cascade); // Когато тема се изтрие, всички коментари към нея също ще бъдат изтрити

        // Връзка: един ForumUser може да има много Comments
        builder.Entity<Comment>()
            .HasOne(c => c.CreatedByUser) // един коментар е създаден от един потребител
            .WithMany() // потребител може да има много коментари, засега го оставяме празно
            .HasForeignKey(c => c.CreatedByUserId) // CreatedByUserId е външен ключ, който указва кой потребител е написал коментара
            .OnDelete(DeleteBehavior.SetNull); // Когато потребител се изтрие, коментарите му ще останат, но полето CreatedByUserId ще бъде зададено на null
    }

}

/* Записки

ApplicationDbContext --> клас, който описва връзката с базата и таблиците
DbSet<Topic> Topics --> създава таблица Topics в базата данни
default! --> уверяваме компилатора, че EF ще инициализира Topics

*/