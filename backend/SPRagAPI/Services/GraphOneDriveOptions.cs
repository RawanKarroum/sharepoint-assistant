namespace SPRagAPI.Services;

public class GraphOneDriveOptions
{
    public const string SectionName = "GraphOneDrive";

    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string FolderName { get; set; } = "RAG-Test-Docs";
}
