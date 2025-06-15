using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using FileProvider.Interfaces;
using Azure.Storage.Sas;
using Azure.Storage;

namespace FileProvider.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public StorageService(IConfiguration configuration)
    {
        var connectionString = configuration["BlobStorageConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Storage connection string cannot be null.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadFileAsync(string containerName, string blobName, Stream content)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(content, overwrite: true);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }

    public async Task DeleteFileAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
    public string GetFileSasUrl(string containerName, string blobName, TimeSpan expiryTime)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiryTime)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
            _blobServiceClient.AccountName,
            "<your-storage-account-key>"
        )).ToString();

        return $"{blobClient.Uri}?{sasToken}";
    }
}