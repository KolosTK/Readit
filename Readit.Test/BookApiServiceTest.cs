using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Readit.Api.Models;
using Readit.Services;
using Xunit;

public class BookApiServiceTests
{
    private static HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://openlibrary.org/") // used for formatting but doesn't send real request
        };
    }

    [Fact]
    public async Task SearchBooksAsync_ReturnsBooks_WhenValidJson()
    {
        // Arrange
        var mockJson = JsonSerializer.Serialize(new OpenLibrarySearchResult
        {
            Docs = new List<OpenLibraryBook>
            {
                new OpenLibraryBook { Title = "Test Book 1", Key = "OL1" },
                new OpenLibraryBook { Title = "Test Book 2", Key = "OL2" }
            }
        });

        var httpClient = CreateMockHttpClient(mockJson);
        var service = new BookApiService(httpClient);

        // Act
        var result = await service.SearchBooksAsync("test");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Test Book 1", result[0].Title);
    }

    [Fact]
    public async Task GetBookDetailsByKeyAsync_ReturnsCorrectBook_WhenMatchFound()
    {
        // Arrange
        var mockJson = JsonSerializer.Serialize(new OpenLibrarySearchResult
        {
            Docs = new List<OpenLibraryBook>
            {
                new OpenLibraryBook { Title = "Matched Book", Key = "/works/OL123" },
                new OpenLibraryBook { Title = "Other Book", Key = "/works/OL456" }
            }
        });

        var httpClient = CreateMockHttpClient(mockJson);
        var service = new BookApiService(httpClient);

        // Act
        var result = await service.GetBookDetailsByKeyAsync("OL123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Matched Book", result!.Title);
    }

    [Fact]
    public async Task GetBookDetailsByKeyAsync_ReturnsNull_WhenKeyIsEmpty()
    {
        var httpClient = CreateMockHttpClient("{}", HttpStatusCode.OK);
        var service = new BookApiService(httpClient);

        var result = await service.GetBookDetailsByKeyAsync("");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBookDetailsByKeyAsync_ReturnsNull_WhenNotFound()
    {
        var mockJson = JsonSerializer.Serialize(new OpenLibrarySearchResult
        {
            Docs = new List<OpenLibraryBook>() // empty list
        });

        var httpClient = CreateMockHttpClient(mockJson);
        var service = new BookApiService(httpClient);

        var result = await service.GetBookDetailsByKeyAsync("OL999");

        Assert.Null(result);
    }
}
