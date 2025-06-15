using ClosedXML.Excel;
using FileProvider.Data;
using FileProvider.Interfaces;
using FileProvider.Models;
using Microsoft.EntityFrameworkCore;

namespace FileProvider.Services;

public class FileService : IFileService
{
    private readonly ProductDbContext _productDbContext;
    private readonly IStorageService _storageService;

    public FileService(ProductDbContext productDbContext, IStorageService storageService)
    {
        _productDbContext = productDbContext;
        _storageService = storageService;
    }

    public async Task<List<ProductEntity>> GetSoldProductsAsync(Guid customerId)
    {
        var latestSoldUntil = await _productDbContext.Products
            .Where(p => p.CustomerId == customerId && p.SoldUntil != null)
            .MaxAsync(p => p.SoldUntil);

        return await _productDbContext.Products
            .Where(p => p.CustomerId == customerId && p.SoldUntil == latestSoldUntil)
            .ToListAsync();
    }


    // NEEDS AUTO CLEANUP BLOB WHEN IT IS PUBLISHED 
    public byte[] GenerateExcel(List<ProductEntity> products)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sold Products");

        worksheet.Cell(1, 1).Value = "Company Name";
        worksheet.Cell(1, 2).Value = "Organization Number";
        worksheet.Cell(1, 3).Value = "Address";
        worksheet.Cell(1, 4).Value = "Postal Code";
        worksheet.Cell(1, 5).Value = "City";
        worksheet.Cell(1, 6).Value = "Phone Number";
        worksheet.Cell(1, 7).Value = "Email";
        worksheet.Cell(1, 8).Value = "Business Type";
        worksheet.Cell(1, 9).Value = "Revenue";
        worksheet.Cell(1, 10).Value = "Number of Employees";
        worksheet.Cell(1, 11).Value = "CEO";

        int row = 2;
        foreach (var product in products)
        {
            worksheet.Cell(row, 1).Value = product.CompanyName;
            worksheet.Cell(row, 2).Value = product.OrganizationNumber;
            worksheet.Cell(row, 3).Value = product.Address;
            worksheet.Cell(row, 4).Value = product.PostalCode;
            worksheet.Cell(row, 5).Value = product.City;
            worksheet.Cell(row, 6).Value = product.PhoneNumber;
            worksheet.Cell(row, 7).Value = product.Email;
            worksheet.Cell(row, 8).Value = product.BusinessType;
            worksheet.Cell(row, 9).Value = product.Revenue;
            worksheet.Cell(row, 10).Value = product.NumberOfEmployees;
            worksheet.Cell(row, 11).Value = product.CEO;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);


        return stream.ToArray();


    }

    public async Task<string> GenerateAndUploadExcelAsync(Guid customerId)
    {
        var soldProducts = await _productDbContext.Products
            .Where(p => p.CustomerId == customerId && p.SoldUntil != null)
            .OrderByDescending(p => p.SoldUntil)
            .ToListAsync();

        if (!soldProducts.Any())
            throw new Exception("No sold products found for the given customer.");

        Console.WriteLine($"--- SoldUntil values for customer {customerId} ---");
        foreach (var product in soldProducts)
        {
            Console.WriteLine($"ProductId: {product.ProductId}, SoldUntil: {product.SoldUntil:O}");
        }

        var latestSoldUntil = soldProducts.First().SoldUntil!.Value;

        var products = soldProducts
            .Where(p => p.SoldUntil!.Value == latestSoldUntil)
            .ToList();

        if (!products.Any())
            throw new Exception("No matching sold products found for the given customer.");

        byte[] excelFile = GenerateExcel(products);

        string folderPath = @"C:\filetest";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"SoldProducts_{customerId}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        await File.WriteAllBytesAsync(filePath, excelFile);

        return filePath;
    }
}
