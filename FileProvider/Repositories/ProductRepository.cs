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

    public async Task<IEnumerable<ProductEntity>> GetSoldProductsAsync(Guid customerId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = @"
        SELECT 
            [p].[Address], 
            [p].[BusinessType], 
            [p].[CEO], 
            [p].[City], 
            [p].[CompanyName],
            [p].[Email], 
            [p].[OrganizationNumber], 
            [p].[NumberOfEmployees], 
            [p].[PhoneNumber], 
            [p].[PostalCode],
            [p].[Revenue]
        FROM 
            [Products] AS [p]
        WHERE 
            [p].[CustomerId] = @CustomerId 
            AND [p].[SoldUntil] = (
                SELECT MAX([SoldUntil]) 
                FROM [Products] 
                WHERE [CustomerId] = @CustomerId
            )";

        return await connection.QueryAsync<ProductEntity>(
            query,
            new { CustomerId = customerId }
        );
    }

}
