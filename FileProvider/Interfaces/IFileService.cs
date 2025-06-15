using FileProvider.Models;

namespace FileProvider.Interfaces;

public interface IFileService
{
    byte[] GenerateExcel(List<ProductEntity> products);
    Task<List<ProductEntity>> GetSoldProductsAsync(Guid customerId);

    Task<string> GenerateAndUploadExcelAsync(Guid customerId);
}