using SPRagAPI.DTOs;

namespace SPRagAPI.Services;

public class ChatService : IChatService
{
    private const int TopK = 5;
    private const string NoMatchAnswer =
        "I couldn't find anything relevant in the available SharePoint documents to answer that.";

    private readonly IChunkSearchService _search;
    private readonly IAiAnswerService _ai;

    public ChatService(IChunkSearchService search, IAiAnswerService ai)
    {
        _search = search;
        _ai = ai;
    }

    public async Task<ChatResponse> AnswerAsync(
        ChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var hits = await _search.SearchAsync(request.Question, TopK, cancellationToken);

        if (hits.Count == 0)
        {
            return new ChatResponse
            {
                Answer = NoMatchAnswer,
                Sources = new List<SourceLink>(),
                ConversationId = request.ConversationId
            };
        }

        var chunks = hits.Select(h => h.Chunk).ToList();
        var answer = await _ai.GenerateAnswerAsync(request.Question, chunks, cancellationToken);

        var sources = chunks
            .GroupBy(c => c.DocumentId)
            .Select(g =>
            {
                var first = g.First();
                return new SourceLink
                {
                    DocumentId = first.DocumentId,
                    Title = first.DocumentTitle ?? string.Empty,
                    Url = first.DocumentUrl ?? string.Empty,
                    Snippet = Truncate(first.Content, 240)
                };
            })
            .ToList();

        return new ChatResponse
        {
            Answer = answer,
            Sources = sources,
            ConversationId = request.ConversationId
        };
    }

    private static string Truncate(string text, int max) =>
        text.Length <= max ? text : text[..max] + "...";
}
