using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class InMemoryDocumentChunkStore : IDocumentChunkStore
{
    private readonly List<DocumentChunk> _chunks = new();
    private readonly object _lock = new();

    public Task AddRangeAsync(
        IEnumerable<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _chunks.AddRange(chunks);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DocumentChunk>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<DocumentChunk> snapshot = _chunks.ToList();
            return Task.FromResult(snapshot);
        }
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _chunks.Clear();
        }
        return Task.CompletedTask;
    }
}
