namespace SPRagAPI.Services;

public interface IFileTextExtractionService
{
    /// <summary>
    /// Reads text from a file stream based on the file extension.
    /// Returns an empty string for unsupported types or on failure.
    /// </summary>
    Task<string> ExtractTextAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default);
}
