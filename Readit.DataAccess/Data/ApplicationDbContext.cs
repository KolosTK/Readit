using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Readit.Models;

namespace Readit.DataAccess.Data;
public class ApplicationDbContext :  IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserBook> UserBooks { get; set; }
    public DbSet<BookGenre> BookGenres { get; set; }
    public DbSet<UserLibrary> UserLibraries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Combination of BookId and GenreId is unique, so it is a composite key
        modelBuilder.Entity<BookGenre>()
            .HasKey(bg => new { bg.BookId, bg.GenreId });
        
        modelBuilder.Entity<BookGenre>()
            //Each BookGenre entry is associated with one Book  
            .HasOne(bg => bg.Book)
            //Book can have many BookGenres
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId);
        
        modelBuilder.Entity<BookGenre>()
            .HasOne(bg => bg.Genre)
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId);
        
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "The Book Thief",
                ISBN = "9781784162122",
                Author = "Markus Zusak",
                GlobalRate = 8,
                
                Summary = "It is 1939. Nazi Germany. The country is holding its breath. Death has never been busier, and will be busier still.",
                PagesNumber = 592,
                YearOfPublishing = 2005
            },
            new Book
            {
                Id = 2,
                Title = "Believe it to achieve it",
                ISBN = "9781784162122",
                Author = "Brian Tracy and Christina Stein",
                GlobalRate = 9,
                Summary = "Letting go of negative thoughts is one of the most important steps to living a successful, fulfilling life, but also often the most difficult. In this practical, research-based guide, bestselling authors Brian Tracy and psychotherapist Christina Stein present their \"Psychology of Achievement\" program to help you identify and overcome detrimental patterns and ideas preventing you from achieving your goals or feeling happy and satisfied in your life.",
                PagesNumber = 228,
                YearOfPublishing = 2017
            }
        );
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Non-Fiction" },
            new Genre { Id = 2, Name = "Adventure" },
            new Genre { Id = 3, Name = "Classics" },
            new Genre { Id = 4, Name = "Historical" },
            new Genre { Id = 5, Name = "Fiction" }
        );
        
        modelBuilder.Entity<BookGenre>().HasData(
            new BookGenre { BookId = 2, GenreId = 1 }, 
            new BookGenre { BookId = 1, GenreId = 2 },  
            new BookGenre { BookId = 1, GenreId = 3 },  
            new BookGenre { BookId = 1, GenreId = 4 },  
            new BookGenre { BookId = 1, GenreId = 5 }  
        );
    }
}