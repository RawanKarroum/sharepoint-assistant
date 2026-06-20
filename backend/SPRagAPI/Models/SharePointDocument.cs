namespace SPRagAPI.Models;

public class SharePointDocument
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string WebUrl { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public string? LibraryName { get; set; }
    public string? Author { get; set; }
    public DateTime? LastModified { get; set; }
    public string? ContentType { get; set; }
}
