using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FileProvider.Interfaces;
using FileProvider.Models;
using FileProvider.Repositories;
using FileProvider.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.RabbitMQ;
using Microsoft.Extensions.Logging;

namespace OrderReportFunction;

public class FileFunction
{
    private readonly IProductRepository _productRepository;
    private readonly IFileService _fileService;
    private readonly IStorageService _storageService;
    private readonly IEmailService _emailService;

    public FileFunction(IProductRepository productRepository, IFileService fileService, IStorageService storageService, IEmailService emailService)
    {
        _productRepository = productRepository;
        _fileService = fileService;
        _storageService = storageService;
        _emailService = emailService;
    }

    [FunctionName("GenerateOrderReport")]
    public async Task Run(
        [RabbitMQTrigger("order_reports", ConnectionStringSetting = "RabbitMQConnection")] string message,
        [RabbitMQ(ConnectionStringSetting = "RabbitMQConnection")] ICollector<string> rabbitQueue,
        ILogger log)
    {
        log.LogInformation($"Received RabbitMQ message: {message}");

        try
        {
            // Parse the incoming message
            var request = JsonSerializer.Deserialize<FileRequest>(message);
            if (request == null || string.IsNullOrEmpty(request.RecipientEmail))
            {
                log.LogError("Invalid message received.");
                return;
            }

            // 1. Get all sold products
            var products = await _productRepository.GetSoldProductsAsync();
            var productList = new List<ProductEntity>(products);

            if (productList.Count == 0)
            {
                log.LogWarning("No sold products found.");
                return;
            }

            // 2. Generate Excel file
            byte[] excelFile = _fileService.GenerateExcel(productList);

            // 3. Upload file to Azure Blob Storage
            string fileName = $"OrderReport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
            string fileUrl = await _storageService.UploadFileAsync(excelFile, fileName);

            log.LogInformation($"File uploaded: {fileUrl}");

            // 4. Send email with file link
            await _emailService.SendEmailAsync(request.RecipientEmail, fileUrl);

            log.LogInformation("Email sent successfully.");

            // 5. Send confirmation back to OrderProvider via RabbitMQ
            var response = new FileGenerationResponse
            {
                Success = true,
                Message = "Order report generated and emailed successfully.",
                FileUrl = fileUrl
            };

            rabbitQueue.Add(JsonSerializer.Serialize(response));
            log.LogInformation("Response sent to OrderProvider.");
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing order report: {ex.Message}");
            var errorResponse = new FileGenerationResponse
            {
                Success = false,
                Message = $"Failed to generate order report: {ex.Message}"
            };
            rabbitQueue.Add(JsonSerializer.Serialize(errorResponse));
        }
    }
}
