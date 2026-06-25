using SPRagAPI.Models;

namespace SPRagAPI.Data;

public class DocumentEntity
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string WebUrl { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? LibraryName { get; set; }
    public string? Author { get; set; }
    public DateTime? LastModified { get; set; }
    public string? ContentType { get; set; }
    public string ExtractedText { get; set; } = string.Empty;
    public DateTime SyncedAt { get; set; }

    public List<DocumentChunk> Chunks { get; set; } = new();
}
