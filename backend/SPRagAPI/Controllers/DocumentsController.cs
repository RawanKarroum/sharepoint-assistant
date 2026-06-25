using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPRagAPI.Data;
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
    private readonly GraphOneDriveDocumentService _oneDrive;
    private readonly AppDbContext? _db;

    public DocumentsController(
        ISharePointDocumentService documents,
        IDocumentChunkingService chunker,
        IDocumentChunkStore store,
        GraphOneDriveDocumentService oneDrive,
        IServiceProvider serviceProvider)
    {
        _documents = documents;
        _chunker = chunker;
        _store = store;
        _oneDrive = oneDrive;
        _db = serviceProvider.GetService<AppDbContext>();
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

        if (_db is not null)
        {
            await _db.Documents.ExecuteDeleteAsync(cancellationToken);
        }

        var documents = await _documents.GetAllAsync(cancellationToken);
        var chunkCount = 0;
        var syncedAt = DateTime.UtcNow;

        foreach (var doc in documents)
        {
            var chunks = _chunker.Chunk(doc);
            chunkCount += chunks.Count;

            if (_db is not null)
            {
                var entity = new DocumentEntity
                {
                    Id = doc.Id,
                    Title = doc.Title,
                    WebUrl = doc.WebUrl,
                    SiteId = doc.SiteId,
                    LibraryName = doc.LibraryName,
                    Author = doc.Author,
                    LastModified = doc.LastModified,
                    ContentType = doc.ContentType,
                    ExtractedText = doc.ExtractedText,
                    SyncedAt = syncedAt
                };

                _db.Documents.Add(entity);
                await _db.SaveChangesAsync(cancellationToken);
            }

            await _store.AddRangeAsync(chunks, cancellationToken);
        }

        var result = new SyncResult
        {
            DocumentsProcessed = documents.Count,
            ChunksCreated = chunkCount,
            SyncedAt = syncedAt
        };

        return Ok(result);
    }

    // Temporary diagnostic endpoint. Once we're happy with the integration,
    // we'll bind ISharePointDocumentService to GraphOneDriveDocumentService and remove this.
    [HttpGet("onedrive-test")]
    public async Task<ActionResult<IReadOnlyList<SharePointDocument>>> ListOneDrive(
        CancellationToken cancellationToken)
    {
        var docs = await _oneDrive.GetAllAsync(cancellationToken);
        return Ok(docs);
    }
}
