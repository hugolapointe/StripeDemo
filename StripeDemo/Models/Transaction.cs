using System.ComponentModel.DataAnnotations;

namespace StripeDemo.Models;

public class Transaction {

    public enum TransactionStatus {
        Pending,
        Succeeded,
        Failed,
        Canceled,
        Refunded
    }

    // Core Fields
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    // Customer Fields
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;

    // PSP Fields
    public string? PaymentIntentId { get; set; }

    // Audit Fields
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    private Transaction() { } // EF Core

    public Transaction(
        string name,
        string email,
        decimal amount) {
        Amount = amount;
        CustomerName = name;
        CustomerEmail = email;
    }

    public void Update(string paymentIntentStatus) {

        Status = paymentIntentStatus switch {
            "succeeded" => TransactionStatus.Succeeded,
            "requires_payment_method" => TransactionStatus.Failed,
            "canceled" => TransactionStatus.Canceled,
            _ => TransactionStatus.Pending,
        };
    }
}
