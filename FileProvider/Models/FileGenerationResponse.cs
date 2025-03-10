using System.ComponentModel.DataAnnotations;

namespace FileProvider.Models;

public class FileGenerationResponse
{
    [Required]
    public string OrderId { get; set; }

    [Required]
    public string CustomerId { get; set; }

    public string Message { get; set; }

    [Required]
    public bool IsSuccess { get; set; } = false;
}
