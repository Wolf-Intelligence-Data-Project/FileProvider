using Dapper;
using FileProvider.Interfaces;
using FileProvider.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
namespace FileProvider.Repositories;

class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration config)
    {
        _connectionString = config["ProductsDatabase"];
    }

    public async Task<IEnumerable<ProductEntity>> GetSoldProductsAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = "SELECT * FROM Products WHERE SoldUntil IS NOT NULL AND CustomerId IS NOT NULL";
        return await connection.QueryAsync<ProductEntity>(query);
    }
}
