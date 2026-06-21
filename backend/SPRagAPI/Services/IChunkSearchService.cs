using SPRagAPI.DTOs;

namespace SPRagAPI.Services;

public interface IChunkSearchService
{
    Task<IReadOnlyList<ChunkSearchResult>> SearchAsync(
        string query,
        int topK = 5,
        CancellationToken cancellationToken = default);
}
