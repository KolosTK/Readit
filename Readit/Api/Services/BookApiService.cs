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
    public async Task<OpenLibraryBook?> GetBookDetailsByKeyAsync(string workKey)
    {
        if (string.IsNullOrWhiteSpace(workKey))
            return null;

        var searchUrl = $"https://openlibrary.org/search.json?q={workKey}";
        var searchResponse = await _httpClient.GetAsync(searchUrl);
        if (!searchResponse.IsSuccessStatusCode)
            return null;

        var searchJson = await searchResponse.Content.ReadAsStringAsync();
        var searchData = JsonSerializer.Deserialize<OpenLibrarySearchResult>(searchJson);

        var book = searchData?.Docs?.FirstOrDefault(b => b.Key?.EndsWith(workKey) == true);
        return book;
    }


}
