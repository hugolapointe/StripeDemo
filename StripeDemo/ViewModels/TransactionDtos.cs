namespace StripeDemo.ViewModels;

public record PaymentDetails(
    string CustomerName,
    string CustomerEmail,
    int ProductId
);

public record ConfirmPaymentDetails(
    int TransactionId,
    string PaymentIntentId,
    string PaymentMethodId
);
