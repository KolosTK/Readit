using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Readit.Api.Models;
using Readit.DataAccess;
using Readit.Library;
using Readit.Models;
using Xunit;

public class LibraryServiceTests
{
    private static LibraryService CreateService(string dbName, out ApplicationDbContext context)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        context = new ApplicationDbContext(options);

        var userStoreMock = new Mock<IUserStore<User>>();
        var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        var testUser = new User { Id = "user123", UserName = "testuser" };
        userManagerMock.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, testUser.Id),
            new Claim(ClaimTypes.Name, testUser.UserName)
        }));

        var httpAccessor = new Mock<IHttpContextAccessor>();
        httpAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        return new LibraryService(context, userManagerMock.Object, httpAccessor.Object);
    }

    [Fact]
    public async Task ToggleBookAsync_ShouldAddBook_WhenNotExists()
    {
        var service = CreateService("AddBookDb", out var context);

        var book = new OpenLibraryBook
        {
            Title = "Test Book",
            CoverId = 123,
            Key = "OL123",
            AuthorName = new List<string> { "Author One" }
        };

        var result = await service.ToggleBookAsync(book);

        Assert.True(result);
        Assert.Single(context.UserBooks);
    }

    [Fact]
    public async Task ToggleBookAsync_ShouldRemoveBook_WhenExists()
    {
        var service = CreateService("RemoveBookDb", out var context);

        // First add
        await service.ToggleBookAsync(new OpenLibraryBook
        {
            Title = "Test Book",
            Key = "OL123"
        });

        // Then remove
        var result = await service.ToggleBookAsync(new OpenLibraryBook
        {
            Title = "Test Book",
            Key = "OL123"
        });

        Assert.False(result);
        Assert.Empty(context.UserBooks);
    }
}
