using SPRagAPI.Models;

namespace SPRagAPI.Services;

public interface IAiAnswerService
{
    Task<string> GenerateAnswerAsync(
        string question,
        IReadOnlyList<DocumentChunk> contextChunks,
        CancellationToken cancellationToken = default);
}
