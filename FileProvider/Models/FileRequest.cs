using System.ComponentModel.DataAnnotations;

namespace FileProvider.Models;

public class FileRequest
{

    public Guid OrderId { get; set; }
    [Required]

    public Guid CustomerId { get; set; }
    [Required]
    public string CustomerEmail { get; set; }

    public DateTime CreatedAt { get; set; }
    [Required]
    public DateTime SoldUntil { get; set; }

    public decimal PricePerProduct { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPriceWithoutVat { get; set; }

    public decimal TotalPrice { get; set; }

    public string PaymentStatus { get; set; }

    public Guid FiltersUsed { get; set; }

    public string KlarnaPaymentId { get; set; }
}
