using Microsoft.AspNetCore.Mvc;
using SPRagAPI.Models;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISharePointDocumentService _documents;

    public DocumentsController(ISharePointDocumentService documents)
    {
        _documents = documents;
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
}
