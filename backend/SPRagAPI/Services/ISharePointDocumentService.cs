using SPRagAPI.Models;

namespace SPRagAPI.Services;

public interface ISharePointDocumentService
{
    Task<IReadOnlyList<SharePointDocument>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SharePointDocument?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
