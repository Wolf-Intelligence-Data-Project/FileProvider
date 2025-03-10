using Azure.Storage.Blobs;
using FileProvider.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FileProvider.Services;

public class StorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public StorageService(IConfiguration config)
    {
        var blobServiceClient = new BlobServiceClient(config["BlobStorageConnectionString"]);
        _containerClient = blobServiceClient.GetBlobContainerClient(config["BlobContainerName"]);
    }

    public async Task<string> UploadFileAsync(byte[] fileData, string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        using var stream = new MemoryStream(fileData);
        await blobClient.UploadAsync(stream, true);
        return blobClient.Uri.ToString();
    }
}
