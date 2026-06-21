using SPRagAPI.Models;

namespace SPRagAPI.Services;

public interface IDocumentChunkingService
{
    IReadOnlyList<DocumentChunk> Chunk(SharePointDocument document);
}
