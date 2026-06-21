using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class FakeSharePointDocumentService : ISharePointDocumentService
{
    private static readonly List<SharePointDocument> Documents =
    [
        new SharePointDocument
        {
            Id = "doc-001",
            Title = "Employee Handbook",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Employee-Handbook.pdf",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "HR Team",
            LastModified = new DateTime(2025, 3, 15),
            ContentType = "pdf"
        },
        new SharePointDocument
        {
            Id = "doc-002",
            Title = "Travel Policy",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Travel-Policy.docx",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "Finance Team",
            LastModified = new DateTime(2025, 1, 10),
            ContentType = "docx"
        },
        new SharePointDocument
        {
            Id = "doc-003",
            Title = "Remote Work Guidelines",
            WebUrl = "https://contoso.sharepoint.com/sites/hr/Documents/Remote-Work-Guidelines.pdf",
            SiteId = "site-hr",
            LibraryName = "Documents",
            Author = "HR Team",
            LastModified = new DateTime(2024, 11, 20),
            ContentType = "pdf"
        }
    ];

    public Task<IReadOnlyList<SharePointDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<SharePointDocument>>(Documents);
    }

    public Task<SharePointDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Documents.FirstOrDefault(d => d.Id == id));
    }
}
