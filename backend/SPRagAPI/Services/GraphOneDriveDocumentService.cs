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

    public GraphOneDriveDocumentService(IOptions<GraphOneDriveOptions> options)
    {
        _options = options.Value;
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
        return page.Value
            .Where(item => item.File != null)
            .Select(MapToDocument)
            .ToList();
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
            return item is null ? null : MapToDocument(item);
        }
        catch (ODataError ex) when (ex.ResponseStatusCode == 404)
        {
            return null;
        }
    }

    private static SharePointDocument MapToDocument(DriveItem item)
    {
        // ExtractedText is intentionally empty here. V1 lists metadata only;
        // file content download + text extraction (PDF/Word -> string) is a separate step.
        return new SharePointDocument
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
    }
}
