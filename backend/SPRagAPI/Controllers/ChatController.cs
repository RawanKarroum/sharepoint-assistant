using Microsoft.AspNetCore.Mvc;
using SPRagAPI.DTOs;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chat;

    public ChatController(IChatService chat)
    {
        _chat = chat;
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

        var response = await _chat.AnswerAsync(request, cancellationToken);
        return Ok(response);
    }
}
