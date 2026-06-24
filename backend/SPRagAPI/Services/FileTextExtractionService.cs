using System.Text;
using UglyToad.PdfPig;

namespace SPRagAPI.Services;

public class FileTextExtractionService : IFileTextExtractionService
{
    private readonly ILogger<FileTextExtractionService> _logger;

    public FileTextExtractionService(ILogger<FileTextExtractionService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExtractTextAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();

        try
        {
            return extension switch
            {
                "pdf" => ExtractPdfText(content),
                "txt" => await ExtractPlainTextAsync(content, cancellationToken),
                _ => string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract text from {FileName}", fileName);
            return string.Empty;
        }
    }

    private static string ExtractPdfText(Stream content)
    {
        // PdfPig needs a seekable stream; Graph returns a forward-only stream.
        using var memoryStream = CopyToMemoryStream(content);

        using var document = PdfDocument.Open(memoryStream);
        var builder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            builder.AppendLine(page.Text);
        }

        return builder.ToString().Trim();
    }

    private static async Task<string> ExtractPlainTextAsync(
        Stream content,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(content, leaveOpen: true);
        return (await reader.ReadToEndAsync(cancellationToken)).Trim();
    }

    private static MemoryStream CopyToMemoryStream(Stream source)
    {
        var memoryStream = new MemoryStream();
        source.CopyTo(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
