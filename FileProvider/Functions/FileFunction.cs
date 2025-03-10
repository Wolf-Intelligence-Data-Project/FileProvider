using FileProvider.Interfaces;
using FileProvider.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FileProvider.Functions;

public class FileFunction
{
    private readonly ILogger<FileFunction> _logger;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;
    private readonly IStorageService _storageService; // Use IStorageService
    private readonly IConfiguration _config;

    public FileFunction(
        ILogger<FileFunction> logger,
        IFileService fileService,
        IEmailService emailService,
        IStorageService storageService, // Inject IStorageService
        IConfiguration config)
    {
        _logger = logger;
        _fileService = fileService;
        _emailService = emailService;
        _storageService = storageService; // Initialize storage service
        _config = config;
    }

    [Function("OrderReportFunction")]
    public async Task Run([Microsoft.Azure.Functions.Worker.RabbitMQTrigger("order-report-queue", ConnectionStringSetting = "RabbitMQConnectionString")] string message)
    {
        try
        {
            var fileRequest = JsonSerializer.Deserialize<FileRequest>(message);
            if (fileRequest == null)
            {
                _logger.LogError("Invalid message received from RabbitMQ.");
                return;
            }

            _logger.LogInformation($"Processing order report for: {fileRequest.Email}");

            // Fetch sold products from the database
            var products = _fileService.GetSoldProducts(fileRequest.CustomerId, fileRequest.SoldUntil);

            // Generate Excel file
            byte[] excelData = _fileService.GenerateExcel(products);

            // Upload to Blob Storage using IStorageService
            string fileUrl = await _storageService.GetFileUrlAsync($"order-report-{fileRequest.CustomerId}.xlsx");

            // Send email with download link
            await _emailService.SendEmailAsync(fileRequest.Email, fileUrl);

            _logger.LogInformation($"Report successfully sent to {fileRequest.Email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing order report: {ex.Message}");
        }
    }
}