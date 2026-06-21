using Microsoft.AspNetCore.Mvc;
using SPRagAPI.Models;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISharePointDocumentService _documents;
    private readonly IDocumentChunkingService _chunker;

    public DocumentsController(
        ISharePointDocumentService documents,
        IDocumentChunkingService chunker)
    {
        _documents = documents;
        _chunker = chunker;
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
}
