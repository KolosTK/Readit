using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readit.Models;

namespace Readit.DataAccess;

public class ApplicationDbContext : IdentityDbContext<User>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {}
    public DbSet<UserBook> UserBooks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<Friendship> Friendships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict); // or .NoAction()

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Followee)
            .WithMany()
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict); // or .NoAction()

        // Your existing role seeding:
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "user", NormalizedName = "USER" }
        );

    }

}