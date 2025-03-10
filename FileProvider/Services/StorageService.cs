using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using FileProvider.Interfaces;

namespace FileProvider.Services;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public StorageService(IConfiguration configuration)
    {
        string connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"];
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task UploadFileAsync(string fileName, byte[] fileContent)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();  // Ensure the container exists

        var blobClient = containerClient.GetBlobClient(fileName);
        using (var stream = new MemoryStream(fileContent))
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }
    }

    public async Task<string> GetFileUrlAsync(string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        return blobClient.Uri.ToString();
    }
}