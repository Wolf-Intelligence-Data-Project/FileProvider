using FileProvider.Models;

namespace FileProvider.Interfaces;

public interface IFileService
{
    byte[] GenerateExcel(List<ProductEntity> products);
    List<ProductEntity> GetSoldProducts(Guid customerId, DateTime soldUntil);
}