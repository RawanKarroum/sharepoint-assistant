using SPRagAPI.DTOs;

namespace SPRagAPI.Services;

public class KeywordChunkSearchService : IChunkSearchService
{
    private static readonly char[] Separators =
        { ' ', '\t', '\r', '\n', '.', ',', ';', ':', '?', '!', '"', '\'', '(', ')', '[', ']' };

    private readonly IDocumentChunkStore _store;

    public KeywordChunkSearchService(IDocumentChunkStore store)
    {
        _store = store;
    }

    public async Task<IReadOnlyList<ChunkSearchResult>> SearchAsync(
        string query,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        var terms = Tokenize(query);
        if (terms.Count == 0 || topK <= 0)
        {
            return Array.Empty<ChunkSearchResult>();
        }

        var chunks = await _store.GetAllAsync(cancellationToken);

        return chunks
            .Select(c => new ChunkSearchResult { Chunk = c, Score = Score(c.Content, terms) })
            .Where(r => r.Score > 0)
            .OrderByDescending(r => r.Score)
            .Take(topK)
            .ToList();
    }

    private static List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        return text
            .ToLowerInvariant()
            .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length >= 2)
            .ToList();
    }

    private static int Score(string content, List<string> terms)
    {
        var lower = content.ToLowerInvariant();
        var score = 0;

        foreach (var term in terms)
        {
            var index = 0;
            while ((index = lower.IndexOf(term, index, StringComparison.Ordinal)) != -1)
            {
                score++;
                index += term.Length;
            }
        }

        return score;
    }
}
