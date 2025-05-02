using System.Text.Json.Serialization;

namespace Readit.Api.Models;

public class OpenLibraryBook
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("author_name")]
    public List<string> AuthorName { get; set; }
    
    [JsonPropertyName("cover_i")]
    public int? CoverId { get; set; }
}