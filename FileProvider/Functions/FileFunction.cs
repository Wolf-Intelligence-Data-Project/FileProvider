using Microsoft.Extensions.Logging;
using FileProvider.Interfaces;
using FileProvider.Models;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker.Http;

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

    [Function("OrderReportFunction")]
    public async Task Run(
     [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "order-report")] HttpRequestData req,
     FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("OrderReportFunction");
        try
        {
            var message = await req.ReadAsStringAsync();

            var fileRequest = JsonConvert.DeserializeObject<FileRequest>(message);
            if (fileRequest == null)
            {
                logger.LogError("Invalid message received.");
                return;
            }

            logger.LogInformation($"Processing order report for: {fileRequest.CustomerEmail}");

            string fileUrl = await _fileService.GenerateAndUploadExcelAsync(fileRequest.CustomerId);

            // Send email with download link
            await _emailService.SendEmailAsync(fileRequest.CustomerEmail, fileUrl);

            logger.LogInformation($"Report successfully sent to {fileRequest.CustomerEmail}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing order report: {ex.Message}");
        }
    }
}
