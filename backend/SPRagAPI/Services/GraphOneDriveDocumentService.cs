using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using SPRagAPI.Models;

namespace SPRagAPI.Services;

// OneDrive and SharePoint document libraries both expose files through Graph's DriveItem API.
// In Graph terms:
//   - A user's OneDrive is the drive at GET /me/drive
//   - A SharePoint document library is also a drive, but at /sites/{siteId}/drives/{driveId}
// Both return the same DriveItem shape (id, name, webUrl, file vs folder, ...), so the code
// here will be ~90% reusable later when we point at a SharePoint library.
public class GraphOneDriveDocumentService : ISharePointDocumentService
{
    private readonly GraphServiceClient _graph;
    private readonly GraphOneDriveOptions _options;
    private readonly IFileTextExtractionService _textExtractor;
    private readonly ILogger<GraphOneDriveDocumentService> _logger;

    public GraphOneDriveDocumentService(
        IOptions<GraphOneDriveOptions> options,
        IFileTextExtractionService textExtractor,
        ILogger<GraphOneDriveDocumentService> logger)
    {
        _options = options.Value;
        _textExtractor = textExtractor;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_options.TenantId) || string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new InvalidOperationException(
                "GraphOneDrive:TenantId and GraphOneDrive:ClientId must be set in user-secrets. " +
                "Register an app at https://aka.ms/AppRegistrations (delegated permission Files.Read) " +
                "and store both values via `dotnet user-secrets set`.");
        }

        // Device code flow: the first time the app calls Graph it prints a URL + short code
        // to the API console. Open the URL in any browser, enter the code, sign in once,
        // and the SDK keeps a cached token for the next ~1 hour. After that it auto-refreshes.
        var credential = new DeviceCodeCredential(new DeviceCodeCredentialOptions
        {
            TenantId = _options.TenantId,
            ClientId = _options.ClientId,
            DeviceCodeCallback = (info, _) =>
            {
                Console.WriteLine();
                Console.WriteLine(info.Message);
                Console.WriteLine();
                return Task.CompletedTask;
            },
            TokenCachePersistenceOptions = new TokenCachePersistenceOptions
            {
                Name = "SPRagAPI.GraphCache",
                UnsafeAllowUnencryptedStorage = true
            }
        });

        // "Files.Read" is the minimum delegated scope to list and read user's OneDrive items.
        _graph = new GraphServiceClient(credential, scopes: new[] { "Files.Read" });
    }

    public async Task<IReadOnlyList<SharePointDocument>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        // GET /me/drive/root:/RAG-Test-Docs:/children
        // The "path:/" syntax addresses a DriveItem by name instead of GUID — easier in code.
        // The SharePoint-library equivalent later will be:
        //   _graph.Sites[siteId].Drives[driveId].Root.ItemWithPath(folderName).Children
        var drive = await _graph.Me.Drive.GetAsync(cancellationToken: cancellationToken);
        if (drive?.Id is null)
        {
            return Array.Empty<SharePointDocument>();
        }

        DriveItemCollectionResponse? page;
        try
        {
            page = await _graph.Drives[drive.Id]
                .Root
                .ItemWithPath(_options.FolderName)
                .Children
                .GetAsync(cancellationToken: cancellationToken);
        }
        catch (ODataError ex) when (ex.ResponseStatusCode == 404)
        {
            return Array.Empty<SharePointDocument>();
        }

        if (page?.Value is null)
        {
            return Array.Empty<SharePointDocument>();
        }

        // V1: list files only, skip subfolders. (DriveItem.File is null for folders.)
        var fileItems = page.Value.Where(item => item.File != null).ToList();
        var documents = new List<SharePointDocument>(fileItems.Count);

        // Sequential download keeps the code easy to follow; switch to Task.WhenAll for many files.
        foreach (var item in fileItems)
        {
            documents.Add(await BuildDocumentAsync(item, drive.Id!, cancellationToken));
        }

        return documents;
    }

    public async Task<SharePointDocument?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        // GET /me/drive/items/{id}
        try
        {
            var drive = await _graph.Me.Drive.GetAsync(cancellationToken: cancellationToken);
            if (drive?.Id is null)
            {
                return null;
            }

            var item = await _graph.Drives[drive.Id].Items[id]
                .GetAsync(cancellationToken: cancellationToken);
            return item is null ? null : await BuildDocumentAsync(item, drive.Id!, cancellationToken);
        }
        catch (ODataError ex) when (ex.ResponseStatusCode == 404)
        {
            return null;
        }
    }

    private async Task<SharePointDocument> BuildDocumentAsync(
        DriveItem item,
        string driveId,
        CancellationToken cancellationToken)
    {
        var document = new SharePointDocument
        {
            Id = item.Id ?? string.Empty,
            Title = item.Name ?? string.Empty,
            WebUrl = item.WebUrl ?? string.Empty,
            SiteId = null,
            LibraryName = "OneDrive",
            Author = item.CreatedBy?.User?.DisplayName,
            LastModified = item.LastModifiedDateTime?.UtcDateTime,
            ContentType = Path.GetExtension(item.Name ?? string.Empty)
                .TrimStart('.')
                .ToLowerInvariant(),
            ExtractedText = string.Empty
        };

        if (string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.Name))
        {
            return document;
        }

        try
        {
            // GET /drives/{driveId}/items/{itemId}/content
            var contentStream = await _graph.Drives[driveId].Items[item.Id].Content
                .GetAsync(cancellationToken: cancellationToken);

            if (contentStream is null)
            {
                return document;
            }

            await using (contentStream)
            {
                document.ExtractedText = await _textExtractor.ExtractTextAsync(
                    contentStream,
                    item.Name,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to download or extract text for OneDrive file {FileName} ({ItemId})",
                item.Name,
                item.Id);
        }

        return document;
    }
}
