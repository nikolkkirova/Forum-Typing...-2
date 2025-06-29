using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Forum.Data;
using Forum.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Добавяме DbContext-а и връзката към базата данни
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ForumUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // не изисква потвърждение на акаунта
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();


builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireModerator", policy => policy.RequireRole("Moderator", "Administator"));
    options.AddPolicy("RequireAdministrator", policy => policy.RequireRole("Administator"));
});

builder.Services.AddSingleton<Forum.MLModel.SentimentService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// app.MapStaticAssets();
//app.MapRazorPages()
//.WithStaticAssets();


app.MapRazorPages();
app.MapDefaultControllerRoute();

using (var scope = app.Services.CreateScope()) // CreateScope() създава временен обхват за dependency injection
{
    var services = scope.ServiceProvider; // services извлича достъп до UserManager и RoleManager
    await DbSeeder.SeedRoles(services); // извиква се методът за създаване на роли и администратор
}

app.Run();

