using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SPRagAPI.Data;
using SPRagAPI.Services;

namespace SPRagAPI.Controllers;

/// <summary>
/// Temporary troubleshooting endpoints. Remove when deployment issues are resolved.
/// </summary>
[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly IOptions<OpenAiOptions> _openAiOptions;
    private readonly AppDbContext? _db;
    private readonly ILogger<DebugController> _logger;

    public DebugController(
        IConfiguration configuration,
        IWebHostEnvironment environment,
        IOptions<OpenAiOptions> openAiOptions,
        IServiceProvider serviceProvider,
        ILogger<DebugController> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _openAiOptions = openAiOptions;
        _db = serviceProvider.GetService<AppDbContext>();
        _logger = logger;
    }

    [HttpGet("config")]
    public async Task<IActionResult> GetConfig(CancellationToken cancellationToken)
    {
        var ragDbConfigured = !string.IsNullOrWhiteSpace(
            _configuration.GetConnectionString("RagDb"));
        var openAiApiKeyConfigured = !string.IsNullOrWhiteSpace(
            _configuration[$"{OpenAiOptions.SectionName}:ApiKey"]);

        var canConnectToDatabase = false;
        int? documentChunkCount = null;
        int? documentCount = null;

        if (_db is not null)
        {
            try
            {
                canConnectToDatabase = await _db.Database.CanConnectAsync(cancellationToken);
                if (canConnectToDatabase)
                {
                    documentChunkCount = await _db.Chunks.CountAsync(cancellationToken);
                    documentCount = await _db.Documents.CountAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Debug config: database connectivity check failed");
            }
        }

        return Ok(new
        {
            aspNetCoreEnvironment = _environment.EnvironmentName,
            ragDbConnectionStringConfigured = ragDbConfigured,
            openAiApiKeyConfigured,
            openAiModel = _openAiOptions.Value.Model,
            canConnectToDatabase,
            documentChunkCount,
            documentCount
        });
    }
}
