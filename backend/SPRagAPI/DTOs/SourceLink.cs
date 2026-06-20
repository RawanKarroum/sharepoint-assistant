namespace SPRagAPI.DTOs;

public class SourceLink
{
    public string DocumentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Snippet { get; set; }
}
