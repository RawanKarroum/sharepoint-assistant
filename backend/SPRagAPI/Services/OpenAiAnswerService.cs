using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SPRagAPI.Models;

namespace SPRagAPI.Services;

public class OpenAiAnswerService : IAiAnswerService
{
    private const string SystemPrompt =
        "You are a helpful assistant that answers questions strictly based on the provided " +
        "document excerpts from SharePoint. If the excerpts do not contain the answer, say you " +
        "don't know based on the available documents. Be concise.";

    private readonly ChatClient _client;

    public OpenAiAnswerService(IOptions<OpenAiOptions> options)
    {
        var opts = options.Value;
        if (string.IsNullOrWhiteSpace(opts.ApiKey))
        {
            throw new InvalidOperationException(
                "OpenAI:ApiKey is not configured. Set it via user-secrets or the OpenAI__ApiKey environment variable.");
        }

        _client = new ChatClient(model: opts.Model, apiKey: opts.ApiKey);
    }

    public async Task<string> GenerateAnswerAsync(
        string question,
        IReadOnlyList<DocumentChunk> contextChunks,
        CancellationToken cancellationToken = default)
    {
        var contextBlock = string.Join(
            "\n---\n",
            contextChunks.Select((c, i) =>
                $"[Source {i + 1}: {c.DocumentTitle}]\n{c.Content}"));

        var userPrompt =
            $"Question: {question}\n\n" +
            $"Document excerpts:\n{contextBlock}";

        var completion = await _client.CompleteChatAsync(
            new ChatMessage[]
            {
                new SystemChatMessage(SystemPrompt),
                new UserChatMessage(userPrompt)
            },
            cancellationToken: cancellationToken);

        return completion.Value.Content[0].Text;
    }
}
