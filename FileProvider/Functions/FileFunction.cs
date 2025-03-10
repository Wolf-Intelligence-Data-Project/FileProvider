using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using FileProvider.Interfaces;
using FileProvider.Models;

namespace FileProvider.Functions;

public class FileFunction
{
    private readonly ILogger<FileFunction> _logger;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;
    private readonly IStorageService _storageService;

    public FileFunction(
        ILogger<FileFunction> logger,
        IFileService fileService,
        IEmailService emailService,
        IStorageService storageService)
    {
        _logger = logger;
        _fileService = fileService;
        _emailService = emailService;
        _storageService = storageService;
    }

    [FunctionName("OrderReportFunction")]
    public async Task Run(
        [RabbitMQTrigger("order-report-queue", ConnectionStringSetting = "RabbitMQConnectionString")] string message,
        ILogger log)
    {
        try
        {
            var fileRequest = JsonSerializer.Deserialize<FileRequest>(message);
            if (fileRequest == null)
            {
                log.LogError("Invalid message received from RabbitMQ.");
                return;
            }

            log.LogInformation($"Processing order report for: {fileRequest.Email}");

            // Fetch sold products
            var products = _fileService.GetSoldProducts(fileRequest.CustomerId, fileRequest.SoldUntil);

            // Generate Excel file
            byte[] excelData = _fileService.GenerateExcel(products);

            // Upload to Blob Storage
            string fileUrl = await _storageService.GetFileUrlAsync($"order-report-{fileRequest.CustomerId}.xlsx");

            // Send email with download link
            await _emailService.SendEmailAsync(fileRequest.Email, fileUrl);

            log.LogInformation($"Report successfully sent to {fileRequest.Email}");
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing order report: {ex.Message}");
        }
    }
}
