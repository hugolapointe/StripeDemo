namespace StripeDemo.ViewModels;

public record ConfirmationDetails(
    int Id,
    string Name,
    string Email,
    decimal Amount,
    string Currency,
    string Status,
    DateTime DateCreated
);
