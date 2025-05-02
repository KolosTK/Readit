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
    
    [JsonPropertyName("first_publish_year")]
    public int? FirstPublishYear { get; set; }
    [JsonPropertyName("key")]
    public string? Key { get; set; }
    [JsonIgnore] 
    public bool IsInLibrary { get; set; }
}