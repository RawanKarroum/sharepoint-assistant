using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class DocumentChunkingService : IDocumentChunkingService
{
    private const int ChunkSize = 500;
    private const int ChunkOverlap = 50;

    public IReadOnlyList<DocumentChunk> Chunk(SharePointDocument document)
    {
        var text = document.ExtractedText;
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<DocumentChunk>();
        }

        var chunks = new List<DocumentChunk>();
        var step = ChunkSize - ChunkOverlap;
        var chunkIndex = 0;

        for (var start = 0; start < text.Length; start += step)
        {
            var length = Math.Min(ChunkSize, text.Length - start);
            var content = text.Substring(start, length);

            chunks.Add(new DocumentChunk
            {
                Id = $"{document.Id}:{chunkIndex}",
                DocumentId = document.Id,
                ChunkIndex = chunkIndex,
                Content = content,
                DocumentTitle = document.Title,
                DocumentUrl = document.WebUrl
            });

            chunkIndex++;
        }

        return chunks;
    }
}
