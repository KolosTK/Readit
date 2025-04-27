using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Readit.DataAccess;
using Readit.Models;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        // ✅ Seed roles and assign them to users before app runs
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await SeedRolesAndAdminAsync(services);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication(); 
        app.UseAuthorization();

        app.MapRazorPages();

        await app.RunAsync(); 
    }
    
    private static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        string[] roles = { "admin", "user" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Assign "user" role to all users who don't have it
        var allUsers = userManager.Users.ToList();
        foreach (var user in allUsers)
        {
            var rolesForUser = await userManager.GetRolesAsync(user);
            if (!rolesForUser.Contains("user"))
            {
                await userManager.AddToRoleAsync(user, "user");
            }
        }

        // Optional: assign "admin" role to specific user
        var adminEmail = "try@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
    }
}
