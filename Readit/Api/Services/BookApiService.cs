using Readit.Api.Models;

namespace Readit.Services;
using System.Net.Http;
using System.Text.Json;

public class BookApiService
{
    private readonly HttpClient _httpClient;

    public BookApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<OpenLibraryBook>> SearchBooksAsync(string query, int limit = 12, int offset = 0)
    {
        var url = $"https://openlibrary.org/search.json?q={query}&limit={limit}&offset={offset}";
        var response = await _httpClient.GetStringAsync(url);
        var searchResult = JsonSerializer.Deserialize<OpenLibrarySearchResult>(response);
        return searchResult?.Docs ?? new List<OpenLibraryBook>();
    }
}
