namespace FileProvider.Interfaces;

public interface IStorageService
{
    Task UploadFileAsync(string fileName, byte[] fileContent);
    Task<string> GetFileUrlAsync(string fileName);
}