using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Readit.DataAccess;
using Readit.Library;
using Readit.Models;
using Readit.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;


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
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<LibraryService>();

        builder.Services.AddHttpClient<BookApiService>();
        
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("uk") };

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
        });
        builder.Services.AddRazorPages()
            .AddViewLocalization();
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
        var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);

        app.UseRouting();
        app.UseStaticFiles();
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
