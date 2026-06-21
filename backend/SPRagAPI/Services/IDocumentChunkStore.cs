using SPRagAPI.Models;

namespace SPRagAPI.Services;

public interface IDocumentChunkStore
{
    Task AddRangeAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentChunk>> GetAllAsync(CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}
