using Microsoft.AspNetCore.Mvc;
using SPRagAPI.DTOs;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly IChunkSearchService _search;

    public SearchController(IChunkSearchService search)
    {
        _search = search;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ChunkSearchResult>>> Search(
        [FromQuery] string q,
        [FromQuery] int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { error = "Query parameter 'q' is required." });
        }

        var results = await _search.SearchAsync(q, topK, cancellationToken);
        return Ok(results);
    }
}
