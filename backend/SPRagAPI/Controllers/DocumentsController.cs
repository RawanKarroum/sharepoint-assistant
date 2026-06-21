using Microsoft.AspNetCore.Mvc;
using SPRagAPI.DTOs;
using SPRagAPI.Models;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISharePointDocumentService _documents;
    private readonly IDocumentChunkingService _chunker;
    private readonly IDocumentChunkStore _store;

    public DocumentsController(
        ISharePointDocumentService documents,
        IDocumentChunkingService chunker,
        IDocumentChunkStore store)
    {
        _documents = documents;
        _chunker = chunker;
        _store = store;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SharePointDocument>>> GetAll(
        CancellationToken cancellationToken)
    {
        var docs = await _documents.GetAllAsync(cancellationToken);
        return Ok(docs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SharePointDocument>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var doc = await _documents.GetByIdAsync(id, cancellationToken);
        return doc is null ? NotFound() : Ok(doc);
    }

    [HttpGet("{id}/chunks")]
    public async Task<ActionResult<IReadOnlyList<DocumentChunk>>> GetChunks(
        string id,
        CancellationToken cancellationToken)
    {
        var doc = await _documents.GetByIdAsync(id, cancellationToken);
        if (doc is null)
        {
            return NotFound();
        }

        var chunks = _chunker.Chunk(doc);
        return Ok(chunks);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResult>> Sync(CancellationToken cancellationToken)
    {
        await _store.ClearAsync(cancellationToken);

        var documents = await _documents.GetAllAsync(cancellationToken);
        var chunkCount = 0;

        foreach (var doc in documents)
        {
            var chunks = _chunker.Chunk(doc);
            await _store.AddRangeAsync(chunks, cancellationToken);
            chunkCount += chunks.Count;
        }

        var result = new SyncResult
        {
            DocumentsProcessed = documents.Count,
            ChunksCreated = chunkCount,
            SyncedAt = DateTime.UtcNow
        };

        return Ok(result);
    }
}
