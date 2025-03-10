using System.ComponentModel.DataAnnotations;

namespace FileProvider.Models;

class FileRequest
{
    [Required]
    public Guid CustomerId { get; set; }
    [Required]
    public DateTime SoldUntil { get; set; }
}
