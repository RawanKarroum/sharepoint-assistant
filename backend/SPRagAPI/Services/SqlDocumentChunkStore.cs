using Microsoft.EntityFrameworkCore;
using SPRagAPI.Data;
using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class SqlDocumentChunkStore : IDocumentChunkStore
{
    private readonly AppDbContext _db;

    public SqlDocumentChunkStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(
        IEnumerable<DocumentChunk> chunks,
        CancellationToken cancellationToken = default)
    {
        _db.Chunks.AddRange(chunks);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _db.Chunks
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _db.Chunks.ExecuteDeleteAsync(cancellationToken);
    }
}
