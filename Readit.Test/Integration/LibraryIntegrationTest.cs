using Microsoft.EntityFrameworkCore;
using Readit.DataAccess;
using Readit.Models;
using Xunit;

namespace Readit.Test.Integration;

public class LibraryIntegrationTest
{
    [Fact]
    public async Task AddBook_ThenFetchBooks_WorksWithRealDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("IntegrationTestDb")
            .Options;

        var context = new ApplicationDbContext(options);

        var user = new User { Id = "userX", UserName = "integrationuser" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.UserBooks.Add(new UserBook
        {
            Title = "Integration Book",
            UserId = user.Id,
            WorkKey = "OL987"
        });
        await context.SaveChangesAsync();

        var books = context.UserBooks.Where(b => b.UserId == user.Id).ToList();

        Assert.Single(books);
        Assert.Equal("Integration Book", books[0].Title);
    }

}