using System.Text.Json.Serialization;

namespace Readit.Api.Models;

public class OpenLibrarySearchResult
{
    [JsonPropertyName("docs")]
    public List<OpenLibraryBook> Docs { get; set; }
}