using System.ComponentModel.DataAnnotations;

namespace StripeDemo.Models;

public class Transaction(
    string name, 
    string email, 
    decimal amount, 
    int productId
) {

    public enum TransactionStatus {
        Pending,
        Succeeded,
        Failed,
        Canceled
    }

    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = name;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = email;

    public string? PaymentIntentId { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public decimal Amount { get; set; } = amount;

    [Required]
    public string Currency { get; set; } = "cad";

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public int ProductId { get; set; } = productId;
    public Product? Product { get; set; }

    public void Update(string paymentIntentStatus) {

        Status = paymentIntentStatus switch {
            "succeeded" => TransactionStatus.Succeeded,
            "requires_payment_method" => TransactionStatus.Failed,
            "canceled" => TransactionStatus.Canceled,
            _ => TransactionStatus.Pending,
        };
    }
}
