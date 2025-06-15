using Microsoft.Extensions.Configuration;

namespace FileProvider.Interfaces;

// Still local development

public interface IStorageService
{
    Task<string> UploadFileAsync(string containerName, string blobName, Stream content);
    Task<Stream> DownloadFileAsync(string containerName, string blobName);
    Task DeleteFileAsync(string containerName, string blobName);

    string GetFileSasUrl(string containerName, string blobName, TimeSpan expiryTime);
}