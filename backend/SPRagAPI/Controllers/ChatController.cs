using Microsoft.AspNetCore.Mvc;
using SPRagAPI.DTOs;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chat, ILogger<ChatController> logger)
    {
        _chat = chat;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Ask(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new { error = "request.question is required." });
        }

        try
        {
            _logger.LogInformation(
                "Chat request received (conversationId: {ConversationId}, questionLength: {QuestionLength})",
                request.ConversationId,
                request.Question.Length);

            var response = await _chat.AnswerAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Chat request failed (conversationId: {ConversationId})",
                request.ConversationId);
            throw;
        }
    }
}
