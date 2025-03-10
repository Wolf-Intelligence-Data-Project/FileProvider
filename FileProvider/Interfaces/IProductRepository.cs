using FileProvider.Models;

namespace FileProvider.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductEntity>> GetSoldProductsAsync();
    }
}