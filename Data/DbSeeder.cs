using Microsoft.AspNetCore.Identity; // за работа с роли и потребители
using Microsoft.Extensions.DependencyInjection; // за достъп до RoleManager и UserManager
using Forum.Models; // за достъп до моя клас ForumUser

// Този файл е за автоматично създаване на роли и администраторски потребител при стартиране на приложението.

namespace Forum.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRoles(IServiceProvider sp) // метод SeedRoles, който приема IServiceProvider
        {
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>(); // за създаване/проверка на роли
            var userManager = sp.GetRequiredService<UserManager<ForumUser>>(); // за създаване/проверка на потребители

            // масив с имената на ролите, които искаме да създадем
            string[] roles = { "User", "Moderator", "Administrator" };

            // Обхождаме всички роли и ги създаваме, ако не съществуват
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Администратор --> аз
            var adminEmail = "nikolkirova@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail); // тук се търси дали вече не съществува администратор с този имейл

            // Ако не съществува, създаваме го
            if (adminUser == null)
            {
                adminUser = new ForumUser // създаваме нов обект от тип ForumUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // потвърждаваме имейла, за да не се налага да го потвърждаваме ръчно
                };

                await userManager.CreateAsync(adminUser, "dF6cXU3@n3!aL@q"); // паролата на администратора

                // след като създадем потребителя, добавяме го към ролята "Administrator"
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }

            // Модератор --> Дани
            var moderatorEmail = "dani@gmail.com";
            var moderatorUser = await userManager.FindByEmailAsync(moderatorEmail);

            if (moderatorUser == null)
            {
                moderatorUser = new ForumUser
                {
                    UserName = moderatorEmail,
                    Email = moderatorEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(moderatorUser, "8i2TFynLSCfK.$P"); // паролата на модератора
                await userManager.AddToRoleAsync(moderatorUser, "Moderator"); // добавяме го към ролята "Moderator"
            }
        }
    }
}

// seed --> създаване на начални данни
// клас DbSeeder --> логика за създаване на роли
// метод SeedRoles
// IServiceProvider --> контейнер, който държи достъп до всичките услуги като UserManager, DbContext и т.н.
// roleManager --> за създаване/проверка на роли
// userManager --> за създаване/проверка на потребители, взема се от вграден контейнер
// async --> асинхронен метод, позволява използването на await
// await --> "изчакай" асинхронната операция да завърши, преди да продължи изпълнението на кода

// Dependency Injection (DI) --> механизъм, чрез който външни зависимости се вкарват автоматично в даден клас, например вместо сами да създаваме RoleManager, DI ни го дава
