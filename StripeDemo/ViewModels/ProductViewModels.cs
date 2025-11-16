namespace StripeDemo.ViewModels;

public record ProductViewModels(
    int Id,
    string Name,
    string Description,
    decimal Price
);
