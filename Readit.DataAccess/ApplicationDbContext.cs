using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readit.Models;

namespace Readit.DataAccess;

public class ApplicationDbContext : IdentityDbContext<User>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {}

    public DbSet<Book> Books { get; set; }
    public DbSet<UserBook> UserBooks { get; set; }
    public DbSet<Comment> Comments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var admin = new IdentityRole
        {
            Id = "1",
            Name = "admin",
            NormalizedName = "ADMIN"
        };

        var user = new IdentityRole
        {
            Id = "2",
            Name = "user",
            NormalizedName = "USER"
        };

        
        modelBuilder.Entity<IdentityRole>().HasData(admin, user);
        
        modelBuilder.Entity<Book>().HasData(new Book
        {
            Id = 1,
            Title = "Test Book"
        });
    }
}