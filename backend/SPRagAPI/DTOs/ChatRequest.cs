namespace SPRagAPI.DTOs;

public class ChatRequest
{
    public string Question { get; set; } = string.Empty;
    public string? ConversationId { get; set; }
}
