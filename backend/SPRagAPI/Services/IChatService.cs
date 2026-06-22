using SPRagAPI.DTOs;

namespace SPRagAPI.Services;

public interface IChatService
{
    Task<ChatResponse> AnswerAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
