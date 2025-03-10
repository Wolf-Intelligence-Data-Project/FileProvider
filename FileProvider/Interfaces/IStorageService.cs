namespace FileProvider.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(byte[] fileData, string fileName);
    }
}