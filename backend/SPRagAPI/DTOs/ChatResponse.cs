namespace SPRagAPI.DTOs;

public class ChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public List<SourceLink> Sources { get; set; } = new();
    public string? ConversationId { get; set; }
}
