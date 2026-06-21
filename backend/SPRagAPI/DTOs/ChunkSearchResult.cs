using SPRagAPI.Models;

namespace SPRagAPI.DTOs;

public class ChunkSearchResult
{
    public DocumentChunk Chunk { get; set; } = new();
    public int Score { get; set; }
}
