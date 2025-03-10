using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using FileProvider.Interfaces;
using FileProvider.Models;

namespace FileProvider.Services
{
    public class FileService : IFileService
    {
        public byte[] GenerateExcel(List<ProductEntity> products)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sold Products");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Sold Until";
            worksheet.Cell(1, 4).Value = "Customer ID";
            worksheet.Cell(1, 5).Value = "Price";

            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cell(row, 1).Value = product.Id;
                worksheet.Cell(row, 2).Value = product.Name;
                worksheet.Cell(row, 3).Value = product.SoldUntil.ToString();
                worksheet.Cell(row, 4).Value = product.CustomerId;
                worksheet.Cell(row, 5).Value = product.Price;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
