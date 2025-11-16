namespace StripeDemo.Models;

public class Transaction(string name, string email, decimal amount, int productId) {

    public enum TransactionStatus {
        Pending,
        Confirmed,
        RequiresAction,
        Succeeded,
        Failed,
        Canceled
    }

    public int Id { get; set; }

    // Informations de l'utilisateur
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;

    // Informations de Stripe
    public string PaymentIntentId { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    // Détails du paiement
    public decimal Amount { get; set; } = amount;
    public string Currency { get; set; } = "cad";
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    // Relation avec le produit
    public int ProductId { get; set; } = productId;
    public Product Product { get; set; }

    public void Update(string paymentIntentStatus) {
        Status = paymentIntentStatus switch {
            "succeeded" => TransactionStatus.Succeeded,
            "requires_action" => TransactionStatus.RequiresAction,
            "requires_payment_method" => TransactionStatus.Failed,
            "canceled" => TransactionStatus.Canceled,
            _ => TransactionStatus.Pending,
        };
    }
}
