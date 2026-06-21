namespace SPRagAPI.DTOs;

public class SyncResult
{
    public int DocumentsProcessed { get; set; }
    public int ChunksCreated { get; set; }
    public DateTime SyncedAt { get; set; }
}
