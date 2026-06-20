namespace SPRagAPI.Models;

public class DocumentChunk
{
    public string Id { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? DocumentTitle { get; set; }
    public string? DocumentUrl { get; set; }
}
