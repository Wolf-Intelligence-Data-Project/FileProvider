using ClosedXML.Excel;
using FileProvider.Data;
using FileProvider.Interfaces;
using FileProvider.Models;

namespace FileProvider.Services;

public class FileService : IFileService
{
    private readonly ProductDbContext _productDbContext;

    public FileService(ProductDbContext productDbContext)
    {
        _productDbContext = productDbContext;
    }

    // Correct implementation of GetSoldProducts fetching data from the database
    public List<ProductEntity> GetSoldProducts(Guid customerId, DateTime soldUntil)
    {
        // Query the database to get products that match the customerId and soldUntil date
        return _productDbContext.Products
            .Where(p => p.CustomerId == customerId && p.SoldUntil <= soldUntil)
            .ToList();
    }

    public byte[] GenerateExcel(List<ProductEntity> products)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sold Products");

        // Define headers (Removed Product ID, Sold Until, Customer ID, Reserved Until)
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
}
